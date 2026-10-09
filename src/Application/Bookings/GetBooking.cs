namespace Application;

public interface IGetBookingInputBoundary { Task ExecuteAsync(int bookingId); }
public interface IGetBookingOutputBoundary
{
    void PresentBooking(BookingDto booking);
    void PresentError(ErrorKind kind, string message);
}

public class GetBookingUseCase(
    ICustomerRepository customers,
    IBookingRepository bookings,
    IGetBookingOutputBoundary output) : IGetBookingInputBoundary
{
    public async Task ExecuteAsync(int bookingId)
    {
        var booking = await bookings.GetByIdAsync(bookingId);
        if (booking is null)
        {
            output.PresentError(ErrorKind.NotFound, "Không tìm thấy đặt phòng.");
            return;
        }
        var customer = await customers.GetByIdAsync(booking.CustomerId);
        output.PresentBooking(BookingMapper.ToDto(booking, customer?.FullName ?? ""));
    }
}