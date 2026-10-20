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
using Microsoft.OpenApi.Models;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Додавання MVC (Controllers + Views) та конфігурація Swagger Gen
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Warehouse Management System",
        Version = "v1"
    });

    // Налаштування підтримки авторизації через JWT Bearer у Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введіть JWT токен у форматі: Bearer {your_token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 2. Підключення до бази даних MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 3. Rate Limiting (Обмеження частоти запитів)
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

// 5. Конфігурація JWT та Cookie (Автентифікація)
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

// Ініціалізація бази даних та створення адміністратора за замовчуванням
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

// 🚀 Підключення та кастомізація Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse Management System");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Warehouse Management System";

    // Приховує блок Schemas у Swagger UI
    c.DefaultModelsExpandDepth(-1);

    // Кастомізація стилів Swagger UI
    c.HeadContent = @"
        <style>
            .swagger-ui .topbar .download-url-wrapper { 
                display: none !important; 
            }
            .swagger-ui .topbar-wrapper img {
                content: url('data:image/svg+xml;utf8,<svg xmlns=""http://www.w3.org/2000/svg"" width=""320"" height=""30""><text x=""0"" y=""22"" fill=""white"" font-family=""sans-serif"" font-size=""18"" font-weight=""bold"">Warehouse Management System</text></svg>');
            }
            .swagger-ui .info .title small.version-stamp,
            .swagger-ui .info .title .version,
            .swagger-ui .info .title span {
                display: none !important;
            }
            section.models {
                display: none !important;
            }
        </style>";
});

// Стандартна маршрутизація MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Items}/{action=Index}/{id?}");

app.MapControllers();

app.Run();
