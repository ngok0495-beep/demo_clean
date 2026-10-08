using Application;
using Domain;
namespace Infrastructure;

public class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms =
    [
        new() { Id = 1, Number = "101", Type = "Standard", PricePerNight = 500_000, Status = RoomStatus.Available },
        new() { Id = 2, Number = "102", Type = "Standard", PricePerNight = 500_000, Status = RoomStatus.Available },
        new() { Id = 3, Number = "201", Type = "Deluxe",   PricePerNight = 900_000, Status = RoomStatus.Available },
        new() { Id = 4, Number = "301", Type = "Suite",    PricePerNight = 1_800_000, Status = RoomStatus.Maintenance },
    ];

    public Task<IReadOnlyList<Room>> GetAllAsync() => Task.FromResult<IReadOnlyList<Room>>(_rooms);
    public Task<Room?> GetByIdAsync(int id) => Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
}

public class InMemoryBookingRepository : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(int id) =>
    Task.FromResult(_items.FirstOrDefault(b => b.Id == id));
    private readonly List<Booking> _items = [];
    private int _nextId = 1;

    public Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId) =>
        Task.FromResult<IReadOnlyList<Booking>>(_items.Where(b => b.RoomId == roomId).ToList());

    public Task AddAsync(Booking booking)
    {
        booking.Id = _nextId++;
        _items.Add(booking);
        return Task.CompletedTask;
    }
}
