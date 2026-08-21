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

        public async Task<IEnumerable<Product>> GetAllActiveProductsAsync()
        {
            // Отримуємо лише активні товари
            return await _context.Products
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
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