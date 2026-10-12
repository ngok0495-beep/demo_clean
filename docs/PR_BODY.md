## Room Management API

Thêm chức năng quản lý phòng lưu trên EF Core/SQL Server: danh sách có lọc, xem chi tiết, tạo, cập nhật, đổi trạng thái và xóa. Danh sách quản trị hiển thị cả phòng bảo trì; API tìm phòng hiện có của nhóm vẫn giữ hợp đồng cũ.

Use case kiểm tra dữ liệu, số phòng trùng và bảo vệ phòng đang sử dụng hoặc có lịch sử đặt phòng/sự cố. Controller/presenter tách khỏi nghiệp vụ qua input/output boundary. Thêm Swagger, migration ban đầu, tra cứu loại phòng, cấu hình user-secrets, tài liệu demo và kịch bản HTTP tự dọn dữ liệu thử.

## Validation

- Build: 0 lỗi, 0 cảnh báo.
- Automated integration tests: 23/23 đạt trên TestServer + EF Core SQLite.
- API đang chạy tại localhost:5227: 46 request / 87 kiểm tra đạt, gồm đủ 6 API, 400/404/409, dữ liệu không đổi sau lỗi, bộ lọc, Swagger và API tìm phòng cũ.
- SQL Server SERVER1 / HotelCleanDemo_RoomManagement: xác nhận migration đã áp dụng, model không có thay đổi chưa được migrate, xuất SQL idempotent thành công.
- Phòng tạo từ một tiến trình API đọc được từ tiến trình API thứ hai; đã xóa phòng thử, 4 phòng gốc giữ nguyên.

## Giới hạn

Phục vụ demo local, chưa có đăng nhập/phân quyền hoặc chống ghi đè đồng thời. Bảo vệ booking/incident được kiểm tra bằng automated tests; không tạo lịch sử thử trong SQL thật. Không áp dụng migration ban đầu vào database nhóm có sẵn bảng nhưng chưa có lịch sử migrations. Xem `docs/VERIFICATION.md` và `docs/ROOM_MANAGEMENT_VI.md`.
