using Api.Presenters;
using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateBookingRequest request,
        [FromServices] ICreateBookingInputBoundary useCase,
        [FromServices] CreateBookingPresenter presenter)
    {
        await useCase.ExecuteAsync(request);
        return presenter.ViewModel!;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(
        int id,
        [FromServices] IGetBookingInputBoundary useCase,
        [FromServices] GetBookingPresenter presenter)
    {
        await useCase.ExecuteAsync(id);
        return presenter.ViewModel!;
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Cancel(
        int id,
        [FromServices] ICancelBookingInputBoundary useCase,
        [FromServices] CancelBookingPresenter presenter)
    {
        await useCase.ExecuteAsync(id);
        return presenter.ViewModel!;
    }
}