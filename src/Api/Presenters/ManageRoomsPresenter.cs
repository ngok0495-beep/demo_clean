using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public class ManageRoomsPresenter : IManageRoomsOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }
    public void PresentRooms(IReadOnlyList<RoomDetailsDto> rooms) => ViewModel = new OkObjectResult(rooms);
    public void PresentRoom(RoomDetailsDto room, bool created = false) => ViewModel = created
        ? new CreatedResult($"/api/rooms/{room.Id}", room) : new OkObjectResult(room);
    public void PresentTypes(IReadOnlyList<RoomTypeOptionDto> types) => ViewModel = new OkObjectResult(types);
    public void PresentDeleted() => ViewModel = new NoContentResult();
    public void PresentError(ErrorKind kind, string message) => ViewModel = ProblemResults.From(kind, message);
}
