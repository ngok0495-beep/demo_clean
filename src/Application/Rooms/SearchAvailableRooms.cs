using Domain;
namespace Application;

public interface ISearchAvailableRoomsInputBoundary { Task ExecuteAsync(SearchRoomsQuery query); }
public interface ISearchAvailableRoomsOutputBoundary
{
    void PresentRooms(IReadOnlyList<RoomDto> rooms);
    void PresentError(ErrorKind kind, string message);
}

public record SearchRoomsQuery(DateOnly? From, DateOnly? To, string? Type);
public record RoomDto(int Id, string Number, string Type, decimal PricePerNight);

public class SearchAvailableRoomsUseCase(
    IRoomRepository rooms,
    IBookingRepository bookings,
    ISearchAvailableRoomsOutputBoundary output) : ISearchAvailableRoomsInputBoundary
{
    public async Task ExecuteAsync(SearchRoomsQuery q)
    {
        try
        {
            if (q.From.HasValue != q.To.HasValue)
                throw new AppException(ErrorKind.Validation, "Phải truyền đủ cả from và to, hoặc không truyền cả hai.");

            var byDate = q.From.HasValue;
            if (byDate && q.To <= q.From)
                throw new AppException(ErrorKind.Validation, "Ngày trả phải sau ngày nhận.");

            var result = new List<RoomDto>();
            foreach (var r in await rooms.GetAllAsync())
            {
                if (q.Type is not null && !r.RoomType.Name.Equals(q.Type, StringComparison.OrdinalIgnoreCase)) continue;

                // Phòng đang bảo trì không bao giờ hiển thị (dù có hay không có filter ngày)
                if (r.Status == RoomStatus.Maintenance) continue;

                if (byDate)
                {
                    var existing = await bookings.GetByRoomAsync(r.Id);
                    if (existing.Any(b => b.Overlaps(q.From!.Value, q.To!.Value))) continue;
                }

                // Có ngày: giá của đêm nhận phòng (đã tính mùa vụ); không có ngày: giá cơ bản
                var price = byDate ? r.RoomType.PriceOn(q.From!.Value) : r.RoomType.BasePrice;
                result.Add(new RoomDto(r.Id, r.Number, r.RoomType.Name, price));
            }
            output.PresentRooms(result);
        }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
        catch (DomainException ex) { output.PresentError(ErrorKind.Validation, ex.Message); }
    }
}