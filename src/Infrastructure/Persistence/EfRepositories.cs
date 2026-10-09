using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence;

public class EfRoomRepository(HotelDbContext db) : IRoomRepository
{
    public async Task<IReadOnlyList<Room>> GetAllAsync() =>
        await db.Rooms.AsNoTracking()
            .Include(r => r.RoomType).ThenInclude(t => t.SeasonalRates)
            .OrderBy(r => r.Number)
            .ToListAsync();

    public Task<Room?> GetByIdAsync(int id) =>
        db.Rooms.AsNoTracking()
            .Include(r => r.RoomType).ThenInclude(t => t.SeasonalRates)
            .FirstOrDefaultAsync(r => r.Id == id);
}

public class EfCustomerRepository(HotelDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(int id) =>
        db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
}

public class EfBookingRepository(HotelDbContext db) : IBookingRepository
{
    public async Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId) =>
        await db.Bookings.AsNoTracking().Where(b => b.RoomId == roomId).ToListAsync();

    // Có tracking để dùng cho UpdateAsync (hủy đặt phòng)
    public Task<Booking?> GetByIdAsync(int id) =>
        db.Bookings.FirstOrDefaultAsync(b => b.Id == id);

    public async Task AddAsync(Booking booking)
    {
        db.Bookings.Add(booking);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        // Entity đã được track bởi EF (GetByIdAsync không dùng AsNoTracking),
        // chỉ cần SaveChanges để persist thay đổi trạng thái.
        await db.SaveChangesAsync();
    }
}