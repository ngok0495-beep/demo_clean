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
    Task AddAsync(Booking booking);
}