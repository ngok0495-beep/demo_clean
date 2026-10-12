using System.Text.Json.Serialization;
using Application;
using Domain;
namespace Api.Controllers;

// Required JSON members prevent an empty PATCH body from silently selecting Available (0).
public record SaveRoomBody(
    [property: JsonRequired] string Number,
    [property: JsonRequired] int Floor,
    [property: JsonRequired] int RoomTypeId,
    [property: JsonRequired] RoomStatus Status)
{
    public SaveRoomRequest ToRequest() => new(Number, Floor, RoomTypeId, Status);
}

public record ChangeRoomStatusBody([property: JsonRequired] RoomStatus Status);
