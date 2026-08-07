using Microsoft.AspNetCore.Mvc;
using TradeSpace.Models;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    // Впровадження залежностей (Dependency Injection) через конструктор
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: /Products
    // Отримуємо список усіх активних товарів для відображення в каталозі
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllActiveProductsAsync();
        
        // Повертаємо Razor View (Views/Products/Index.cshtml) із переданою моделлю
        return View(products);
    }

    // GET: /Products/Details/{id}
    // Перегляд детальної картки товару
    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        // Шукаємо товар за його унікальним ідентифікатором
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            // Якщо товар не знайдено, повертаємо стандартну 404 сторінку
            return NotFound();
        }

        return View(product); // Відображаємо Views/Products/Details.cshtml
    }

    // GET: /Products/Create
    // Відображення сторінки з формою створення нового товару
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Products/Create
    // Обробка відправки форми створення нового товару
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        // Перевіряємо валідності введених даних
        if (!ModelState.IsValid)
        {
            // Якщо є помилки, повертаємо форму з підсвіченими помилками
            return View(product);
        }

        // Створюємо новий товар у базі даних
        var createdProduct = await _productService.CreateProductAsync(product);
        
        // Перенаправляємо користувача на сторінку створеного товару
        return RedirectToAction(nameof(Details), new { id = createdProduct.Id });
    }
}