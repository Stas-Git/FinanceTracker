using Expense_Tracker.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// ������ �������� ������
builder.Services.AddControllersWithViews();

// �������� UserService ��� ������������ �����������
builder.Services.AddScoped<UserService>();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("uk-UA");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("uk-UA");

// ������������ �'������� � ����� �����
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// �������� ������ Syncfusion
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBMAY9C3t2VVhhQlFac1pJXGFWfVJpTGpQdk5xdV9DaVZUTWY/P1ZhSXxRdkNjWn9edHNRRmZYWEM=");

// ������ �������������� �� ��������� cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";   // ��������������� �� ������� �����
        options.LogoutPath = "/Account/Logout"; // ��������������� �� ������� ��� ������
        options.AccessDeniedPath = "/Account/AccessDenied"; // ��������������� �� ������� �������
    });

var app = builder.Build();

// ����������� ������� HTTP ������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

// ������ �������������
app.UseRouting();

// �������� �������������� �� �����������
app.UseAuthentication(); // �� �� ���� ����� UseAuthorization
app.UseAuthorization();

// ������������ �������� ��� ����������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// ���������� ������� ��� ������������
app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Users}/{id?}");

// ������ �������
app.Run();
