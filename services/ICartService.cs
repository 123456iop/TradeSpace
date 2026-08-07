// CartService.cs Interface

namespace TradeSpace.Services;
using TradeSpace.Models;

public interface ICartService
{
    Task<Cart> GetCartByUserIdAsync(Guid userId);
    Task AddToCartAsync(Guid userId, Guid productId, int quantity);
    Task ClearCartAsync(Guid cartId);
}