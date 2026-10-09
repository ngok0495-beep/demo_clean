using Domain;
namespace Application;

public interface ICancelBookingInputBoundary { Task ExecuteAsync(int bookingId); }
public interface ICancelBookingOutputBoundary
{
    void PresentCancelled(CancelResultDto result);
    void PresentError(ErrorKind kind, string message);
}

public class CancelBookingUseCase(
    IBookingRepository bookings,
    TimeProvider time,
    ICancelBookingOutputBoundary output) : ICancelBookingInputBoundary
{
    public async Task ExecuteAsync(int bookingId)
    {
        var booking = await bookings.GetByIdAsync(bookingId);
        if (booking is null)
        {
            output.PresentError(ErrorKind.NotFound, "Không tìm thấy đặt phòng.");
            return;
        }

        try
        {
            var refund = booking.Cancel(time.GetLocalNow().DateTime);
            await bookings.UpdateAsync(booking);
            output.PresentCancelled(new CancelResultDto(booking.Id, booking.Status.ToString(), refund));
        }
        catch (DomainException ex) { output.PresentError(ErrorKind.Conflict, ex.Message); }
    }
}