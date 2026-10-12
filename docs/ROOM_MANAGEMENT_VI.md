# Room Management — hướng dẫn làm và demo trên Windows

## 1. Repo đã được chuẩn bị

- Repo gốc: https://github.com/ngok0495-beep/demo_clean
- Nhánh cá nhân: `feature/room-management`, phát triển từ commit nhóm `7f420a8`.
- Solution: `HotelCleanDemo.sln`, ứng dụng ASP.NET Core `net9.0`.
- Chưa push hoặc merge vào `main`. Không sửa các file tham khảo trong `sources/` của project ChatGPT.
- Database riêng mặc định: `HotelCleanDemo_RoomManagement` trên SQL Server LocalDB.

Nếu muốn tự clone từ đầu vào thư mục khác, mở PowerShell tại thư mục muốn lưu:

```powershell
git clone https://github.com/ngok0495-beep/demo_clean.git
cd demo_clean
git switch -c feature/room-management
```

Lưu ý: clone lại repo gốc lúc chưa push sẽ chỉ có mã nhóm, chưa có phần mới. Bản đã triển khai nằm trong thư mục `demo_clean` đi kèm hướng dẫn này.

## 2. Hiểu cấu trúc để trình bày khi chấm

| Tầng | Vai trò | File liên quan |
|---|---|---|
| Domain | Entity phòng, loại phòng và enum trạng thái | `src/Domain/Rooms.cs` (mã nhóm) |
| Application | DTO, interface repository, input/output boundary và nghiệp vụ | `src/Application/Rooms/ManageRooms.cs` |
| Infrastructure | Truy cập SQL Server qua EF Core, xử lý ràng buộc database | `src/Infrastructure/Persistence/EfRoomManagementRepository.cs` |
| Api | Nhận HTTP, trả mã trạng thái qua Presenter | `src/Api/Controllers/RoomManagementController.cs`, `src/Api/Presenters/ManageRoomsPresenter.cs` |

Luồng một yêu cầu: **Controller → Use Case → Repository interface → EF Repository → SQL Server**, sau đó **Use Case → Presenter → HTTP response**. Controller không truy vấn database trực tiếp. Application không phụ thuộc EF Core hoặc ASP.NET Core.

Repo ban đầu đã có entity, ánh xạ database và EF repository, nhưng `Program.cs` vẫn đăng ký InMemory repository; chưa có migrations và Swagger. Bản này nối ứng dụng vào `AddInfrastructure`, thêm migration ban đầu và Swagger. Các file nghiệp vụ booking/search cũ được giữ nguyên. Room Management dùng interface riêng để không phải sửa các implementation InMemory cũ.

## 3. Chạy bằng SQL Server trên máy này

Mở PowerShell tại thư mục có `HotelCleanDemo.sln`. Máy này dùng SQL Server `.\SERVER1`, database `HotelCleanDemo_RoomManagement`, API ở **http://localhost:5227/swagger**. Connection string đã lưu trong user-secrets của `src/Api`; không chép mật khẩu vào appsettings, tài liệu hoặc Git. `UserSecretsId` trong project chỉ là định danh cấu hình.

```powershell
git branch --show-current
dotnet restore HotelCleanDemo.sln
dotnet tool restore
dotnet build HotelCleanDemo.sln -m:1
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef migrations list --project src/Infrastructure --startup-project src/Api
dotnet run --project src/Api --launch-profile http --urls http://localhost:5227
```

Migration `20261010045314_InitialHotelSchema` đã được xác nhận áp dụng trên database này; không cần tạo migration hoặc cập nhật lại để demo. Chỉ dùng `dotnet ef database update --project src/Infrastructure --startup-project src/Api` khi thiết lập một database cá nhân mới. Không áp dụng migration ban đầu vào database nhóm đã có bảng nhưng chưa có lịch sử migrations.

Mặc định appsettings Development dùng LocalDB để người khác có thể cấu hình riêng. User-secrets trên máy này ghi đè mặc định. Nếu gặp SQL error 26 với tên instance, kiểm tra SQL Server Configuration Manager và cổng TCP. Khi kiểm tra trong agent, TCP `127.0.0.1,1433` kết nối được đúng SERVER1; không suy ra các máy khác dùng cùng cổng. Không chạy lệnh liệt kê user-secrets khi quay màn hình demo.

