namespace Domain;

public enum BookingStatus { Confirmed, CheckedIn, Completed, Cancelled }

/// <summary>DATPHONG</summary>
public class Booking
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int RoomId { get; set; }
    public int? AccountId { get; set; }   // MaTK (nullable): nhân viên tạo hộ, null nếu khách tự đặt
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int GuestCount { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; private set; } = BookingStatus.Confirmed;

    public static Booking Create(int customerId, int roomId, DateOnly checkIn, DateOnly checkOut,
                                 int guestCount, decimal totalPrice, int? accountId = null)
    {
        if (checkOut <= checkIn)
            throw new DomainException("Ngày trả phải sau ngày nhận.");
        if (guestCount <= 0)
            throw new DomainException("Số khách phải lớn hơn 0.");
        return new Booking
        {
            CustomerId = customerId, RoomId = roomId, AccountId = accountId,
            CheckIn = checkIn, CheckOut = checkOut,
            GuestCount = guestCount, TotalPrice = totalPrice
        };
    }

    /// <summary>Khoảng [from, to) có chồng lên đặt phòng này không. Đã hủy/hoàn thành thì không chiếm phòng.</summary>
    public bool Overlaps(DateOnly from, DateOnly to) =>
        Status is BookingStatus.Confirmed or BookingStatus.CheckedIn
        && from < CheckOut && to > CheckIn;

    /// <summary>Hoàn 100% nếu hủy trước giờ nhận phòng (14:00) từ 24 giờ trở lên, ngược lại không hoàn.
    /// Giả định khách đã thanh toán đủ TotalPrice.</summary>
    public decimal Cancel(DateTime now)
    {
        if (Status == BookingStatus.Cancelled)
            throw new DomainException("Đặt phòng đã bị hủy.");
        if (Status is BookingStatus.CheckedIn or BookingStatus.Completed)
            throw new DomainException("Không thể hủy đặt phòng đã nhận phòng hoặc đã hoàn thành.");

        Status = BookingStatus.Cancelled;
        var checkInTime = CheckIn.ToDateTime(new TimeOnly(14, 0));
        return checkInTime - now >= TimeSpan.FromHours(24) ? TotalPrice : 0;
    }
}