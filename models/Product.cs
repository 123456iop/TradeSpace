// Product.cs & ProductImage.cs

namespace TradeSpace.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; } // Залишок на складі
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Прив'язка до магазину
    public Guid StoreId { get; set; }
    public Store Store { get; set; } = null!;

    // Прив'язка до категорії
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Зображення і відгуки
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public class ProductImage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false; // Главная обложка товара

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
}