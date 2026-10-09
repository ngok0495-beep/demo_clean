using Domain;
namespace Application;

public interface IRoomRepository
{
    /// <summary>Trả phòng kèm loại phòng và bảng giá mùa vụ.</summary>
    Task<IReadOnlyList<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
}

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
}

public interface IBookingRepository
{
    Task<IReadOnlyList<Booking>> GetByRoomAsync(int roomId);
    Task<Booking?> GetByIdAsync(int id);
    Task AddAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}