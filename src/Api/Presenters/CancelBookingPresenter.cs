using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public class CancelBookingPresenter : ICancelBookingOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }

    public void PresentCancelled(CancelResultDto r) =>
        ViewModel = new OkObjectResult(new
        {
            bookingId = r.BookingId,
            status = r.Status,
            refund = $"{r.RefundAmount:N0} VND"
        });

    public void PresentError(ErrorKind kind, string message) =>
        ViewModel = ProblemResults.From(kind, message);
}