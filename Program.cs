using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TradeSpace.Data;
using TradeSpace.Models;
using TradeSpace.Services;

var builder = WebApplication.CreateBuilder(args);

// Додавання підтримки контролерів та представлень (MVC)
builder.Services.AddControllersWithViews();

// Налаштування бази даних SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=tradespace.db"));

// Реєстрація сервісів (Dependency Injection)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Налаштування кешування та сесій
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Важливо для роботи сесій без згоди на cookies
});

// Налаштування Cookie-аутентифікації
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/Error";
    });

var app = builder.Build();

// Налаштування middleware для обробки помилок
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

// Увімкнення сесій (повинно бути перед аутентифікацією)
app.UseSession();

// Увімкнення аутентифікації та авторизації
app.UseAuthentication();
app.UseAuthorization();

// Налаштування стандартного маршруту
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

// Ініціалізація бази даних та створення адміністратора при запуску
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    context.Database.EnsureCreated();
    
    // Надання прав адміністратора вказаному користувачу
    var adminUser = context.Users.FirstOrDefault(u => u.Email == "zenaigrokritik@gmail.com");
    if (adminUser != null)
    {
        adminUser.Role = UserRole.Admin;
        context.SaveChanges();
    }
}

app.Run();