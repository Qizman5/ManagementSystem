using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Додавання підтримки MVC (Controllers + Views)
builder.Services.AddControllersWithViews();

// 2. Підключення до бази даних MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }
        )
    );
});

// 4. Проксі-заголовки
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// 5. Конфігурація JWT та Cookie
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForJWTTokenGeneration2026!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "ServerApp";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "ServerAppUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "WarehouseAuthCookie";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BearerOrCookie", policy =>
    {
        policy.AddAuthenticationSchemes(
            JwtBearerDefaults.AuthenticationScheme, 
            CookieAuthenticationDefaults.AuthenticationScheme
        );
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.UseForwardedHeaders();

// Ініціалізація БД та створення Admin за замовчуванням
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    if (!dbContext.Workers.Any(w => w.Role == "Admin"))
    {
        dbContext.Workers.Add(new Worker
        {
            Name = "System Admin",
            Username = "admin",
            FullName = "System Admin",
            Email = "arotar2005@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("0000"),
            Role = "Admin"
        });
        dbContext.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Перенаправлення з Swagger
app.MapGet("/swagger", context => { context.Response.Redirect("/Account/Login"); return Task.CompletedTask; });
app.MapGet("/swagger/index.html", context => { context.Response.Redirect("/Account/Login"); return Task.CompletedTask; });

// Головна маршрутизація: за замовчуванням редірект на каталог товарів /Items
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
