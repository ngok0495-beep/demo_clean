using Domain;
namespace Application;

public interface ICreateBookingInputBoundary { Task ExecuteAsync(CreateBookingRequest request); }
public interface ICreateBookingOutputBoundary
{
    void PresentSuccess(BookingDto booking);
    void PresentError(ErrorKind kind, string message);
}

public record CreateBookingRequest(int CustomerId, int RoomId, DateOnly CheckIn, DateOnly CheckOut, int GuestCount);

public class CreateBookingUseCase(
    IRoomRepository rooms,
    ICustomerRepository customers,
    IBookingRepository bookings,
    ICreateBookingOutputBoundary output) : ICreateBookingInputBoundary
{
    public async Task ExecuteAsync(CreateBookingRequest r)
    {
        try
        {
            var customer = await customers.GetByIdAsync(r.CustomerId)
                           ?? throw new AppException(ErrorKind.NotFound, "Khách hàng không tồn tại.");
            var room = await rooms.GetByIdAsync(r.RoomId)
                       ?? throw new AppException(ErrorKind.NotFound, "Phòng không tồn tại.");
            if (room.Status == RoomStatus.Maintenance)
                throw new AppException(ErrorKind.Conflict, "Phòng đang bảo trì.");

            // Kiểm tra overlap TRƯỚC khi tạo đặt phòng
            var existing = await bookings.GetByRoomAsync(r.RoomId);
            if (existing.Any(b => b.Overlaps(r.CheckIn, r.CheckOut)))
                throw new AppException(ErrorKind.Conflict, "Phòng đã có người đặt trong khoảng ngày này.");

            // Tổng tiền tính theo từng đêm (có giá mùa vụ) và lưu cố định vào đặt phòng
            var total = room.RoomType.TotalFor(r.CheckIn, r.CheckOut);
            var booking = Booking.Create(r.CustomerId, r.RoomId, r.CheckIn, r.CheckOut, r.GuestCount, total);

            await bookings.AddAsync(booking);
            output.PresentSuccess(BookingMapper.ToDto(booking, customer.FullName));
        }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
        catch (DomainException ex) { output.PresentError(ErrorKind.Validation, ex.Message); }
    }
}