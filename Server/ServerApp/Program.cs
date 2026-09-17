using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Додавання підтримки контролерів та представлень (MVC)
builder.Services.AddControllersWithViews();

// Підключення до MySQL (із розпізнаванням MySQL 9.0 / 8.0)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));

var app = builder.Build();

// Автоматичне створення бази даних та заповнення тестовими даними при запуску
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Налаштування маршруту за замовчуванням
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();