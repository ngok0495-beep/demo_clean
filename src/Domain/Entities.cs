namespace Domain;

public class DomainException(string message) : Exception(message);

public enum RoomStatus { Available, Occupied, Cleaning, Maintenance }
public enum BookingStatus { Confirmed, Cancelled }

public class Room
{
    public int Id { get; init; }
    public string Number { get; init; } = "";
    public string Type { get; init; } = "";
    public decimal PricePerNight { get; init; }
    public RoomStatus Status { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; init; }
    public string GuestName { get; init; } = "";
    public DateOnly CheckIn { get; init; }
    public DateOnly CheckOut { get; init; }
    public BookingStatus Status { get; private set; } = BookingStatus.Confirmed;

    public static Booking Create(int roomId, string guestName, DateOnly checkIn, DateOnly checkOut)
    {
        if (string.IsNullOrWhiteSpace(guestName))
            throw new DomainException("Tên khách không được để trống.");
        if (checkOut <= checkIn)
            throw new DomainException("Ngày trả phải sau ngày nhận.");
        return new Booking { RoomId = roomId, GuestName = guestName, CheckIn = checkIn, CheckOut = checkOut };
    }

    public bool Overlaps(DateOnly from, DateOnly to) =>
        Status == BookingStatus.Confirmed && from < CheckOut && to > CheckIn;

    // Chính sách hủy: hoàn 100% nếu hủy trước 24h, sau đó không hoàn
    public decimal Cancel(DateTime now, decimal paidAmount)
    {
        if (Status == BookingStatus.Cancelled)
            throw new DomainException("Đặt phòng đã bị hủy.");
        Status = BookingStatus.Cancelled;
        var checkInTime = CheckIn.ToDateTime(new TimeOnly(14, 0));
        return checkInTime - now >= TimeSpan.FromHours(24) ? paidAmount : 0;
    }
}