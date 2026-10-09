namespace Domain;

/// <summary>TAIKHOAN</summary>
public class Account
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Role { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Status { get; set; } = "Active";
}

/// <summary>NHANVIEN</summary>
public class Staff
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string NationalId { get; set; } = "";
    public DateTime HireDate { get; set; }
}

/// <summary>KHACHHANG</summary>
public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string IdNumber { get; set; } = "";   // CCCD_HoChieu
    public DateTime CreatedAt { get; set; }
}

/// <summary>NHAT_KY_HOAT_DONG</summary>
public class ActivityLog
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Action { get; set; } = "";
}