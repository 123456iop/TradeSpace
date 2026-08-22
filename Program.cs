using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. РЕЄСТРАЦІЯ СЕРВІСІВ (DEPENDENCY INJECTION)
// ==========================================

// Підключення підтримки MVC (Контролери + Razor Views)
builder.Services.AddControllersWithViews();

// Налаштування Entity Framework Core з використанням ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=tradespace.db"));

// Реєстрація сервісів бізнес-логіки
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Підключення сесій
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ==========================================
// 2. ЗБОРКА ДОДАТКУ
// ==========================================
var app = builder.Build();


// ==========================================
// 3. НАЛАШТУВАННЯ MIDDLEWARE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Дефолтний маршрут до стартової сторінки
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");


// ==========================================
// 4. ЗАПУСК
// ==========================================
app.Run();