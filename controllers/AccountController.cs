using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Models.ViewModels;

namespace TradeSpace.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. РЕЄСТРАЦІЯ
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Перевіряємо, чи немає вже такого Email
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Цей Email вже зайнятий.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await Authenticate(user); // Автоматично логінимо після реєстрації
            return RedirectToAction("Index", "Products");
        }
        return View(model);
    }

    // 2. ВХІД
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            
            // Перевіряємо юзера та збіг кешів паролів
            if (user != null && user.PasswordHash == HashPassword(model.Password))
            {
                await Authenticate(user);
                return RedirectToAction("Index", "Products");
            }
            ModelState.AddModelError("", "Невірний Email або пароль");
        }
        return View(model);
    }

    // 3. ВИХІД
    [HttpPost]
    [Authorize] // Тільки для авторизованих
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Products");
    }

    // 4. ОСОБИСТИЙ КАБІНЕТ
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        // Отримуємо ID поточного користувача з його Cookie
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (Guid.TryParse(userIdStr, out Guid userId))
        {
            // Завантажуємо юзера разом з його замовленнями
            var user = await _context.Users
                .Include(u => u.Orders)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            return View(user);
        }
        return RedirectToAction("Login");
    }

    // --- ДОПОМІЖНІ МЕТОДИ ---

    // Створення Cookie-сесії для користувача
    private async Task Authenticate(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    // Простий кешер паролів
    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}