using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialHotelSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DICHVU",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DICHVU", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "KHACHHANG",
                columns: table => new
                {
                    MaKH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CCCD_HoChieu = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KHACHHANG", x => x.MaKH);
                });

            migrationBuilder.CreateTable(
                name: "LOAIPHONG",
                columns: table => new
                {
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiPhong = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaCoBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienNghi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAIPHONG", x => x.MaLoaiPhong);
                });

            migrationBuilder.CreateTable(
                name: "NHANVIEN",
                columns: table => new
                {
                    MaNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayVaoLam = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHANVIEN", x => x.MaNV);
                });

            migrationBuilder.CreateTable(
                name: "TAIKHOAN",
                columns: table => new
                {
                    MaTK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TAIKHOAN", x => x.MaTK);
                });

            migrationBuilder.CreateTable(
                name: "GIAMUAVU",
                columns: table => new
                {
                    MaGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false),
                    TenGiaiDoan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgayBatDau = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: false),
                    GiaApDung = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GIAMUAVU", x => x.MaGia);
                    table.ForeignKey(
                        name: "FK_GIAMUAVU_LOAIPHONG_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LOAIPHONG",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PHONG",
                columns: table => new
                {
                    MaPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoPhong = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoTang = table.Column<int>(type: "int", nullable: false),
                    MaLoaiPhong = table.Column<int>(type: "int", nullable: false),
                    TrangThaiPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHONG", x => x.MaPhong);
                    table.ForeignKey(
                        name: "FK_PHONG_LOAIPHONG_MaLoaiPhong",
                        column: x => x.MaLoaiPhong,
                        principalTable: "LOAIPHONG",
                        principalColumn: "MaLoaiPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_HOAT_DONG",
                columns: table => new
                {
                    ma_log = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ma_tk = table.Column<int>(type: "int", nullable: false),
                    thoi_gian = table.Column<DateTime>(type: "datetime2", nullable: false),
                    hanh_dong = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_HOAT_DONG", x => x.ma_log);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_HOAT_DONG_TAIKHOAN_ma_tk",
                        column: x => x.ma_tk,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DATPHONG",
                columns: table => new
                {
                    MaDatPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKH = table.Column<int>(type: "int", nullable: false),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    MaTK = table.Column<int>(type: "int", nullable: true),
                    NgayNhan = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayTra = table.Column<DateOnly>(type: "date", nullable: false),
                    SoKhach = table.Column<int>(type: "int", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThaiDatPhong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DATPHONG", x => x.MaDatPhong);
                    table.ForeignKey(
                        name: "FK_DATPHONG_KHACHHANG_MaKH",
                        column: x => x.MaKH,
                        principalTable: "KHACHHANG",
                        principalColumn: "MaKH",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DATPHONG_PHONG_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PHONG",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DATPHONG_TAIKHOAN_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SUCOPHONG",
                columns: table => new
                {
                    MaSuCo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhong = table.Column<int>(type: "int", nullable: false),
                    MaTK = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ThoiDiemBaoCao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUCOPHONG", x => x.MaSuCo);
                    table.ForeignKey(
                        name: "FK_SUCOPHONG_PHONG_MaPhong",
                        column: x => x.MaPhong,
                        principalTable: "PHONG",
                        principalColumn: "MaPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SUCOPHONG_TAIKHOAN_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HOADON",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatPhong = table.Column<int>(type: "int", nullable: false),
                    MaTK = table.Column<int>(type: "int", nullable: false),
                    TienPhong = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrangThaiHoaDon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HOADON", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HOADON_DATPHONG_MaDatPhong",
                        column: x => x.MaDatPhong,
                        principalTable: "DATPHONG",
                        principalColumn: "MaDatPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HOADON_TAIKHOAN_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SUDUNGDICHVU",
                columns: table => new
                {
                    MaSDDV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatPhong = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    MaTK = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaLuu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiDiemSuDung = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUDUNGDICHVU", x => x.MaSDDV);
                    table.ForeignKey(
                        name: "FK_SUDUNGDICHVU_DATPHONG_MaDatPhong",
                        column: x => x.MaDatPhong,
                        principalTable: "DATPHONG",
                        principalColumn: "MaDatPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SUDUNGDICHVU_DICHVU_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DICHVU",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SUDUNGDICHVU_TAIKHOAN_MaTK",
                        column: x => x.MaTK,
                        principalTable: "TAIKHOAN",
                        principalColumn: "MaTK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GIAODICH",
                columns: table => new
                {
                    MaGD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatPhong = table.Column<int>(type: "int", nullable: false),
                    MaHoaDon = table.Column<int>(type: "int", nullable: true),
                    CongThanhToan = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    MaGDCong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LoaiGD = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DaDoiSoat = table.Column<bool>(type: "bit", nullable: false),
                    ThoiDiem = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GIAODICH", x => x.MaGD);
                    table.ForeignKey(
                        name: "FK_GIAODICH_DATPHONG_MaDatPhong",
                        column: x => x.MaDatPhong,
                        principalTable: "DATPHONG",
                        principalColumn: "MaDatPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GIAODICH_HOADON_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HOADON",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DICHVU",
                columns: new[] { "MaDichVu", "MoTa", "TenDichVu", "DonGia", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Tính theo bộ đồ", "Giặt là", 50000m, "Active" },
                    { 2, "Tính theo món", "Minibar", 30000m, "Active" },
                    { 3, "Mỗi suất", "Ăn sáng tại phòng", 100000m, "Active" },
                    { 4, "Mỗi chuyến", "Đưa đón sân bay", 250000m, "Active" }
                });

            migrationBuilder.InsertData(
                table: "KHACHHANG",
                columns: new[] { "MaKH", "NgayTao", "Email", "HoTen", "CCCD_HoChieu", "MatKhau", "SoDienThoai" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "vana@example.com", "Nguyen Van A", "079000000001", "CHUA_DAT_MAT_KHAU", "0901000001" },
                    { 2, new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "thib@example.com", "Tran Thi B", "079000000002", "CHUA_DAT_MAT_KHAU", "0901000002" }
                });

            migrationBuilder.InsertData(
                table: "LOAIPHONG",
                columns: new[] { "MaLoaiPhong", "TienNghi", "GiaCoBan", "MoTa", "TenLoaiPhong", "TrangThai" },
                values: new object[,]
                {
                    { 1, "TV, điều hòa, wifi", 500000m, "Phòng tiêu chuẩn", "Standard", "Active" },
                    { 2, "TV, điều hòa, wifi, ban công", 900000m, "Phòng cao cấp", "Deluxe", "Active" },
                    { 3, "TV, điều hòa, wifi, bồn tắm", 1800000m, "Phòng hạng sang", "Suite", "Active" }
                });

            migrationBuilder.InsertData(
                table: "NHANVIEN",
                columns: new[] { "MaNV", "Email", "HoTen", "NgayVaoLam", "CCCD", "MatKhau", "SoDienThoai" },
                values: new object[,]
                {
                    { 1, "levanc@hotel.local", "Le Van C", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "079100000001", "CHUA_DAT_MAT_KHAU", "0902000001" },
                    { 2, "phamthid@hotel.local", "Pham Thi D", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "079100000002", "CHUA_DAT_MAT_KHAU", "0902000002" }
                });

            migrationBuilder.InsertData(
                table: "TAIKHOAN",
                columns: new[] { "MaTK", "Email", "HoTen", "MatKhau", "SoDienThoai", "VaiTro", "TrangThai" },
                values: new object[,]
                {
                    { 1, "admin@hotel.local", "Quản trị viên", "CHUA_DAT_MAT_KHAU", "0900000001", "Admin", "Active" },
                    { 2, "letan01@hotel.local", "Lễ tân 01", "CHUA_DAT_MAT_KHAU", "0900000002", "Receptionist", "Active" }
                });

            migrationBuilder.InsertData(
                table: "GIAMUAVU",
                columns: new[] { "MaGia", "GiaApDung", "NgayKetThuc", "TenGiaiDoan", "MaLoaiPhong", "NgayBatDau" },
                values: new object[,]
                {
                    { 1, 700000m, new DateOnly(2026, 12, 31), "Noel 2026", 1, new DateOnly(2026, 12, 20) },
                    { 2, 1200000m, new DateOnly(2026, 12, 31), "Noel 2026", 2, new DateOnly(2026, 12, 20) },
                    { 3, 2400000m, new DateOnly(2026, 12, 31), "Noel 2026", 3, new DateOnly(2026, 12, 20) },
                    { 4, 800000m, new DateOnly(2027, 2, 14), "Tết 2027", 1, new DateOnly(2027, 2, 5) },
                    { 5, 1400000m, new DateOnly(2027, 2, 14), "Tết 2027", 2, new DateOnly(2027, 2, 5) },
                    { 6, 2800000m, new DateOnly(2027, 2, 14), "Tết 2027", 3, new DateOnly(2027, 2, 5) }
                });

            migrationBuilder.InsertData(
                table: "PHONG",
                columns: new[] { "MaPhong", "SoTang", "SoPhong", "MaLoaiPhong", "TrangThaiPhong" },
                values: new object[,]
                {
                    { 1, 1, "101", 1, "Available" },
                    { 2, 1, "102", 1, "Available" },
                    { 3, 2, "201", 2, "Available" },
                    { 4, 3, "301", 3, "Maintenance" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DATPHONG_MaKH",
                table: "DATPHONG",
                column: "MaKH");

            migrationBuilder.CreateIndex(
                name: "IX_DATPHONG_MaPhong_NgayNhan_NgayTra",
                table: "DATPHONG",
                columns: new[] { "MaPhong", "NgayNhan", "NgayTra" });

            migrationBuilder.CreateIndex(
                name: "IX_DATPHONG_MaTK",
                table: "DATPHONG",
                column: "MaTK");

            migrationBuilder.CreateIndex(
                name: "IX_GIAMUAVU_MaLoaiPhong",
                table: "GIAMUAVU",
                column: "MaLoaiPhong");

            migrationBuilder.CreateIndex(
                name: "IX_GIAODICH_MaDatPhong",
                table: "GIAODICH",
                column: "MaDatPhong");

            migrationBuilder.CreateIndex(
                name: "IX_GIAODICH_MaHoaDon",
                table: "GIAODICH",
                column: "MaHoaDon");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaDatPhong",
                table: "HOADON",
                column: "MaDatPhong");

            migrationBuilder.CreateIndex(
                name: "IX_HOADON_MaTK",
                table: "HOADON",
                column: "MaTK");

            migrationBuilder.CreateIndex(
                name: "IX_KHACHHANG_Email",
                table: "KHACHHANG",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LOAIPHONG_TenLoaiPhong",
                table: "LOAIPHONG",
                column: "TenLoaiPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NHANVIEN_Email",
                table: "NHANVIEN",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_HOAT_DONG_ma_tk",
                table: "NHAT_KY_HOAT_DONG",
                column: "ma_tk");

            migrationBuilder.CreateIndex(
                name: "IX_PHONG_MaLoaiPhong",
                table: "PHONG",
                column: "MaLoaiPhong");

            migrationBuilder.CreateIndex(
                name: "IX_PHONG_SoPhong",
                table: "PHONG",
                column: "SoPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SUCOPHONG_MaPhong",
                table: "SUCOPHONG",
                column: "MaPhong");

            migrationBuilder.CreateIndex(
                name: "IX_SUCOPHONG_MaTK",
                table: "SUCOPHONG",
                column: "MaTK");

            migrationBuilder.CreateIndex(
                name: "IX_SUDUNGDICHVU_MaDatPhong",
                table: "SUDUNGDICHVU",
                column: "MaDatPhong");

            migrationBuilder.CreateIndex(
                name: "IX_SUDUNGDICHVU_MaDichVu",
                table: "SUDUNGDICHVU",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_SUDUNGDICHVU_MaTK",
                table: "SUDUNGDICHVU",
                column: "MaTK");

            migrationBuilder.CreateIndex(
                name: "IX_TAIKHOAN_Email",
                table: "TAIKHOAN",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GIAMUAVU");

            migrationBuilder.DropTable(
                name: "GIAODICH");

            migrationBuilder.DropTable(
                name: "NHANVIEN");

            migrationBuilder.DropTable(
                name: "NHAT_KY_HOAT_DONG");

            migrationBuilder.DropTable(
                name: "SUCOPHONG");

            migrationBuilder.DropTable(
                name: "SUDUNGDICHVU");

            migrationBuilder.DropTable(
                name: "HOADON");

            migrationBuilder.DropTable(
                name: "DICHVU");

            migrationBuilder.DropTable(
                name: "DATPHONG");

            migrationBuilder.DropTable(
                name: "KHACHHANG");

            migrationBuilder.DropTable(
                name: "PHONG");

            migrationBuilder.DropTable(
                name: "TAIKHOAN");

            migrationBuilder.DropTable(
                name: "LOAIPHONG");
        }
    }
}
