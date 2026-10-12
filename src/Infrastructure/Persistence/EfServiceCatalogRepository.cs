using Application;
using Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class EfServiceCatalogRepository(HotelDbContext db) : IServiceCatalogRepository
{
    public async Task<IReadOnlyList<Service>> ListAsync(string? status)
    {
        var query = db.Services.AsNoTracking().AsQueryable();
        if (status is not null) query = query.Where(s => s.Status == status);
        return await query.OrderBy(s => s.Name).ToListAsync();
    }

    public Task<Service?> FindAsync(int id) => db.Services.FirstOrDefaultAsync(s => s.Id == id);

    public Task<bool> NameExistsAsync(string name, int? exceptId) =>
        db.Services.AnyAsync(s => s.Name.ToLower() == name.ToLower()
            && (!exceptId.HasValue || s.Id != exceptId.Value));

    public Task<bool> HasUsageAsync(int id) => db.ServiceUsages.AnyAsync(u => u.ServiceId == id);

    public async Task AddAsync(Service service)
    {
        db.Services.Add(service);
        await SaveAsync();
    }

    public async Task DeleteAsync(Service service)
    {
        db.Services.Remove(service);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 547 })
        {
            throw new AppException(ErrorKind.Conflict, "Dịch vụ đang được tham chiếu. Hãy chuyển trạng thái dịch vụ sang Inactive.");
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new AppException(ErrorKind.Conflict, "Dịch vụ đã bị thay đổi hoặc xóa. Hãy tải lại dữ liệu.");
        }
    }
}