Dữ liệu mẫu: loại phòng 1 Standard / 2 Deluxe / 3 Suite; phòng 101, 102, 201, 301; khách hàng 1 và 2. Phòng 301 đang bảo trì. Bảng phòng là `PHONG`, bảng loại phòng là `LOAIPHONG`.

## 4. API hiện có và API của phần cá nhân

| Method và đường dẫn | Chức năng | Nguồn |
|---|---|---|
| `GET /api/rooms?from=2026-12-01&to=2026-12-03&type=Standard` | Tìm phòng, không hiển thị phòng bảo trì | Nhóm |
| `POST /api/bookings` | Tạo đặt phòng | Nhóm |
| `GET /api/bookings/{id}` | Xem đặt phòng | Nhóm |
| `DELETE /api/bookings/{id}` | Hủy đặt phòng, giữ lịch sử | Nhóm |
| `GET /api/rooms/management` | Xem tất cả phòng, bao gồm bảo trì; lọc `status`, `roomTypeId` | Cá nhân |
| `GET /api/rooms/types` | Tra danh sách loại phòng để chọn khi thêm/sửa | Cá nhân |
| `GET /api/rooms/{id}` | Xem chi tiết phòng | Cá nhân |
| `POST /api/rooms` | Thêm phòng | Cá nhân |
| `PUT /api/rooms/{id}` | Cập nhật đầy đủ thông tin phòng | Cá nhân |
| `PATCH /api/rooms/{id}/status` | Cập nhật riêng trạng thái | Cá nhân |
| `DELETE /api/rooms/{id}` | Xóa phòng chưa có lịch sử và không đang sử dụng | Cá nhân |

Đường dẫn `/management` tách danh sách quản trị khỏi API tìm phòng sẵn có để bảo toàn kết quả mà phần nhóm đang sử dụng.

Giá được quản lý theo **loại phòng**, không phải một trường giá riêng trên phòng. Giữ đúng mô hình nhóm: `RoomStatus` gồm `0 = Available` (trống), `1 = Occupied` (đang sử dụng), `2 = Cleaning` (đang dọn), `3 = Maintenance` (bảo trì). JSON dùng số cho enum; các trường trong body mới đều bắt buộc. Sơ đồ tham khảo có trạng thái vệ sinh riêng, còn code nhóm đang dùng một enum chung; bản này không tự thay schema đó.

## 5. Kịch bản Swagger khoảng 5–7 phút

Trong từng mục Swagger, bấm **Try it out**, điền dữ liệu rồi **Execute**.

1. `GET /api/rooms/types`: nhận `200`, có ba loại phòng.
2. `GET /api/rooms/management`: nhận `200`, thấy cả phòng 301 đang bảo trì.
3. `POST /api/rooms` với body sau: nhận `201 Created`. Ghi lại `id` và header `Location`.

```json
{
  "number": "501",
  "floor": 5,
  "roomTypeId": 1,
  "status": 0
}
```

4. Gửi lại body trên: nhận `409 Conflict` vì số phòng trùng.
5. `GET /api/rooms/{id}` với ID vừa tạo: nhận `200`, thông tin phòng 501.
6. `PUT /api/rooms/{id}` với body dưới: nhận `200`, tên loại phòng đổi thành Deluxe.

```json
{
  "number": "502",
  "floor": 5,
  "roomTypeId": 2,
  "status": 2
}
```

7. `PATCH /api/rooms/{id}/status` với `{"status":3}`: nhận `200`. Phòng còn trong `/management` nhưng bị loại khỏi `GET /api/rooms`.
8. Dừng API bằng Ctrl+C, chạy lại. `GET /api/rooms/{id}` vẫn còn dữ liệu: minh họa dữ liệu lưu SQL Server, không mất như RAM.
9. `DELETE /api/rooms/{id}`: nhận `204 No Content`. GET lại nhận `404 Not Found`.
10. Thử thêm với `floor: -1`, `roomTypeId: 99999`, hoặc `status: 99`: nhận `400 Bad Request` kèm ProblemDetails. PATCH `{}` cũng trả `400`.

