namespace Application;

public interface ISearchAvailableRoomsInputBoundary
{
    Task ExecuteAsync(SearchRoomsQuery query);
}
public interface ISearchAvailableRoomsOutputBoundary
{
    void PresentRooms(IReadOnlyList<RoomDto> rooms);
    void PresentError(string message);
}

public interface ICreateBookingInputBoundary
{
    Task ExecuteAsync(CreateBookingRequest request);
}
public interface ICreateBookingOutputBoundary
{
    void PresentSuccess(BookingDto booking);
    void PresentError(string message);
}