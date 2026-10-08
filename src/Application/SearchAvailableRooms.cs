using Domain;
namespace Application;

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
                if (q.Type is not null && !r.Type.Equals(q.Type, StringComparison.OrdinalIgnoreCase)) continue;

                if (byDate)
                {
                    if (r.Status == RoomStatus.Maintenance) continue;
                    var existing = await bookings.GetByRoomAsync(r.Id);
                    if (existing.Any(b => b.Overlaps(q.From!.Value, q.To!.Value))) continue;
                }
                result.Add(new RoomDto(r.Id, r.Number, r.Type, r.PricePerNight));
            }
            output.PresentRooms(result);
        }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
        catch (DomainException ex) { output.PresentError(ErrorKind.Validation, ex.Message); }
    }
}