using System.ComponentModel.DataAnnotations;

namespace TradeSpace.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "Назва обов'язкова")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Опис обов'язковий")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 1000000)]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    public List<ProductImage> Images { get; set; } = new();

    public int StockQuantity { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    [Required]
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    public Guid StoreId { get; set; }
    public Store? Store { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}