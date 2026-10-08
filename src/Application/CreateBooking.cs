using Domain;
namespace Application;

public record CreateBookingRequest(int RoomId, string GuestName, DateOnly CheckIn, DateOnly CheckOut);

public class CreateBookingUseCase(
    IRoomRepository rooms,
    IBookingRepository bookings,
    ICreateBookingOutputBoundary output) : ICreateBookingInputBoundary
{
    public async Task ExecuteAsync(CreateBookingRequest r)
    {
        try
        {
            var room = await rooms.GetByIdAsync(r.RoomId)
                       ?? throw new AppException(ErrorKind.NotFound, "Phòng không tồn tại.");

            var booking = Booking.Create(r.RoomId, r.GuestName, r.CheckIn, r.CheckOut);

            var existing = await bookings.GetByRoomAsync(r.RoomId);
            if (existing.Any(b => b.Overlaps(r.CheckIn, r.CheckOut)))
                throw new AppException(ErrorKind.Conflict, "Phòng đã có người đặt trong khoảng ngày này.");

            await bookings.AddAsync(booking);
            output.PresentSuccess(BookingMapper.ToDto(booking, room));
        }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
        catch (DomainException ex) { output.PresentError(ErrorKind.Validation, ex.Message); }
    }
}