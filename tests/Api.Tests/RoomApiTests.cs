using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Application;
using Domain;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Api.Tests;

// Real HTTP pipeline + EF relational database. SQLite is only used inside tests.
public sealed class RoomApiTests : IDisposable
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public RoomApiTests()
    {
        connection.Open();
        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<HotelDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<HotelDbContext>>();
                services.AddDbContext<HotelDbContext>(o => o.UseSqlite(connection));
            });
        });
        client = factory.CreateClient();
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<HotelDbContext>().Database.EnsureCreated();
    }

    [Fact]
    public async Task Crud_persists_across_requests_and_preserves_search_contract()
    {
        var response = await client.PostAsJsonAsync("/api/rooms", new SaveRoomRequest(" 501 ", 5, 1, RoomStatus.Available));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = (await response.Content.ReadFromJsonAsync<RoomDetailsDto>())!;
        Assert.Equal("501", created.Number);
        Assert.Equal($"/api/rooms/{created.Id}", response.Headers.Location!.ToString());
        var url = response.Headers.Location.ToString();
        Assert.Equal(created, await client.GetFromJsonAsync<RoomDetailsDto>(url));

        var updated = await client.PutAsJsonAsync(url, new SaveRoomRequest("502", 5, 2, RoomStatus.Cleaning));
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var result = (await updated.Content.ReadFromJsonAsync<RoomDetailsDto>())!;
        Assert.Equal("Deluxe", result.RoomTypeName);
        Assert.Equal("502", result.Number);
        Assert.Equal(2, (await client.GetFromJsonAsync<RoomDetailsDto>(url))!.RoomTypeId);

        Assert.Equal(HttpStatusCode.OK, (await client.PatchAsJsonAsync(url + "/status", new ChangeRoomStatusRequest(RoomStatus.Maintenance))).StatusCode);
        var all = (await client.GetFromJsonAsync<List<RoomDetailsDto>>("/api/rooms/management?status=3&roomTypeId=2"))!;
        Assert.Contains(all, r => r.Id == created.Id);
        var search = (await client.GetFromJsonAsync<List<RoomDto>>("/api/rooms"))!;
        Assert.DoesNotContain(search, r => r.Id == created.Id);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(url)).StatusCode);
    }

    [Theory]
    [InlineData("", 1, 1, 0)]
    [InlineData("   ", 1, 1, 0)]
    [InlineData("12345678901", 1, 1, 0)]
    [InlineData("501", -1, 1, 0)]
    [InlineData("501", 1, 99999, 0)]
    [InlineData("501", 1, 1, 99)]
    public async Task Invalid_input_is_400(string number, int floor, int type, int status)
    {
        var response = await client.PostAsJsonAsync("/api/rooms", new SaveRoomRequest(number, floor, type, (RoomStatus)status));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(400, problem.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Duplicate_number_is_409_on_create_and_update()
    {
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/rooms", new SaveRoomRequest("101", 1, 1, 0))).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync("/api/rooms/2", new SaveRoomRequest("101", 1, 1, 0))).StatusCode);
        Assert.Equal("102", (await client.GetFromJsonAsync<RoomDetailsDto>("/api/rooms/2"))!.Number);
        Assert.Equal(HttpStatusCode.OK, (await client.PutAsJsonAsync("/api/rooms/1", new SaveRoomRequest("101", 1, 1, 0))).StatusCode);
    }

    [Fact]
    public async Task Missing_room_is_404_for_every_mutation()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/api/rooms/99999")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PutAsJsonAsync("/api/rooms/99999", new SaveRoomRequest("501", 5, 1, 0))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PatchAsJsonAsync("/api/rooms/99999/status", new ChangeRoomStatusRequest(0))).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync("/api/rooms/99999")).StatusCode);
    }

    [Fact]
    public async Task Booking_history_survives_delete_attempt_and_old_booking_routes_work()
    {
        var from = DateOnly.FromDateTime(DateTime.Today).AddDays(30);
        var response = await client.PostAsJsonAsync("/api/bookings", new CreateBookingRequest(1, 1, from, from.AddDays(2), 1));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var url = response.Headers.Location!.ToString();
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync("/api/rooms/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.DeleteAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync("/api/rooms/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(url)).StatusCode);
    }

    [Fact]
    public async Task Incident_history_and_occupied_rooms_cannot_be_deleted()
    {
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<HotelDbContext>();
            db.RoomIncidents.Add(new RoomIncident { RoomId = 2, AccountId = 1, Description = "Test", ReportedAt = DateTime.UtcNow });
            await db.SaveChangesAsync();
        }
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync("/api/rooms/2")).StatusCode);
        await client.PatchAsJsonAsync("/api/rooms/3/status", new ChangeRoomStatusRequest(RoomStatus.Occupied));
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync("/api/rooms/3")).StatusCode);
    }

    [Fact]
    public async Task Swagger_and_lookup_are_available_and_invalid_filters_are_rejected()
    {
        Assert.Equal(3, (await client.GetFromJsonAsync<List<RoomTypeOptionDto>>("/api/rooms/types"))!.Count);
        Assert.Equal(4, (await client.GetFromJsonAsync<List<RoomDetailsDto>>("/api/rooms/management"))!.Count);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/rooms/management?status=99")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/api/rooms/management?roomTypeId=0")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PatchAsJsonAsync("/api/rooms/1/status", new ChangeRoomStatusRequest((RoomStatus)99))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/api/rooms", new StringContent("{broken", Encoding.UTF8, "application/json"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PatchAsync("/api/rooms/1/status", new StringContent("{}", Encoding.UTF8, "application/json"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/swagger/index.html")).StatusCode);
        var swagger = await client.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");
        Assert.True(swagger.GetProperty("paths").GetProperty("/api/rooms").TryGetProperty("post", out _));
        Assert.True(swagger.GetProperty("paths").GetProperty("/api/rooms/{id}").TryGetProperty("delete", out _));
    }

    [Theory]
    [InlineData("{\"number\":\"changed\",\"floor\":-1,\"roomTypeId\":2,\"status\":3}")]
    [InlineData("{\"number\":\"changed\",\"floor\":1,\"roomTypeId\":99999,\"status\":3}")]
    [InlineData("{\"number\":\"changed\",\"floor\":1,\"roomTypeId\":2,\"status\":99}")]
    [InlineData("{\"number\":null,\"floor\":1,\"roomTypeId\":1,\"status\":0}")]
    [InlineData("{}")]
    [InlineData("{broken")]
    public async Task Rejected_update_preserves_all_room_fields(string json)
    {
        var before = await client.GetFromJsonAsync<RoomDetailsDto>("/api/rooms/1");
        var response = await client.PutAsync("/api/rooms/1", new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await client.GetFromJsonAsync<RoomDetailsDto>("/api/rooms/1"));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"status\":null}")]
    [InlineData("{\"status\":99}")]
    [InlineData("{\"status\":\"unknown\"}")]
    public async Task Rejected_patch_preserves_non_default_status(string json)
    {
        var before = await client.GetFromJsonAsync<RoomDetailsDto>("/api/rooms/4");
        Assert.Equal(RoomStatus.Maintenance, before!.Status);
        var response = await client.PatchAsync("/api/rooms/4/status", new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(before, await client.GetFromJsonAsync<RoomDetailsDto>("/api/rooms/4"));
    }

    [Fact]
    public async Task Combined_filters_exclude_nonmatching_rooms()
    {
        var rooms = (await client.GetFromJsonAsync<List<RoomDetailsDto>>("/api/rooms/management?status=0&roomTypeId=1"))!;
        Assert.Equal(2, rooms.Count);
        Assert.All(rooms, room =>
        {
            Assert.Equal(RoomStatus.Available, room.Status);
            Assert.Equal(1, room.RoomTypeId);
        });
        Assert.Empty((await client.GetFromJsonAsync<List<RoomDetailsDto>>("/api/rooms/management?status=3&roomTypeId=1"))!);
    }

    public void Dispose()
    {
        client.Dispose();
        factory.Dispose();
        connection.Dispose();
    }
}
