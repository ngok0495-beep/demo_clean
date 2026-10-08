using Application;
using Microsoft.AspNetCore.Mvc;
namespace Api.Presenters;

public record BookingViewModel(int Id, string Guest, string CheckIn, string CheckOut, string Total);

public class CreateBookingPresenter : ICreateBookingOutputBoundary
{
    public IActionResult? ViewModel { get; private set; }

    public void PresentSuccess(BookingDto b) =>
        ViewModel = new CreatedResult($"/api/bookings/{b.Id}",
            new BookingViewModel(b.Id, b.GuestName,
                                 b.CheckIn.ToString("dd/MM/yyyy"),
                                 b.CheckOut.ToString("dd/MM/yyyy"),
                                 $"{b.TotalPrice:N0} VND"));

    public void PresentError(string message) =>
        ViewModel = new BadRequestObjectResult(new { error = message });
}