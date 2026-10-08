using Domain;
namespace Application;

public interface IRoomRepository
{
    Task<IReadOnlyList<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
}

public interface IBookingRepository
{
    Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId);
    Task<Booking?> GetByIdAsync(int id);   // thêm dòng này
    Task AddAsync(Booking booking);
}

