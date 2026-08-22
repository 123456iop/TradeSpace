using Microsoft.AspNetCore.Mvc;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    
    // Тестовий ID за замовчуванням, поки немає повноцінної авторизації
    private static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: /Cart?userId=...
    [HttpGet]
    public async Task<IActionResult> Index(Guid? userId)
    {
        // Якщо ID не передано, використовуємо дефолтний тестовий
        var currentUserId = userId ?? DefaultUserId;

        var cart = await _cartService.GetCartByUserIdAsync(currentUserId);
        return View(cart);
    }

    // POST: /Cart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid? userId, Guid productId, int quantity)
    {
        var currentUserId = userId ?? DefaultUserId;

        try
        {
            await _cartService.AddToCartAsync(currentUserId, productId, quantity);
            return RedirectToAction(nameof(Index), new { userId = currentUserId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Details", "Products", new { id = productId });
        }
    }

    // POST: /Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(Guid cartId, Guid? userId)
    {
        var currentUserId = userId ?? DefaultUserId;

        await _cartService.ClearCartAsync(cartId);
        return RedirectToAction(nameof(Index), new { userId = currentUserId });
    }
}