param(
    [string]$BaseUrl = 'http://localhost:5227',
    [string]$ResultsPath = 'test-results/http-room-management.json'
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Net.Http
$handler = [System.Net.Http.HttpClientHandler]::new()
$handler.UseProxy = $false
$http = [System.Net.Http.HttpClient]::new($handler)
$http.Timeout = [TimeSpan]::FromSeconds(20)
$results = [System.Collections.Generic.List[object]]::new()
$createdIds = [System.Collections.Generic.List[int]]::new()

function Check($Name, $Condition) {
    $results.Add([pscustomobject]@{ name = $Name; passed = [bool]$Condition })
    if (!$Condition) { throw "Assertion failed: $Name" }
}

function Request($Name, $Method, $Path, $Expected, $Body = $null) {
    $request = [System.Net.Http.HttpRequestMessage]::new([System.Net.Http.HttpMethod]::new($Method), "$BaseUrl$Path")
    if ($null -ne $Body) {
        $json = if ($Body -is [string]) { $Body } else { ConvertTo-Json $Body -Compress }
        $request.Content = [System.Net.Http.StringContent]::new($json, [System.Text.Encoding]::UTF8, 'application/json')
    }
    $response = $http.SendAsync($request).GetAwaiter().GetResult()
    try {
        $content = $response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
        $status = [int]$response.StatusCode
        $results.Add([pscustomobject]@{ name = $Name; method = $Method; path = $Path; expected = $Expected; actual = $status; passed = ($status -eq $Expected) })
        if ($status -ne $Expected) { throw "$Name returned $status; expected $Expected" }
        $data = if ($content -and $response.Content.Headers.ContentType.MediaType -like '*json') { ConvertFrom-Json $content } else { $null }
        if ($status -in 400,404,409 -and $data) { Check "$Name ProblemDetails status" ($data.status -eq $status) }
        return [pscustomobject]@{ Data = $data; Location = [string]$response.Headers.Location; Content = $content }
    } finally { $response.Dispose(); $request.Dispose() }
}

$before = $null
try {
    $before = (Request 'List all rooms' GET '/api/rooms/management' 200).Data
    $types = (Request 'Room types lookup' GET '/api/rooms/types' 200).Data
    Check 'At least two room types available' ($types.Count -ge 2)
    $number = 'T' + [guid]::NewGuid().ToString('N').Substring(0,8)
    $body = @{ number = " $number "; floor = 5; roomTypeId = $types[0].id; status = 0 }
    $created = Request 'Create room' POST '/api/rooms' 201 $body
    $id = $created.Data.id
    $createdIds.Add($id)
    $url = "/api/rooms/$id"
    Check 'Created Location header' ($created.Location -eq $url)
    Check 'Room number trimmed' ($created.Data.number -ceq $number)
    $detail = (Request 'Read created room' GET $url 200).Data
    Check 'Created room persists across requests' ($detail.number -ceq $number)
    $null = Request 'Duplicate create' POST '/api/rooms' 409 $body
    $body.number = $number + 'U'
    $body.roomTypeId = $types[1].id
    $body.status = 2
    $updated = (Request 'Update room' PUT $url 200 $body).Data
    Check 'Update reloads room type name' ($updated.roomTypeName -ceq $types[1].name)
    Check 'Update persists' (((Request 'Read updated room' GET $url 200).Data.number) -ceq $body.number)
    $duplicate = @{ number = $before[0].number; floor = 5; roomTypeId = $types[0].id; status = 0 }
    $null = Request 'Duplicate update' PUT $url 409 $duplicate
    $null = Request 'Patch maintenance' PATCH "$url/status" 200 @{status=3}
    $filtered = (Request 'Filter status and type' GET "/api/rooms/management?status=3&roomTypeId=$($types[1].id)" 200).Data
    Check 'Filters include test room and exclude mismatches' (($filtered.id -contains $id) -and @($filtered | Where-Object { $_.status -ne 3 -or $_.roomTypeId -ne $types[1].id }).Count -eq 0)
    $search = (Request 'Existing availability search' GET '/api/rooms' 200).Data
    Check 'Search excludes maintenance room' ($search.id -notcontains $id)
    $invalidBodies = @(
        @{number='';floor=5;roomTypeId=$types[0].id;status=0},
        @{number='   ';floor=5;roomTypeId=$types[0].id;status=0},
        @{number='12345678901';floor=5;roomTypeId=$types[0].id;status=0},
        @{number=$number;floor=-1;roomTypeId=$types[0].id;status=0},
        @{number=$number;floor=5;roomTypeId=2147483647;status=0},
        @{number=$number;floor=5;roomTypeId=$types[0].id;status=99}
    )
    $n = 0
    foreach ($invalid in $invalidBodies) {
        $n++
        $null = Request "Invalid create $n" POST '/api/rooms' 400 $invalid
        $null = Request "Invalid update $n" PUT $url 400 $invalid
    }
    foreach ($raw in @('{}', '{broken', '{"status":null}', '{"status":99}')) {
        $null = Request "Invalid PATCH $raw" PATCH "$url/status" 400 $raw
    }
    $null = Request 'Missing POST members' POST '/api/rooms' 400 '{}'
    $null = Request 'Missing PUT members' PUT $url 400 '{}'
    $afterErrors = (Request 'Read after rejected changes' GET $url 200).Data
    Check 'Rejected mutations preserve data' ($afterErrors.number -ceq $body.number -and $afterErrors.status -eq 3 -and $afterErrors.roomTypeId -eq $types[1].id)
    $null = Request 'Invalid status filter' GET '/api/rooms/management?status=99' 400
    $null = Request 'Invalid type filter' GET '/api/rooms/management?roomTypeId=0' 400
    $null = Request 'Invalid text filter' GET '/api/rooms/management?status=unknown' 400
    $null = Request 'Missing GET room' GET '/api/rooms/2147483647' 404
    $null = Request 'Missing PUT room' PUT '/api/rooms/2147483647' 404 $body
    $null = Request 'Missing PATCH room' PATCH '/api/rooms/2147483647/status' 404 @{status=0}
    $null = Request 'Missing DELETE room' DELETE '/api/rooms/2147483647' 404
    $null = Request 'Patch occupied' PATCH "$url/status" 200 @{status=1}
    $null = Request 'Reject deleting occupied room' DELETE $url 409
    $null = Request 'Release test room' PATCH "$url/status" 200 @{status=0}
    $null = Request 'Delete room' DELETE $url 204
    $null = $createdIds.Remove($id)
    $null = Request 'Deleted room is missing' GET $url 404
    $null = Request 'Repeated delete is missing' DELETE $url 404
    $null = Request 'Swagger UI' GET '/swagger/index.html' 200
    $swagger = (Request 'Swagger schema' GET '/swagger/v1/swagger.json' 200).Data
    Check 'Swagger documents all six operations' ($swagger.paths.'/api/rooms/management'.get -and $swagger.paths.'/api/rooms/{id}'.get -and $swagger.paths.'/api/rooms'.post -and $swagger.paths.'/api/rooms/{id}'.put -and $swagger.paths.'/api/rooms/{id}/status'.patch -and $swagger.paths.'/api/rooms/{id}'.delete)
} finally {
    try {
        foreach ($cleanupId in $createdIds) {
            $null = Request 'Cleanup status' PATCH "/api/rooms/$cleanupId/status" 200 @{status=0}
            $null = Request 'Cleanup room' DELETE "/api/rooms/$cleanupId" 204
        }
        if ($null -ne $before) {
            $after = (Request 'Final list' GET '/api/rooms/management' 200).Data
            Check 'Original rooms unchanged after test' ((ConvertTo-Json @($before | Sort-Object id) -Compress) -ceq (ConvertTo-Json @($after | Sort-Object id) -Compress))
        }
    } finally {
        $http.Dispose()
        $parent = Split-Path $ResultsPath -Parent
        if ($parent) { $null = New-Item -ItemType Directory -Force -Path $parent }
        [pscustomobject]@{ timestamp = [DateTimeOffset]::Now.ToString('o'); baseUrl = $BaseUrl; checks = $results } | ConvertTo-Json -Depth 8 | Set-Content -Encoding utf8 $ResultsPath
        $results | Select-Object name,expected,actual,passed | Format-Table -AutoSize
    }
}
