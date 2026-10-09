using Domain;
namespace Application;

public record BookingDto(int Id, int RoomId, int CustomerId, string CustomerName,
                         DateOnly CheckIn, DateOnly CheckOut, int GuestCount,
                         decimal TotalPrice, string Status);
public record CancelResultDto(int BookingId, string Status, decimal RefundAmount);

internal static class BookingMapper
{
    public static BookingDto ToDto(Booking b, string customerName) =>
        new(b.Id, b.RoomId, b.CustomerId, customerName, b.CheckIn, b.CheckOut,
            b.GuestCount, b.TotalPrice, b.Status.ToString());
}