using System.Text.Json.Serialization;
using Application;

namespace Api.Controllers;

public record SaveServiceBody(
    [property: JsonRequired] string? Name,
    [property: JsonRequired] decimal Price,
    [property: JsonRequired] string? Description,
    [property: JsonRequired] string? Status)
{
    public SaveServiceRequest ToRequest() => new(Name, Price, Description, Status);
}
