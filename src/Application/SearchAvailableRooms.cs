using Domain;
namespace Application;

public record SearchRoomsQuery(DateOnly From, DateOnly To, string? Type);
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
            if (q.To <= q.From) throw new DomainException("Ngày trả phải sau ngày nhận.");

            var result = new List<RoomDto>();
            foreach (var r in await rooms.GetAllAsync())
            {
                if (r.Status == RoomStatus.Maintenance) continue;
                if (q.Type is not null && !r.Type.Equals(q.Type, StringComparison.OrdinalIgnoreCase)) continue;

                var existing = await bookings.GetByRoomAsync(r.Id);
                if (existing.Any(b => b.Overlaps(q.From, q.To))) continue;

                result.Add(new RoomDto(r.Id, r.Number, r.Type, r.PricePerNight));
            }
            output.PresentRooms(result);
        }
        catch (DomainException ex) { output.PresentError(ex.Message); }
    }
}