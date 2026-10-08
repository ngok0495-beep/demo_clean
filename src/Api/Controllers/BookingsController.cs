using Api.Presenters;
using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController(
    ICreateBookingInputBoundary useCase,
    CreateBookingPresenter presenter) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        await useCase.ExecuteAsync(request);
        return presenter.ViewModel!;
    }
}