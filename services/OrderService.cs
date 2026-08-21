using System;
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

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Order> CheckoutAsync(Guid userId, string shippingAddress)
        {
            var cart = await _cartService.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Кошик порожній. Неможливо створити замовлення.");
            }

            // Рахуємо суму на основі елементів кошика
            var totalAmount = cart.Items.Sum(i => i.Quantity * (i.Product?.Price ?? 0));

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = shippingAddress,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending, // Використовуємо Enum, як у вашій моделі
                CreatedAt = DateTime.UtcNow
            };

            // Переносимо товари з CartItem в OrderItem
            foreach (var item in cart.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product?.Price ?? 0
                });
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Очищаємо кошик після успішного оформлення
            await _cartService.ClearCartAsync(cart.Id);

            return order;
        }
    }
}