using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public class GetBookingPresenter : IGetBookingOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }

    public void PresentBooking(BookingDto b) =>
        ViewModel = new OkObjectResult(BookingViewModel.From(b));

    public void PresentError(ErrorKind kind, string message) =>
        ViewModel = ProblemResults.From(kind, message);
}