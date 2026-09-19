using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    
    // Тестовий ID за замовчуванням залишаємо лише як резерв
    private static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // Метод для надійного отримання ідентифікатора авторизованого користувача
    private Guid GetCurrentUserId()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid parsedId))
            {
                return parsedId;
            }
        }
        return DefaultUserId;
    }

    // GET: /Cart
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var cart = await _cartService.GetCartByUserIdAsync(currentUserId);
            return View(cart);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Помилка завантаження кошика: {ex.Message}";
            return View(null);
        }
    }

    // POST: /Cart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Guid productId, int quantity = 1)
    {
        // Серверне обмеження: не менше 1 і не більше 1000
        if (quantity <= 0) quantity = 1;
        if (quantity > 1000) quantity = 1000;

        var currentUserId = GetCurrentUserId();

        try
        {
            await _cartService.AddToCartAsync(currentUserId, productId, quantity);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Помилка при додаванні до кошика: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: /Cart/RemoveItem
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
    {
        try
        {
            await _cartService.RemoveItemAsync(cartItemId);
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Помилка при видаленні товару: {ex.Message}";
        }
        
        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(Guid cartId)
    {
        await _cartService.ClearCartAsync(cartId);
        return RedirectToAction(nameof(Index));
    }
}