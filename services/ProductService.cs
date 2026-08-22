using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;

namespace TradeSpace.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Оновлений метод із підтримкою пошуку, категорії та пагінації
        public async Task<(IEnumerable<Product> Products, int TotalPages)> GetFilteredProductsAsync(
            string? searchString, Guid? categoryId, int pageNumber, int pageSize)
        {
            var query = _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Store)
                .AsQueryable();

            // Фільтрація за пошуковим запитом
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(p => p.Title.Contains(searchString) || (p.Description != null && p.Description.Contains(searchString)));
            }

            // Фільтрація за категорією
            if (categoryId.HasValue && categoryId.Value != Guid.Empty)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Рахуємо загальну кількість сторінок
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Застосовуємо пагінацію
            var products = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalPages);
        }

        // Залишаємо старий метод для сумісності, якщо він десь ще використовується
        public async Task<IEnumerable<Product>> GetAllActiveProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Store)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Include(p => p.Store)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateStockAsync(Guid productId, int quantityChange)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return false;

            product.StockQuantity += quantityChange;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}