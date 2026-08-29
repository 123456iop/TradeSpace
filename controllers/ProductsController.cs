using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ApplicationDbContext _context;

    public ProductsController(IProductService productService, ApplicationDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchString, Guid? categoryId, int pageNumber = 1)
    {
        int pageSize = 8;
        var (products, totalPages) = await _productService.GetFilteredProductsAsync(searchString, categoryId, pageNumber, pageSize);

        ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentCategory = categoryId;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;

        return View(products);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpGet]
    public async Task<IActionResult> Manage()
    {
        var store = await GetUserStoreAsync();
        if (store == null) return RedirectToAction("Index", "Account");

        var products = await _productService.GetSellerProductsAsync(store.Id);
        return View(products ?? new List<Product>());
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();
        return View(new Product());
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        var store = await GetUserStoreAsync();
        if (store == null) return RedirectToAction("Index", "Account");

        product.StoreId = store.Id;
        product.CreatedAt = DateTime.UtcNow;

        ModelState.Remove(nameof(product.Store));
        ModelState.Remove(nameof(product.Category));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();
            return View(product);
        }

        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            product.Images = new List<ProductImage>
            {
                new ProductImage { Url = product.ImageUrl, IsMain = true }
            };
        }

        await _productService.CreateProductAsync(product);
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var store = await GetUserStoreAsync();
        if (store == null) return RedirectToAction("Index", "Account");

        var product = await _productService.GetProductByIdAsync(id);
        if (product == null || product.StoreId != store.Id) return NotFound();

        ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();
        return View(product);
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Product product)
    {
        var store = await GetUserStoreAsync();
        if (store == null) return RedirectToAction("Index", "Account");

        var existingProduct = await _productService.GetProductByIdAsync(id);
        if (existingProduct == null || existingProduct.StoreId != store.Id) return NotFound();

        ModelState.Remove(nameof(product.Store));
        ModelState.Remove(nameof(product.Category));

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync() ?? new List<Category>();
            return View(product);
        }

        existingProduct.Title = product.Title;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.IsActive = product.IsActive;
        existingProduct.ImageUrl = product.ImageUrl;
        existingProduct.CategoryId = product.CategoryId;

        await _productService.UpdateProductAsync(existingProduct);
        return RedirectToAction(nameof(Manage));
    }

    [Authorize(Roles = "Seller,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var store = await GetUserStoreAsync();
        if (store == null) return RedirectToAction("Index", "Account");

        await _productService.DeleteProductAsync(id, store.Id);
        return RedirectToAction(nameof(Manage));
    }

    private async Task<Store?> GetUserStoreAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out Guid userId)) return null;

        return await _context.Stores.FirstOrDefaultAsync(s => s.UserId == userId);
    }
}