using Microsoft.AspNetCore.Mvc;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: /Orders/Checkout?userId=...
    // Відображення сторінки з формою введення адреси доставки
    [HttpGet]
    public IActionResult Checkout(Guid userId)
    {
        ViewBag.UserId = userId;
        return View(); // Відображаємо Views/Orders/Checkout.cshtml
    }

    // POST: /Orders/Checkout
    // Обробка відправки формы оформлення замовлення
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(Guid userId, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
        {
            ModelState.AddModelError("shippingAddress", "Будь ласка, вкажіть адресу доставки.");
            ViewBag.UserId = userId;
            return View();
        }

        try
        {
            // Викликаємо сервіс оформлення замовлення.
            // Він автоматично перенесе товари з кошика, зафіксує ціни та спише залишки.
            var order = await _orderService.CheckoutAsync(userId, shippingAddress);
            
            // Перенаправляємо на сторінку подяки за покупку
            return RedirectToAction(nameof(Success), new { orderId = order.Id });
        }
        catch (Exception ex)
        {
            // Обробка помилок (наприклад, якщо кошик порожній)
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.UserId = userId;
            return View();
        }
    }

    // GET: /Orders/Success?orderId=...
    // Сторінка успішного оформлення замовлення 
    [HttpGet]
    public IActionResult Success(Guid orderId)
    {
        ViewBag.OrderId = orderId;
        return View(); // Відображаємо Views/Orders/Success.cshtml
    }
}