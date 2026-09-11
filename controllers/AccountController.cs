using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Models.ViewModels;

namespace TradeSpace.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    private Guid GetCurrentUserId()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return RedirectToAction(nameof(Login));
        return View(user);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Index));

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Користувач з таким Email вже існує.");
            return View(model);
        }

        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PasswordHash = model.Password,
            Role = UserRole.Customer,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(Index));

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == model.Password);
        if (user == null)
        {
            ModelState.AddModelError("", "Невірна пошта або пароль.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return RedirectToAction(nameof(Login));

        var model = new EditProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return RedirectToAction(nameof(Login));

        bool nameChanged = user.FirstName != model.FirstName || user.LastName != model.LastName;

        if (nameChanged)
        {
            if (user.NameLastChangedAt.HasValue && user.NameLastChangedAt.Value.AddDays(30) > DateTime.UtcNow)
            {
                var nextDate = user.NameLastChangedAt.Value.AddDays(30);
                ModelState.AddModelError("", $"Ім'я можна змінювати лише раз на 30 днів. Наступна зміна доступна з {nextDate:dd.MM.yyyy}.");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.NameLastChangedAt = DateTime.UtcNow;
        }

        user.PhoneNumber = model.PhoneNumber;

        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Профіль успішно оновлено.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Нові паролі не збігаються.");
            return View();
        }

        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return RedirectToAction(nameof(Login));

        if (user.PasswordHash != currentPassword)
        {
            ModelState.AddModelError("", "Невірний поточний пароль.");
            return View();
        }

        user.PasswordHash = newPassword;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Пароль успішно змінено.";
        return RedirectToAction(nameof(Index));
    }
}