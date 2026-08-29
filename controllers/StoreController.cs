using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Models.ViewModels;

namespace TradeSpace.Controllers;

[Authorize(Roles = "Seller,Admin")]
public class StoreController : Controller
{
    private readonly ApplicationDbContext _context;

    public StoreController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out Guid userId))
        {
            var existingStore = await _context.Stores.FirstOrDefaultAsync(s => s.UserId == userId);
            if (existingStore != null)
            {
                return RedirectToAction("Index", "Account");
            }
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStoreViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out Guid userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var existingStore = await _context.Stores.FirstOrDefaultAsync(s => s.UserId == userId);
        if (existingStore != null)
        {
            return RedirectToAction("Index", "Account");
        }

        var store = new Store
        {
            Name = model.Name,
            Description = model.Description,
            LogoUrl = model.LogoUrl,
            UserId = userId
        };

        _context.Stores.Add(store);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Account");
    }
}