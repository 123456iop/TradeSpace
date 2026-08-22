using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSpace.Models;

namespace TradeSpace.Services;

public interface IProductService
{
    Task<(IEnumerable<Product> Products, int TotalPages)> GetFilteredProductsAsync(
        string? searchString, Guid? categoryId, int pageNumber, int pageSize);

    Task<IEnumerable<Product>> GetAllActiveProductsAsync();
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateStockAsync(Guid productId, int quantityChange);
}