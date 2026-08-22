using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ApplicationDbContext _context; // Додаємо контекст для витягування категорій

    public ProductsController(IProductService productService, ApplicationDbContext context)
    {
        _productService = productService;
        _context = context;
    }

    // GET: /Products
    [HttpGet]
    public async Task<IActionResult> Index(string? searchString, Guid? categoryId, int pageNumber = 1)
    {
        int pageSize = 8; // Кількість товарів на сторінку

        var (products, totalPages) = await _productService.GetFilteredProductsAsync(searchString, categoryId, pageNumber, pageSize);

        // Передаємо список категорій для випадаючого списку / фільтрів
        ViewBag.Categories = await _context.Categories.ToListAsync();
        ViewBag.CurrentSearch = searchString;
        ViewBag.CurrentCategory = categoryId;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;

        return View(products);
    }

    // GET: /Products/Details/{id}
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    // GET: /Products/Create
    [HttpGet]
    public IActionResult Create() => View();

    // POST: /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);

        var createdProduct = await _productService.CreateProductAsync(product);
        return RedirectToAction(nameof(Details), new { id = createdProduct.Id });
    }
}