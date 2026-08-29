namespace TradeSpace.Models;

public class ProductImage
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public bool IsMain { get; set; } = false;

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}