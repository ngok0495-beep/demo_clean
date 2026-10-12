using Domain;
namespace Application;

public record RoomDetailsDto(int Id, string Number, int Floor, int RoomTypeId, string RoomTypeName, RoomStatus Status);
public record RoomTypeOptionDto(int Id, string Name, decimal BasePrice);
public record SaveRoomRequest(string Number, int Floor, int RoomTypeId, RoomStatus Status);
public record ChangeRoomStatusRequest(RoomStatus Status);

// Separate from the search gateway so existing search/booking contracts remain intact.
public interface IRoomManagementRepository
{
    Task<IReadOnlyList<Room>> ListAsync(RoomStatus? status, int? roomTypeId);
    Task<Room?> FindAsync(int id);
    Task<IReadOnlyList<RoomType>> ListTypesAsync();
    Task<bool> TypeExistsAsync(int id);
    Task<bool> NumberExistsAsync(string number, int? exceptId);
    Task<bool> HasHistoryAsync(int id);
    Task AddAsync(Room room);
    Task SaveAsync();
    Task DeleteAsync(Room room);
}

public interface IManageRoomsInputBoundary
{
    Task ListAsync(RoomStatus? status, int? roomTypeId);
    Task GetAsync(int id);
    Task ListTypesAsync();
    Task CreateAsync(SaveRoomRequest request);
    Task UpdateAsync(int id, SaveRoomRequest request);
    Task ChangeStatusAsync(int id, ChangeRoomStatusRequest request);
    Task DeleteAsync(int id);
}

public interface IManageRoomsOutputBoundary
{
    void PresentRooms(IReadOnlyList<RoomDetailsDto> rooms);
    void PresentRoom(RoomDetailsDto room, bool created = false);
    void PresentTypes(IReadOnlyList<RoomTypeOptionDto> types);
    void PresentDeleted();
    void PresentError(ErrorKind kind, string message);
}

public class ManageRoomsUseCase(IRoomManagementRepository rooms, IManageRoomsOutputBoundary output)
    : IManageRoomsInputBoundary
{
    private static RoomDetailsDto Map(Room r) => new(r.Id, r.Number, r.Floor, r.RoomTypeId, r.RoomType.Name, r.Status);

    private async Task ExecuteAsync(Func<Task> action)
    {
        try { await action(); }
        catch (AppException ex) { output.PresentError(ex.Kind, ex.Message); }
    }

    private static void ValidateStatus(RoomStatus status)
    {
        if (!Enum.IsDefined(status))
            throw new AppException(ErrorKind.Validation, "Trạng thái phòng không hợp lệ.");
    }

    private async Task<string> ValidateAsync(SaveRoomRequest request, int? id = null)
    {
        var number = request.Number?.Trim() ?? "";
        if (number.Length is < 1 or > 10)
            throw new AppException(ErrorKind.Validation, "Số phòng phải có từ 1 đến 10 ký tự.");
        if (request.Floor < 0)
            throw new AppException(ErrorKind.Validation, "Tầng phải lớn hơn hoặc bằng 0.");
        ValidateStatus(request.Status);
        if (!await rooms.TypeExistsAsync(request.RoomTypeId))
            throw new AppException(ErrorKind.Validation, "Loại phòng không tồn tại.");
        if (await rooms.NumberExistsAsync(number, id))
            throw new AppException(ErrorKind.Conflict, "Số phòng đã tồn tại.");
        return number;
    }

    private async Task<Room> FindAsync(int id) => await rooms.FindAsync(id)
        ?? throw new AppException(ErrorKind.NotFound, "Không tìm thấy phòng.");

    public Task ListAsync(RoomStatus? status, int? roomTypeId) => ExecuteAsync(async () =>
    {
        if (status.HasValue) ValidateStatus(status.Value);
        if (roomTypeId.HasValue && roomTypeId.Value <= 0)
            throw new AppException(ErrorKind.Validation, "Mã loại phòng phải lớn hơn 0.");
        output.PresentRooms((await rooms.ListAsync(status, roomTypeId)).Select(Map).ToList());
    });

    public Task GetAsync(int id) => ExecuteAsync(async () => output.PresentRoom(Map(await FindAsync(id))));

    public Task ListTypesAsync() => ExecuteAsync(async () => output.PresentTypes(
        (await rooms.ListTypesAsync()).Select(t => new RoomTypeOptionDto(t.Id, t.Name, t.BasePrice)).ToList()));

    public Task CreateAsync(SaveRoomRequest request) => ExecuteAsync(async () =>
    {
        var number = await ValidateAsync(request);
        var room = new Room { Number = number, Floor = request.Floor, RoomTypeId = request.RoomTypeId, Status = request.Status };
        await rooms.AddAsync(room);
        output.PresentRoom(Map(await FindAsync(room.Id)), created: true);
    });

    public Task UpdateAsync(int id, SaveRoomRequest request) => ExecuteAsync(async () =>
    {
        var room = await FindAsync(id);
        var number = await ValidateAsync(request, id);
        room.Number = number;
        room.Floor = request.Floor;
        room.RoomTypeId = request.RoomTypeId;
        room.Status = request.Status;
        await rooms.SaveAsync();
        output.PresentRoom(Map(await FindAsync(id)));
    });

    public Task ChangeStatusAsync(int id, ChangeRoomStatusRequest request) => ExecuteAsync(async () =>
    {
        var room = await FindAsync(id);
        ValidateStatus(request.Status);
        room.Status = request.Status;
        await rooms.SaveAsync();
        output.PresentRoom(Map(room));
    });

    public Task DeleteAsync(int id) => ExecuteAsync(async () =>
    {
        var room = await FindAsync(id);
        if (room.Status == RoomStatus.Occupied || await rooms.HasHistoryAsync(id))
            throw new AppException(ErrorKind.Conflict, "Không thể xóa phòng đang sử dụng hoặc đã có lịch sử đặt phòng/sự cố.");
        await rooms.DeleteAsync(room);
        output.PresentDeleted();
    });
}
