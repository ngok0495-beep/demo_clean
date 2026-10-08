using Domain;
namespace Application;

public record CreateBookingRequest(int RoomId, string GuestName, DateOnly CheckIn, DateOnly CheckOut);
public record BookingDto(int Id, int RoomId, string GuestName, DateOnly CheckIn, DateOnly CheckOut, decimal TotalPrice);

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
                       ?? throw new DomainException("Phòng không tồn tại.");

            var booking = Booking.Create(r.RoomId, r.GuestName, r.CheckIn, r.CheckOut);

            var existing = await bookings.GetByRoomAsync(r.RoomId);
            if (existing.Any(b => b.Overlaps(r.CheckIn, r.CheckOut)))
                throw new DomainException("Phòng đã có người đặt trong khoảng ngày này.");

            await bookings.AddAsync(booking);

            var nights = r.CheckOut.DayNumber - r.CheckIn.DayNumber;
            output.PresentSuccess(new BookingDto(booking.Id, booking.RoomId, booking.GuestName,
                                                 booking.CheckIn, booking.CheckOut, nights * room.PricePerNight));
        }
        catch (DomainException ex) { output.PresentError(ex.Message); }
    }
}