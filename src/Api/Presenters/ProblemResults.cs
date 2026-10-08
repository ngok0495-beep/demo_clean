using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public static class ProblemResults
{
    public static IActionResult From(ErrorKind kind, string message)
    {
        var (status, title) = kind switch
        {
            ErrorKind.NotFound => (404, "Not Found"),
            ErrorKind.Conflict => (409, "Conflict"),
            _ => (400, "Bad Request")
        };
        return new ObjectResult(new ProblemDetails { Status = status, Title = title, Detail = message })
        { StatusCode = status };
    }
}