using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data; 
using TradeSpace.Models;

namespace TradeSpace.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        // Впроваджуємо контекст БД та сервіс кошика
        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(int userId, string shippingAddress)
        {
            // 1. Отримуємо всі товари з кошика користувача
            var cartItems = await _cartService.GetCartByUserIdAsync(userId);

            if (cartItems == null || !cartItems.Any())
            {
                throw new InvalidOperationException("Кошик порожній. Неможливо створити замовлення.");
            }

            // 2. Рахуємо загальну суму замовлення
            var totalAmount = await _cartService.GetTotalAsync(userId);

            // 3. Створюємо об'єкт замовлення
            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                TotalAmount = totalAmount,
                Status = "Нове",
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 4. Очищаємо кошик після успішного оформлення замовлення
            await _cartService.ClearCartAsync(userId);

            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            // Отримуємо замовлення
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            // Отримуємо історію замовлень користувача, сортуючи від найновіших
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Отримуємо всі замовлення для панелі адміністратора
            return await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order != null)
            {
                // Оновлюємо статус та зберігаємо зміни
                order.Status = status;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}