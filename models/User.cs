namespace TradeSpace.Models;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    public DateTime CreatedAt { get; set; }

    public ICollection<Store> Stores { get; set; } = new List<Store>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}