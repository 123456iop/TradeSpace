//  ProductService.cs Interface

namespace TradeSpace.Services;
using TradeSpace.Models;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllActiveProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateStockAsync(Guid productId, int quantityChange);
}