using Microsoft.AspNetCore.Mvc;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // Подовитись кошик користувача
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        // Отримуємо кошик конкретного користувача (якщо його немає, сервіс створить новий)
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return Ok(cart);
    }

    // Додати товар до кошика
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            // Намагаємося додати товар до кошика
            await _cartService.AddToCartAsync(request.UserId, request.ProductId, request.Quantity);
            
            // Повертаємо успішний статус без тіла відповіді
            return Ok(new { message = "Товар успішно додано до кошика" });
        }
        catch (Exception ex)
        {
            // Якщо товару немає на складі, повертаємо помилку 400 (Bad Request)
            return BadRequest(new { error = ex.Message });
        }
    }

    // Видаляємо товари з кошику
    [HttpDelete("{cartId:guid}/clear")]
    public async Task<IActionResult> ClearCart(Guid cartId)
    {
        // Очищаємо всі товари з кошика
        await _cartService.ClearCartAsync(cartId);
        return NoContent(); // Статус 204: запит виконано, повертати нічого
    }
}

// DTO (Data Transfer Object) для прийняття даних з фронтенду
public record AddToCartRequest(Guid UserId, Guid ProductId, int Quantity);