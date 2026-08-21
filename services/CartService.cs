using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;

namespace TradeSpace.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByUserIdAsync(Guid userId)
        {
            // Шукаємо кошик разом із його елементами (CartItem) та даними товарів
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Якщо кошика ще немає - створюємо його, як це очікує контролер
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task AddToCartAsync(Guid userId, Guid productId, int quantity)
        {
            var cart = await GetCartByUserIdAsync(userId);
            
            // Шукаємо товар безпосередньо у списку Items кошика
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId);

            if (cart != null && cart.Items.Any())
            {
                // Очищаємо зв'язані елементи кошика
                _context.RemoveRange(cart.Items);
                await _context.SaveChangesAsync();
            }
        }
    }
}