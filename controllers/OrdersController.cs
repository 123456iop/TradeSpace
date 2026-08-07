using Microsoft.AspNetCore.Mvc;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // Офоромлення замовлення
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        try
        {
            // Викликаємо сервіс оформлення замовлення.
            // Він автоматично перенесе товари з кошика, зафіксує ціни та спише залишки.
            var order = await _orderService.CheckoutAsync(request.UserId, request.ShippingAddress);
            
            return Ok(new { 
                message = "Замовлення успішно оформлено", 
                orderId = order.Id,
                totalAmount = order.TotalAmount
            });
        }
        catch (Exception ex)
        {
            // Обробка помилок (наприклад, якщо кошик порожній)
            return BadRequest(new { error = ex.Message });
        }
    }
}

// DTO для даних оформлення замовлення
public record CheckoutRequest(Guid UserId, string ShippingAddress);