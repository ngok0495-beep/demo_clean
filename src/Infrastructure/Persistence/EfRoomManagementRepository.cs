using Application;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence;

public class EfRoomManagementRepository(HotelDbContext db) : IRoomManagementRepository
{
    public async Task<IReadOnlyList<Room>> ListAsync(RoomStatus? status, int? roomTypeId)
    {
        var query = db.Rooms.AsNoTracking().Include(r => r.RoomType).AsQueryable();
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);
        if (roomTypeId.HasValue) query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
        return await query.OrderBy(r => r.Number).ToListAsync();
    }

    public Task<Room?> FindAsync(int id) => db.Rooms.Include(r => r.RoomType).FirstOrDefaultAsync(r => r.Id == id);
    public async Task<IReadOnlyList<RoomType>> ListTypesAsync() => await db.RoomTypes.AsNoTracking().OrderBy(t => t.Id).ToListAsync();
    public Task<bool> TypeExistsAsync(int id) => db.RoomTypes.AnyAsync(t => t.Id == id);
    public Task<bool> NumberExistsAsync(string number, int? exceptId) =>
        db.Rooms.AnyAsync(r => r.Number == number && (!exceptId.HasValue || r.Id != exceptId.Value));
    public async Task<bool> HasHistoryAsync(int id) =>
        await db.Bookings.AnyAsync(b => b.RoomId == id) || await db.RoomIncidents.AnyAsync(i => i.RoomId == id);

    public async Task AddAsync(Room room)
    {
        db.Rooms.Add(room);
        await SaveAsync();
    }

    public async Task DeleteAsync(Room room)
    {
        db.Rooms.Remove(room);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        try { await db.SaveChangesAsync(); }
        // The unique index / foreign keys also protect against concurrent requests.
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new AppException(ErrorKind.Conflict, "Số phòng đã tồn tại.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new AppException(ErrorKind.Conflict, "Dữ liệu liên quan đã thay đổi hoặc phòng đang được tham chiếu.");
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new AppException(ErrorKind.Conflict, "Phòng đã bị thay đổi hoặc xóa. Hãy tải lại dữ liệu.");
        }
    }
}
