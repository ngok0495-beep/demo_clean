using Domain;
namespace Application;

public record BookingDto(int Id, int RoomId, string GuestName, DateOnly CheckIn,
                         DateOnly CheckOut, decimal TotalPrice, string Status);
public record CancelResultDto(int BookingId, string Status, decimal RefundAmount);

internal static class BookingMapper
{
    public static decimal TotalPrice(Booking b, Room r) =>
        (b.CheckOut.DayNumber - b.CheckIn.DayNumber) * r.PricePerNight;

    public static BookingDto ToDto(Booking b, Room r) =>
        new(b.Id, b.RoomId, b.GuestName, b.CheckIn, b.CheckOut, TotalPrice(b, r), b.Status.ToString());
}