using Application;
namespace Api.Presenters;

public record RoomViewModel(int Id, string Number, string Type, string Price);

public record BookingViewModel(int Id, int RoomId, int CustomerId, string Guest,
                               string CheckIn, string CheckOut, int GuestCount,
                               string Total, string Status)
{
    public static BookingViewModel From(BookingDto b) =>
        new(b.Id, b.RoomId, b.CustomerId, b.CustomerName,
            b.CheckIn.ToString("dd/MM/yyyy"), b.CheckOut.ToString("dd/MM/yyyy"),
            b.GuestCount, $"{b.TotalPrice:N0} VND", b.Status);
}