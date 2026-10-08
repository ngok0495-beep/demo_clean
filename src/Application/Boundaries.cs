namespace Application;

public interface ISearchAvailableRoomsInputBoundary { Task ExecuteAsync(SearchRoomsQuery query); }
public interface ISearchAvailableRoomsOutputBoundary
{
    void PresentRooms(IReadOnlyList<RoomDto> rooms);
    void PresentError(ErrorKind kind, string message);
}

public interface ICreateBookingInputBoundary { Task ExecuteAsync(CreateBookingRequest request); }
public interface ICreateBookingOutputBoundary
{
    void PresentSuccess(BookingDto booking);
    void PresentError(ErrorKind kind, string message);
}

public interface IGetBookingInputBoundary { Task ExecuteAsync(int bookingId); }
public interface IGetBookingOutputBoundary
{
    void PresentBooking(BookingDto booking);
    void PresentError(ErrorKind kind, string message);
}

public interface ICancelBookingInputBoundary { Task ExecuteAsync(int bookingId); }
public interface ICancelBookingOutputBoundary
{
    void PresentCancelled(CancelResultDto result);
    void PresentError(ErrorKind kind, string message);
}