using Api.Presenters;
using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController(
    ISearchAvailableRoomsInputBoundary useCase,
    SearchRoomsPresenter presenter) : ControllerBase
{
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable(
        [FromQuery(Name = "from")] DateOnly checkIn,
        [FromQuery(Name = "to")] DateOnly checkOut,
        [FromQuery] string? type)
    {
        await useCase.ExecuteAsync(new SearchRoomsQuery(checkIn, checkOut, type));
        return presenter.ViewModel!;
    }
}