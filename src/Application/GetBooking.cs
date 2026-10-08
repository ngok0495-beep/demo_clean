namespace Application;

public class GetBookingUseCase(
    IRoomRepository rooms,
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
        var room = (await rooms.GetByIdAsync(booking.RoomId))!;
        output.PresentBooking(BookingMapper.ToDto(booking, room));
    }
}