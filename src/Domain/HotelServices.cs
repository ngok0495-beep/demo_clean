namespace Domain;

/// <summary>DICHVU</summary>
public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "Active";
}

/// <summary>SUDUNGDICHVU</summary>
public class ServiceUsage
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ServiceId { get; set; }
    public int AccountId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }   // DonGiaLuu: đơn giá tại thời điểm sử dụng
    public DateTime UsedAt { get; set; }
}