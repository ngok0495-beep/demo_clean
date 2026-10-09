using Domain;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence;

public class HotelDbContext(DbContextOptions<HotelDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Staff> StaffMembers => Set<Staff>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<SeasonalRate> SeasonalRates => Set<SeasonalRate>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomIncident> RoomIncidents => Set<RoomIncident>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServiceUsage> ServiceUsages => Set<ServiceUsage>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PaymentTransaction> Transactions => Set<PaymentTransaction>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        const string Money = "decimal(18,2)";

        mb.Entity<Account>(e =>
        {
            e.ToTable("TAIKHOAN");
            e.Property(x => x.Id).HasColumnName("MaTK");
            e.Property(x => x.FullName).HasColumnName("HoTen").HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasColumnName("Email").HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasColumnName("SoDienThoai").HasMaxLength(20);
            e.Property(x => x.Role).HasColumnName("VaiTro").HasMaxLength(30).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnName("MatKhau").HasMaxLength(255).IsRequired();
            e.Property(x => x.Status).HasColumnName("TrangThai").HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.Email).IsUnique();
        });

        mb.Entity<Staff>(e =>
        {
            e.ToTable("NHANVIEN");
            e.Property(x => x.Id).HasColumnName("MaNV");
            e.Property(x => x.FullName).HasColumnName("HoTen").HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasColumnName("Email").HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasColumnName("SoDienThoai").HasMaxLength(20);
            e.Property(x => x.PasswordHash).HasColumnName("MatKhau").HasMaxLength(255).IsRequired();
            e.Property(x => x.NationalId).HasColumnName("CCCD").HasMaxLength(20);
            e.Property(x => x.HireDate).HasColumnName("NgayVaoLam");
            e.HasIndex(x => x.Email).IsUnique();
        });

        mb.Entity<Customer>(e =>
        {
            e.ToTable("KHACHHANG");
            e.Property(x => x.Id).HasColumnName("MaKH");
            e.Property(x => x.FullName).HasColumnName("HoTen").HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasColumnName("Email").HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasColumnName("SoDienThoai").HasMaxLength(20);
            e.Property(x => x.PasswordHash).HasColumnName("MatKhau").HasMaxLength(255).IsRequired();
            e.Property(x => x.IdNumber).HasColumnName("CCCD_HoChieu").HasMaxLength(20);
            e.Property(x => x.CreatedAt).HasColumnName("NgayTao");
            e.HasIndex(x => x.Email).IsUnique();
        });

        mb.Entity<RoomType>(e =>
        {
            e.ToTable("LOAIPHONG");
            e.Property(x => x.Id).HasColumnName("MaLoaiPhong");
            e.Property(x => x.Name).HasColumnName("TenLoaiPhong").HasMaxLength(50).IsRequired();
            e.Property(x => x.BasePrice).HasColumnName("GiaCoBan").HasColumnType(Money);
            e.Property(x => x.Amenities).HasColumnName("TienNghi").HasMaxLength(500);
            e.Property(x => x.Description).HasColumnName("MoTa").HasMaxLength(500);
            e.Property(x => x.Status).HasColumnName("TrangThai").HasMaxLength(20).IsRequired();
            e.HasMany(x => x.SeasonalRates).WithOne().HasForeignKey(x => x.RoomTypeId);
            e.HasIndex(x => x.Name).IsUnique();
        });

        mb.Entity<SeasonalRate>(e =>
        {
            e.ToTable("GIAMUAVU");
            e.Property(x => x.Id).HasColumnName("MaGia");
            e.Property(x => x.RoomTypeId).HasColumnName("MaLoaiPhong");
            e.Property(x => x.Name).HasColumnName("TenGiaiDoan").HasMaxLength(100).IsRequired();
            e.Property(x => x.StartDate).HasColumnName("NgayBatDau");
            e.Property(x => x.EndDate).HasColumnName("NgayKetThuc");
            e.Property(x => x.AppliedPrice).HasColumnName("GiaApDung").HasColumnType(Money);
        });

        mb.Entity<Room>(e =>
        {
            e.ToTable("PHONG");
            e.Property(x => x.Id).HasColumnName("MaPhong");
            e.Property(x => x.Number).HasColumnName("SoPhong").HasMaxLength(10).IsRequired();
            e.Property(x => x.Floor).HasColumnName("SoTang");
            e.Property(x => x.RoomTypeId).HasColumnName("MaLoaiPhong");
            e.Property(x => x.Status).HasColumnName("TrangThaiPhong").HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.RoomType).WithMany().HasForeignKey(x => x.RoomTypeId);
            e.HasIndex(x => x.Number).IsUnique();
        });

        mb.Entity<RoomIncident>(e =>
        {
            e.ToTable("SUCOPHONG");
            e.Property(x => x.Id).HasColumnName("MaSuCo");
            e.Property(x => x.RoomId).HasColumnName("MaPhong");
            e.Property(x => x.AccountId).HasColumnName("MaTK");
            e.Property(x => x.Description).HasColumnName("MoTa").HasMaxLength(1000);
            e.Property(x => x.ReportedAt).HasColumnName("ThoiDiemBaoCao");
            e.Property(x => x.Status).HasColumnName("TrangThai").HasMaxLength(20).IsRequired();
            e.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId);
            e.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);
        });

        mb.Entity<Booking>(e =>
        {
            e.ToTable("DATPHONG");
            e.Property(x => x.Id).HasColumnName("MaDatPhong");
            e.Property(x => x.CustomerId).HasColumnName("MaKH");
            e.Property(x => x.RoomId).HasColumnName("MaPhong");
            e.Property(x => x.AccountId).HasColumnName("MaTK");
            e.Property(x => x.CheckIn).HasColumnName("NgayNhan");
            e.Property(x => x.CheckOut).HasColumnName("NgayTra");
            e.Property(x => x.GuestCount).HasColumnName("SoKhach");
            e.Property(x => x.TotalPrice).HasColumnName("TongTien").HasColumnType(Money);
            e.Property(x => x.Status).HasColumnName("TrangThaiDatPhong").HasConversion<string>().HasMaxLength(20);
            e.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId);
            e.HasOne<Room>().WithMany().HasForeignKey(x => x.RoomId);
            e.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);   // tùy chọn vì AccountId là int?
            e.HasIndex(x => new { x.RoomId, x.CheckIn, x.CheckOut });
        });

        mb.Entity<Service>(e =>
        {
            e.ToTable("DICHVU");
            e.Property(x => x.Id).HasColumnName("MaDichVu");
            e.Property(x => x.Name).HasColumnName("TenDichVu").HasMaxLength(100).IsRequired();
            e.Property(x => x.Price).HasColumnName("DonGia").HasColumnType(Money);
            e.Property(x => x.Description).HasColumnName("MoTa").HasMaxLength(500);
            e.Property(x => x.Status).HasColumnName("TrangThai").HasMaxLength(20).IsRequired();
        });

        mb.Entity<ServiceUsage>(e =>
        {
            e.ToTable("SUDUNGDICHVU");
            e.Property(x => x.Id).HasColumnName("MaSDDV");
            e.Property(x => x.BookingId).HasColumnName("MaDatPhong");
            e.Property(x => x.ServiceId).HasColumnName("MaDichVu");
            e.Property(x => x.AccountId).HasColumnName("MaTK");
            e.Property(x => x.Quantity).HasColumnName("SoLuong");
            e.Property(x => x.UnitPrice).HasColumnName("DonGiaLuu").HasColumnType(Money);
            e.Property(x => x.UsedAt).HasColumnName("ThoiDiemSuDung");
            e.HasOne<Booking>().WithMany().HasForeignKey(x => x.BookingId);
            e.HasOne<Service>().WithMany().HasForeignKey(x => x.ServiceId);
            e.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);
        });

        mb.Entity<Invoice>(e =>
        {
            e.ToTable("HOADON");
            e.Property(x => x.Id).HasColumnName("MaHoaDon");
            e.Property(x => x.BookingId).HasColumnName("MaDatPhong");
            e.Property(x => x.AccountId).HasColumnName("MaTK");
            e.Property(x => x.RoomAmount).HasColumnName("TienPhong").HasColumnType(Money);
            e.Property(x => x.ServiceAmount).HasColumnName("TienDichVu").HasColumnType(Money);
            e.Property(x => x.TotalAmount).HasColumnName("TongTien").HasColumnType(Money);
            e.Property(x => x.PaymentMethod).HasColumnName("PhuongThucThanhToan").HasMaxLength(30);
            e.Property(x => x.Status).HasColumnName("TrangThaiHoaDon").HasMaxLength(20).IsRequired();
            e.HasOne<Booking>().WithMany().HasForeignKey(x => x.BookingId);
            e.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);
        });

        mb.Entity<PaymentTransaction>(e =>
        {
            e.ToTable("GIAODICH");
            e.Property(x => x.Id).HasColumnName("MaGD");
            e.Property(x => x.BookingId).HasColumnName("MaDatPhong");
            e.Property(x => x.InvoiceId).HasColumnName("MaHoaDon");
            e.Property(x => x.Gateway).HasColumnName("CongThanhToan").HasMaxLength(30);
            e.Property(x => x.GatewayTransactionId).HasColumnName("MaGDCong").HasMaxLength(100);
            e.Property(x => x.Type).HasColumnName("LoaiGD").HasMaxLength(30);
            e.Property(x => x.Amount).HasColumnName("SoTien").HasColumnType(Money);
            e.Property(x => x.Status).HasColumnName("TrangThai").HasMaxLength(20);
            e.Property(x => x.Reconciled).HasColumnName("DaDoiSoat");
            e.Property(x => x.OccurredAt).HasColumnName("ThoiDiem");
            e.HasOne<Booking>().WithMany().HasForeignKey(x => x.BookingId);
            e.HasOne<Invoice>().WithMany().HasForeignKey(x => x.InvoiceId);   // tùy chọn vì InvoiceId là int?
        });

        mb.Entity<ActivityLog>(e =>
        {
            e.ToTable("NHAT_KY_HOAT_DONG");
            e.Property(x => x.Id).HasColumnName("ma_log");
            e.Property(x => x.AccountId).HasColumnName("ma_tk");
            e.Property(x => x.Timestamp).HasColumnName("thoi_gian");
            e.Property(x => x.Action).HasColumnName("hanh_dong").HasMaxLength(500);
            e.HasOne<Account>().WithMany().HasForeignKey(x => x.AccountId);
        });

        // Tránh lỗi "multiple cascade paths" của SQL Server: không tự xóa dây chuyền
        foreach (var fk in mb.Model.GetEntityTypes().SelectMany(t => t.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        SeedData.Apply(mb);
    }
}