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
            // Шукаємо кошик разом із його елементами та даними товарів
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Якщо кошика ще немає - створюємо його
            if (cart == null)
            {
                // Перевіряємо, чи існує користувач у базі
                var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
                if (!userExists)
                {
                    // Якщо користувача немає, ми не можемо створювати кошик через обмеження зовнішнього ключа.
                    // Викидаємо зрозумілу помилку замість мовчазного створення тестового користувача, 
                    // яке могло ламати DbContext.
                    throw new Exception("Користувача не знайдено. Будь ласка, перезайдіть в систему.");
                }

                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                
                // Одразу зберігаємо новий кошик у БД, щоб отримати його Id для товарів
                await _context.SaveChangesAsync();
                
                // Ініціалізуємо порожню колекцію для уникнення NullReferenceException
                cart.Items = new List<CartItem>();
            }

            return cart;
        }

        public async Task AddToCartAsync(Guid userId, Guid productId, int quantity)
        {
            // 1. Отримуємо або створюємо кошик
            var cart = await GetCartByUserIdAsync(userId);
            
            // 2. Шукаємо товар явним запитом до БД, щоб уникнути проблем із відстеженням (tracking)
            // що викликають DbUpdateConcurrencyException
            var existingItem = await _context.Set<CartItem>()
                .FirstOrDefaultAsync(i => i.CartId == cart.Id && i.ProductId == productId);

            if (existingItem != null)
            {
                // Якщо товар вже є, оновлюємо кількість та явно вказуємо EF Core про зміни
                existingItem.Quantity += quantity;
                _context.Set<CartItem>().Update(existingItem);
            }
            else
            {
                // Якщо товару немає, створюємо новий запис із явним вказуванням CartId
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                
                // Додаємо безпосередньо в таблицю CartItem
                _context.Set<CartItem>().Add(newItem);
            }

            // 3. Зберігаємо зміни
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(Guid cartItemId)
        {
            var item = await _context.Set<CartItem>().FindAsync(cartItemId);
            if (item != null)
            {
                _context.Set<CartItem>().Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(Guid cartId)
        {
            var items = await _context.Set<CartItem>()
                .Where(i => i.CartId == cartId)
                .ToListAsync();

            if (items.Any())
            {
                _context.Set<CartItem>().RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}