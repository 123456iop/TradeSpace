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
    Task<IEnumerable<Product>> GetSellerProductsAsync(Guid storeId);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<Product> CreateProductAsync(Product product);
    Task UpdateProductAsync(Product product);
    Task DeleteProductAsync(Guid id, Guid storeId);
    Task<bool> UpdateStockAsync(Guid productId, int quantityChange);
    Task<IEnumerable<Category>> GetStoreCategoriesAsync(Guid storeId);
    Task<Category> CreateCategoryAsync(Category category);
}