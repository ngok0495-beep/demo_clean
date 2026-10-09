namespace Domain;

public enum RoomStatus { Available, Occupied, Cleaning, Maintenance }

/// <summary>LOAIPHONG</summary>
public class RoomType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal BasePrice { get; set; }
    public string Amenities { get; set; } = "";
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Active";
    public List<SeasonalRate> SeasonalRates { get; set; } = [];

    /// <summary>Giá một đêm: ưu tiên giá mùa vụ (GIAMUAVU), không có thì dùng giá cơ bản.</summary>
    public decimal PriceOn(DateOnly night)
    {
        var rate = SeasonalRates
            .Where(r => r.Covers(night))
            .OrderByDescending(r => r.StartDate)
            .FirstOrDefault();
        return rate?.AppliedPrice ?? BasePrice;
    }

    /// <summary>Tổng tiền từ ngày nhận đến ngày trả (không tính đêm của ngày trả).</summary>
    public decimal TotalFor(DateOnly checkIn, DateOnly checkOut)
    {
        decimal total = 0;
        for (var d = checkIn; d < checkOut; d = d.AddDays(1))
            total += PriceOn(d);
        return total;
    }
}

/// <summary>GIAMUAVU</summary>
public class SeasonalRate
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal AppliedPrice { get; set; }

    public bool Covers(DateOnly night) => StartDate <= night && night <= EndDate;
}

/// <summary>PHONG</summary>
public class Room
{
    public int Id { get; set; }
    public string Number { get; set; } = "";   // SoPhong (cột bổ sung, ERD chưa có)
    public int Floor { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; } = null!;
    public RoomStatus Status { get; set; }
}

/// <summary>SUCOPHONG</summary>
public class RoomIncident
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int AccountId { get; set; }
    public string Description { get; set; } = "";
    public DateTime ReportedAt { get; set; }
    public string Status { get; set; } = "Open";
}