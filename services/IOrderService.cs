// OrderService.cs Interface

namespace TradeSpace.Services;
using TradeSpace.Models;

public interface IOrderService
{
    Task<Order> CheckoutAsync(Guid userId, string shippingAddress);
}