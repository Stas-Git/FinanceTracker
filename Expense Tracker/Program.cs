using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Додаємо необхідні сервіси
builder.Services.AddControllersWithViews();

// Реєструємо UserService для впровадження залежностей
builder.Services.AddScoped<UserService>();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("uk-UA");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("uk-UA");

// Конфігурація з'єднання з базою даних
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// Реєструємо ліцензію Syncfusion
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VVhhQlFac1pJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdkNjWn9edHNRRmZYWEM=");

// Додаємо аутентифікацію за допомогою cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // Перенаправлення на сторінку логіну
        options.LogoutPath = "/Account/Logout"; // Перенаправлення на сторінку для виходу
        options.AccessDeniedPath = "/Account/AccessDenied"; // Перенаправлення на сторінку доступу
    });

var app = builder.Build();

// Налаштовуємо обробку HTTP запитів
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// Додаємо маршрутизацію
app.UseRouting();

// Включаємо аутентифікацію та авторизацію
app.UseAuthentication(); // Це має бути перед UseAuthorization
app.UseAuthorization();

// Налаштування маршруту для контролерів
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Додатковий маршрут для адміністраторів
app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Users}/{id?}");

// Запуск додатку
app.Run();
