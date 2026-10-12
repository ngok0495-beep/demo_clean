# Báo cáo kiểm thử Room Management — 11/10/2026

Repo `ngok0495-beep/demo_clean`, branch `feature/room-management`, base `main`.

## Kết quả từng hạng mục

| Hạng mục | Kết quả thực tế |
|---|---|
| Build toàn solution | Đạt, 0 lỗi / 0 cảnh báo với `dotnet build HotelCleanDemo.sln --no-restore -m:1` |
| Automated tests | Đạt **23/23**, không bỏ qua; HTTP TestServer + EF Core SQLite, database riêng từng test |
| HTTP thực tế | Đạt **46 request / 87 kiểm tra** tại `http://localhost:5227`; chạy bằng `scripts/Test-RoomManagement.ps1` |
| 6 Room Management APIs | GET danh sách 200; GET chi tiết 200; POST 201 + Location; PUT 200; PATCH 200; DELETE 204, GET lại 404 |
| Validation 400 | Số phòng rỗng/khoảng trắng/quá 10 ký tự, tầng âm, loại phòng không tồn tại, enum sai, JSON lỗi, thiếu trường, PATCH null, query sai |
| Conflict 409 | Tạo/sửa trùng số phòng; xóa phòng Occupied |
| Not found 404 | GET/PUT/PATCH/DELETE ID không tồn tại, DELETE lặp lại |
| Dữ liệu sau lỗi | PUT/PATCH bị từ chối không làm thay đổi các trường của phòng |
| Bộ lọc và tương thích API nhóm | Lọc kết hợp status + roomTypeId chính xác; API tìm phòng cũ loại phòng Maintenance |
| Bảo vệ lịch sử | Automated tests kiểm tra booking kể cả sau khi hủy, incident, và phòng Occupied; không thêm booking/incident thử vào SQL thật |
| Swagger | UI và OpenAPI JSON trả 200; mô tả đủ 6 thao tác |
| Lưu bền SQL Server | Tạo phòng qua tiến trình API 5227, đọc cùng phòng từ tiến trình API độc lập 5228, xóa thành công 204 |
| Dọn dữ liệu thử | Sau kịch bản HTTP, toàn bộ thông tin 4 phòng ban đầu giữ nguyên; phòng thử lưu bền cũng đã xóa |
| Migration đã áp dụng | Truy vấn trực tiếp `__EFMigrationsHistory` và EF CLI xác nhận `20261010045314_InitialHotelSchema` trong `HotelCleanDemo_RoomManagement` trên instance `SERVER1` |
| Migration/model | `has-pending-model-changes` báo không có thay đổi; xuất thành công SQL idempotent; không cần migration mới |
| Git/secret | `git diff --check` đạt; quét 51 file hiện tại + 77 blob lịch sử không phát hiện mẫu credential hoặc literal mật khẩu cấu hình. Đây là kiểm tra theo mẫu, không phải chứng nhận không có mọi loại secret. |

## Rà soát Clean Architecture

- `Domain` không tham chiếu framework hoặc project ngoài; `Application` chỉ tham chiếu `Domain`.
- Input/output boundary và repository interface thuộc Application; EF Core/SQL Server nằm trong Infrastructure; controller/presenter thuộc Api.
- `Program.cs` đóng vai trò composition root, đăng ký presenter cùng instance scoped cho controller và use case.
- Controller chuyển request sang use case; validation, duplicate check và quy tắc xóa nằm trong use case. Infrastructure chuyển lỗi unique/FK/concurrency SQL thành lỗi ứng dụng.
- Không phát hiện lỗi nghiệp vụ cần thay đổi trong phạm vi các ca đã kiểm tra. Bổ sung 11 ca hồi quy cho PUT/PATCH lỗi và lọc kết hợp; giữ hợp đồng API nhóm.
- Giữ `UserSecretsId` đã có trên máy để ứng dụng đọc cấu hình riêng. ID này không phải mật khẩu. Bổ sung ignore cho `.env.*`, `secrets.json`, `appsettings.Local.json`.

## Lưu ý môi trường và giới hạn đã xác định

Kết nối theo tên instance bị SQL error 26 trong môi trường agent. Kết nối tạm bằng `tcp:127.0.0.1,1433` thành công và truy vấn server xác nhận đúng instance `SERVER1`. Chỉ override biến môi trường cho tiến trình kiểm tra, không sửa user-secrets, không ghi mật khẩu vào repo. API 5227 đang chạy của người dùng được giữ nguyên.

Chưa có đăng nhập/phân quyền; chỉ dùng demo local. Không có row-version hoặc cơ chế chống ghi đè khi hai người cùng sửa; chưa kiểm thử tải hay đua đồng thời giữa thao tác đổi trạng thái và xóa. PATCH là thao tác vận hành thủ công, chưa đồng bộ check-in/check-out. Chưa dừng/khởi động lại tiến trình 5227 của người dùng; đã kiểm tra lưu bền bằng tiến trình độc lập và cùng SQL database.

## Chạy lại

Từ thư mục chứa `HotelCleanDemo.sln`, khi API đang chạy:

```powershell
dotnet test HotelCleanDemo.sln --no-restore -m:1 --logger "trx;LogFileName=room-management.trx"
./scripts/Test-RoomManagement.ps1 -BaseUrl http://localhost:5227
```

Kịch bản HTTP cần ít nhất một phòng và hai loại phòng để kiểm tra dữ liệu trùng/lọc. Nó chỉ sửa/xóa phòng ngẫu nhiên tự tạo trong lần chạy; luôn cố dọn dữ liệu bằng `finally`. Nếu bị tắt tiến trình đột ngột, kiểm tra phòng thử còn lại trước lần demo.

Bằng chứng local được bỏ qua bởi Git: `tests/Api.Tests/TestResults/room-management.trx`, `test-results/http-room-management.json`, `test-results/persistence.json`, `test-results/migrations.sql`, `test-results/secret-audit.json`.

## GitHub

Đọc remote/fetch thành công. Credential helper bị môi trường thực thi chặn truy cập Windows Credential Manager (`Access is denied`). Không đưa credential vào file để khắc phục.

Đã commit toàn bộ thay đổi local và thử `git push -u origin feature/room-management`, nhưng Git báo không đọc được Username trong phiên không tương tác. **Chưa push thành công, chưa tạo PR, chưa merge main.** Nội dung PR sẵn trong `docs/PR_BODY.md`. Đang chờ kết nối GitHub được người dùng xác nhận để tiếp tục.
