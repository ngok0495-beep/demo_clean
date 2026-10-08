using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public class SearchRoomsPresenter : ISearchAvailableRoomsOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }

    public void PresentRooms(IReadOnlyList<RoomDto> rooms) =>
        ViewModel = new OkObjectResult(
            rooms.Select(r => new RoomViewModel(r.Id, r.Number, r.Type, $"{r.PricePerNight:N0} VND/đêm")));

    public void PresentError(ErrorKind kind, string message) =>
        ViewModel = ProblemResults.From(kind, message);
}