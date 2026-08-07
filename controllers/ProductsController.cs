using Microsoft.AspNetCore.Mvc;
using TradeSpace.Models;
using TradeSpace.Services;

namespace TradeSpace.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    // Впровадження залежностей (Dependency Injection) через конструктор
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // Отримуємо список товарів 
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        // Отримуємо список усіх активних товарів, які є в наявності
        var products = await _productService.GetAllActiveProductsAsync();
        
        // Повертаємо HTTP статус 200 (OK) разом із даними
        return Ok(products);
    }

    // Пошук товару
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        // Шукаємо товар за його унікальним ідентифікатором
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
        {
            // Якщо товар не знайдено, повертаємо помилку 404 (Not Found)
            return NotFound(new { message = "Товар не знайдено" });
        }

        return Ok(product);
    }

    // Створення нового товару
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        // Створюємо новий товар у базі даних
        var createdProduct = await _productService.CreateProductAsync(product);
        
        // Повертаємо статус 201 (Created) та посилання на створений ресурс
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }
}