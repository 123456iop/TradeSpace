using Microsoft.AspNetCore.Mvc;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: /Cart?userId=...
    // Перегляд кошика користувача
    [HttpGet]
    public async Task<IActionResult> Index(Guid userId)
    {
        // Отримуємо кошик конкретного користувача (якщо його немає, сервіс створить новий)
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return View(cart); // Відображаємо Views/Cart/Index.cshtml
    }

    // POST: /Cart/Add
    // Додавання товару до кошика з форми на сторінці каталогу або детальної картки
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid userId, Guid productId, int quantity)
    {
        try
        {
            // Намагаємося додати товар до кошика
            await _cartService.AddToCartAsync(userId, productId, quantity);
            
            // Після успішного додавання перенаправляємо користувача до його кошика
            return RedirectToAction(nameof(Index), new { userId });
        }
        catch (Exception ex)
        {
            // Якщо товару немає на складі або сталася інша помилка,
            // зберігаємо текст помилки у TempData та повертаємо на сторінку товару
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }

    // POST: /Cart/Clear
    // Очищення всіх товарів із кошика
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(Guid cartId, Guid userId)
    {
        // Очищаємо всі товари з кошика
        await _cartService.ClearCartAsync(cartId);
        
        // Оновлюємо сторінку кошика
        return RedirectToAction(nameof(Index), new { userId });
    }
}
