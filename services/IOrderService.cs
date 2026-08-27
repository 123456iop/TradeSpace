// OrderService.cs Interface

namespace TradeSpace.Services;
using TradeSpace.Models;

public interface IOrderService
{
    Task<Order> CheckoutAsync(Guid userId, string shippingAddress);
    Task<List<Order>> GetUserOrdersAsync(Guid userId);
    Task<Order?> GetOrderByIdAsync(Guid orderId, Guid userId);
}