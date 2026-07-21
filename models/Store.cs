namespace TradeSpace.Models;

public class Store
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Власник магазину
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Товари магазину
    public ICollection<Product> Products { get; set; } = new List<Product>();
}