Để demo bảo vệ lịch sử: tạo một phòng mới; tạo booking cho phòng đó qua API nhóm với body gồm `customerId: 1`, `roomId` vừa tạo, `checkIn`, `checkOut` (ngày trả sau ngày nhận), `guestCount: 1`. Xóa phòng sẽ nhận `409`; hủy booking cũng vẫn giữ lịch sử nên phòng tiếp tục không được xóa. Không dùng dữ liệu thật của nhóm để thử.

Quy tắc kiểm tra: số phòng sau khi bỏ khoảng trắng đầu/cuối dài 1–10 ký tự, tầng ≥ 0, loại phòng tồn tại, trạng thái thuộc enum, số phòng duy nhất. Unique index và khóa ngoại SQL Server còn bảo vệ khi có nhiều request cùng lúc. PATCH cập nhật trạng thái vận hành thủ công, chưa triển khai quy trình check-in/check-out hoặc tự đồng bộ trạng thái với booking.

## 6. Kiểm thử và giới hạn

```powershell
dotnet test HotelCleanDemo.sln
```

Các integration test dùng HTTP pipeline thật trong TestServer và EF Core SQLite riêng cho từng ca; không dùng hoặc xóa database SQL Server của bạn. Bộ test kiểm tra CRUD qua nhiều request, đổi loại phòng, lọc danh sách, mã lỗi, bảo vệ lịch sử, các route booking cũ, Swagger UI và OpenAPI JSON.

Kết quả kiểm tra thực tế được ghi trong `docs/VERIFICATION.md`. SQLite test không thay thế việc thử migration và CRUD trên SQL Server. Kiểm thử HTTP và kiểm tra SQL Server đã hoàn tất ngày 11/10/2026; xem số liệu và giới hạn trong báo cáo.

Repo chưa có đăng nhập/phân quyền. API quản lý hiện phục vụ demo local; phân quyền quản trị và xử lý trạng thái đồng thời với check-in/check-out cần thống nhất với nhóm nếu phát triển tiếp.

## 7. Commit và chia sẻ mà không ghi đè nhóm

Phần triển khai đã được commit trên branch riêng. Để xem:

```powershell
git status
git log -1 --oneline
git diff main...HEAD --stat
```

Nếu bạn sửa thêm, kiểm tra diff và chỉ thêm đúng file của mình trước khi commit. Khi muốn đưa branch cá nhân lên GitHub:

```powershell
git push -u origin feature/room-management
```

Lệnh này chỉ đẩy branch cá nhân. Sau đó tạo Pull Request để nhóm xem; không push trực tiếp vào `main` và không force-push. Nếu GitHub báo 403, cần chủ repo cấp quyền collaborator hoặc dùng fork của bạn.

## 8. Lỗi hay gặp

- Không tìm thấy project: đang mở sai thư mục; chuyển đến thư mục có `HotelCleanDemo.sln`.
- Không tìm thấy `dotnet ef`: chạy `dotnet tool restore` tại gốc repo.
- Không kết nối LocalDB: chạy `sqllocaldb info` rồi `sqllocaldb start MSSQLLocalDB` từ PowerShell bình thường của bạn; kiểm tra tên instance. Nếu dùng Express, sửa biến kết nối như mục 3.
- `Invalid object name PHONG`: chưa chạy `dotnet ef database update`, hoặc API đang trỏ nhầm database.
- Swagger 404: dùng profile `http` với môi trường Development.
- Cổng 5227 bận: dừng phiên API cũ hoặc thêm `--urls http://localhost:5227` vào lệnh chạy rồi mở đúng cổng.
- Demo báo 409 khi thêm phòng 501: đã có phòng này từ lần thử trước; chọn số khác hoặc xóa phòng demo nếu chưa có lịch sử.

Tài liệu chính thức: [EF Core migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/), [EF CLI](https://learn.microsoft.com/en-us/ef/core/cli/dotnet), [Swagger/OpenAPI](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger).
