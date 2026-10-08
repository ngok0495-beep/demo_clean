using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public record RoomViewModel(string Number, string Type, string Price);

public class SearchRoomsPresenter : ISearchAvailableRoomsOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }

    public void PresentRooms(IReadOnlyList<RoomDto> rooms) =>
        ViewModel = new OkObjectResult(
            rooms.Select(r => new RoomViewModel(r.Number, r.Type, $"{r.PricePerNight:N0} VND/đêm")));

    public void PresentError(string message) =>
        ViewModel = new BadRequestObjectResult(new { error = message });
}