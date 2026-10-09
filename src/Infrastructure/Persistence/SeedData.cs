using Domain;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence;

internal static class SeedData
{
    // Chưa có chức năng đăng nhập nên chưa băm mật khẩu thật; khi làm login phải dùng PasswordHasher
    private const string NoPassword = "CHUA_DAT_MAT_KHAU";

    public static void Apply(ModelBuilder mb)
    {
        mb.Entity<RoomType>().HasData(
            new RoomType { Id = 1, Name = "Standard", BasePrice = 500_000,   Amenities = "TV, điều hòa, wifi",              Description = "Phòng tiêu chuẩn" },
            new RoomType { Id = 2, Name = "Deluxe",   BasePrice = 900_000,   Amenities = "TV, điều hòa, wifi, ban công",    Description = "Phòng cao cấp" },
            new RoomType { Id = 3, Name = "Suite",    BasePrice = 1_800_000, Amenities = "TV, điều hòa, wifi, bồn tắm",     Description = "Phòng hạng sang" });

        mb.Entity<SeasonalRate>().HasData(
            new SeasonalRate { Id = 1, RoomTypeId = 1, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 700_000 },
            new SeasonalRate { Id = 2, RoomTypeId = 2, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 1_200_000 },
            new SeasonalRate { Id = 3, RoomTypeId = 3, Name = "Noel 2026", StartDate = new DateOnly(2026, 12, 20), EndDate = new DateOnly(2026, 12, 31), AppliedPrice = 2_400_000 },
            new SeasonalRate { Id = 4, RoomTypeId = 1, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 800_000 },
            new SeasonalRate { Id = 5, RoomTypeId = 2, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 1_400_000 },
            new SeasonalRate { Id = 6, RoomTypeId = 3, Name = "Tết 2027",  StartDate = new DateOnly(2027, 2, 5),   EndDate = new DateOnly(2027, 2, 14),  AppliedPrice = 2_800_000 });

        mb.Entity<Room>().HasData(
            new Room { Id = 1, Number = "101", Floor = 1, RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { Id = 2, Number = "102", Floor = 1, RoomTypeId = 1, Status = RoomStatus.Available },
            new Room { Id = 3, Number = "201", Floor = 2, RoomTypeId = 2, Status = RoomStatus.Available },
            new Room { Id = 4, Number = "301", Floor = 3, RoomTypeId = 3, Status = RoomStatus.Maintenance });

        mb.Entity<Customer>().HasData(
            new Customer { Id = 1, FullName = "Nguyen Van A", Email = "vana@example.com", Phone = "0901000001", PasswordHash = NoPassword, IdNumber = "079000000001", CreatedAt = new DateTime(2026, 10, 1) },
            new Customer { Id = 2, FullName = "Tran Thi B",   Email = "thib@example.com", Phone = "0901000002", PasswordHash = NoPassword, IdNumber = "079000000002", CreatedAt = new DateTime(2026, 10, 1) });

        mb.Entity<Account>().HasData(
            new Account { Id = 1, FullName = "Quản trị viên", Email = "admin@hotel.local",   Phone = "0900000001", Role = "Admin",        PasswordHash = NoPassword },
            new Account { Id = 2, FullName = "Lễ tân 01",     Email = "letan01@hotel.local", Phone = "0900000002", Role = "Receptionist", PasswordHash = NoPassword });

        mb.Entity<Staff>().HasData(
            new Staff { Id = 1, FullName = "Le Van C",  Email = "levanc@hotel.local",  Phone = "0902000001", PasswordHash = NoPassword, NationalId = "079100000001", HireDate = new DateTime(2025, 1, 1) },
            new Staff { Id = 2, FullName = "Pham Thi D", Email = "phamthid@hotel.local", Phone = "0902000002", PasswordHash = NoPassword, NationalId = "079100000002", HireDate = new DateTime(2025, 6, 1) });

        mb.Entity<Service>().HasData(
            new Service { Id = 1, Name = "Giặt là",            Price = 50_000,  Description = "Tính theo bộ đồ" },
            new Service { Id = 2, Name = "Minibar",           Price = 30_000,  Description = "Tính theo món" },
            new Service { Id = 3, Name = "Ăn sáng tại phòng", Price = 100_000, Description = "Mỗi suất" },
            new Service { Id = 4, Name = "Đưa đón sân bay",   Price = 250_000, Description = "Mỗi chuyến" });
    }
}