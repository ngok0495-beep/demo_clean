namespace Domain;

/// <summary>HOADON</summary>
public class Invoice
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int AccountId { get; set; }
    public decimal RoomAmount { get; set; }
    public decimal ServiceAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; } = "";
    public string Status { get; set; } = "Unpaid";
}

/// <summary>GIAODICH</summary>
public class PaymentTransaction
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int? InvoiceId { get; set; }
    public string Gateway { get; set; } = "";              // CongThanhToan
    public string GatewayTransactionId { get; set; } = ""; // MaGDCong
    public string Type { get; set; } = "";                 // LoaiGD
    public decimal Amount { get; set; }
    public string Status { get; set; } = "";
    public bool Reconciled { get; set; }                   // DaDoiSoat
    public DateTime OccurredAt { get; set; }
}