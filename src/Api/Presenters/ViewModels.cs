using Application;
namespace Api.Presenters;

public record RoomViewModel(int Id, string Number, string Type, string Price);

public record BookingViewModel(int Id, int RoomId, string Guest, string CheckIn,
                               string CheckOut, string Total, string Status)
{
    public static BookingViewModel From(BookingDto b) =>
        new(b.Id, b.RoomId, b.GuestName, b.CheckIn.ToString("dd/MM/yyyy"),
            b.CheckOut.ToString("dd/MM/yyyy"), $"{b.TotalPrice:N0} VND", b.Status);
}