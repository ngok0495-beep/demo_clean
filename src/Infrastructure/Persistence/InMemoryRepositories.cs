using Application;
using Domain;
namespace Infrastructure.Persistence;

/// <summary>
/// InMemory repository phục vụ môi trường dev/demo, không cần SQL Server.
/// Dữ liệu khởi tạo khớp với SeedData.Apply().
/// </summary>
public class InMemoryRoomRepository : IRoomRepository
{
    private static readonly List<RoomType> _roomTypes =
    [
        new RoomType { Id = 1, Name = "Standard", BasePrice = 500_000, Amenities = "TV, điều hòa, wifi",           Description = "Phòng tiêu chuẩn",
            SeasonalRates = [
                new SeasonalRate { Id = 1, RoomTypeId = 1, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 700_000 },
                new SeasonalRate { Id = 4, RoomTypeId = 1, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 800_000 },
            ]},
        new RoomType { Id = 2, Name = "Deluxe",   BasePrice = 900_000,   Amenities = "TV, điều hòa, wifi, ban công", Description = "Phòng cao cấp",
            SeasonalRates = [
                new SeasonalRate { Id = 2, RoomTypeId = 2, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 1_200_000 },
                new SeasonalRate { Id = 5, RoomTypeId = 2, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 1_400_000 },
            ]},
        new RoomType { Id = 3, Name = "Suite",    BasePrice = 1_800_000, Amenities = "TV, điều hòa, wifi, bồn tắm",  Description = "Phòng hạng sang",
            SeasonalRates = [
                new SeasonalRate { Id = 3, RoomTypeId = 3, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 2_400_000 },
                new SeasonalRate { Id = 6, RoomTypeId = 3, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 2_800_000 },
            ]},
    ];

    private static readonly List<Room> _rooms =
    [
        new Room { Id = 1, Number = "101", Floor = 1, RoomTypeId = 1, RoomType = _roomTypes[0], Status = RoomStatus.Available },
        new Room { Id = 2, Number = "102", Floor = 1, RoomTypeId = 1, RoomType = _roomTypes[0], Status = RoomStatus.Available },
        new Room { Id = 3, Number = "201", Floor = 2, RoomTypeId = 2, RoomType = _roomTypes[1], Status = RoomStatus.Available },
        new Room { Id = 4, Number = "301", Floor = 3, RoomTypeId = 3, RoomType = _roomTypes[2], Status = RoomStatus.Maintenance },
    ];

    public Task<IReadOnlyList<Room>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Room>>(_rooms.OrderBy(r => r.Number).ToList());

    public Task<Room?> GetByIdAsync(int id) =>
        Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
}

public class InMemoryCustomerRepository : ICustomerRepository
{
    private static readonly List<Customer> _customers =
    [
        new Customer { Id = 1, FullName = "Nguyen Van A", Email = "vana@example.com",  Phone = "0901000001", IdNumber = "079000000001", CreatedAt = new DateTime(2026, 10, 1) },
        new Customer { Id = 2, FullName = "Tran Thi B",   Email = "thib@example.com",  Phone = "0901000002", IdNumber = "079000000002", CreatedAt = new DateTime(2026, 10, 1) },
    ];

    public Task<Customer?> GetByIdAsync(int id) =>
        Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
}

public class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = [];
    private int _nextId = 1;

    public Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId) =>
        Task.FromResult<IReadOnlyList<Booking>>(_bookings.Where(b => b.RoomId == roomId).ToList());

    public Task<Booking?> GetByIdAsync(int id) =>
        Task.FromResult(_bookings.FirstOrDefault(b => b.Id == id));

    public Task AddAsync(Booking booking)
    {
        booking.Id = _nextId++;
        _bookings.Add(booking);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Booking booking) => Task.CompletedTask; // đã mutate in-place
}
