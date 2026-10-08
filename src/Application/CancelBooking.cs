using Domain;
namespace Application;

public class CancelBookingUseCase(
    IRoomRepository rooms,
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
            var room = (await rooms.GetByIdAsync(booking.RoomId))!;
            // Giả định khách đã thanh toán đủ tổng tiền khi đặt
            var refund = booking.Cancel(time.GetLocalNow().DateTime, BookingMapper.TotalPrice(booking, room));
            output.PresentCancelled(new CancelResultDto(booking.Id, booking.Status.ToString(), refund));
        }
        catch (DomainException ex) { output.PresentError(ErrorKind.Conflict, ex.Message); }
    }
}