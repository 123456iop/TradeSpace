namespace TradeSpace.Services;
using TradeSpace.Models;
using System;
using System.Threading.Tasks;

public interface ICartService
{
    // Отримання кошика за ID користувача
    Task<Cart> GetCartByUserIdAsync(Guid userId);
    
    // Додавання товару до кошика
    Task AddToCartAsync(Guid userId, Guid productId, int quantity);
    
    // Очищення всього кошика
    Task ClearCartAsync(Guid cartId);
    
    // Видалення конкретного товару з кошика
    Task RemoveItemAsync(Guid cartItemId);
}