using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { message = "Не вказано логін або пароль." });
            }

            var inputLogin = model.Username.Trim();
            var inputPassword = model.Password.Trim();

            // 1. Пошук користувача за Username або Email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == inputLogin || u.Email == inputLogin);

            // Якщо користувача немає в БД — створюємо його на льоту
            if (user == null)
            {
                user = new User
                {
                    Username = inputLogin,
                    Email = inputLogin.Contains("@") ? inputLogin : "arotar2005@gmail.com",
                    PasswordHash = "0000",
                    Role = "Admin"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            // 2. Перевірка пароля (дозволяємо "0000" або чисте порівняння/BCrypt)
            bool isPasswordValid = false;
            var storedHash = (user.PasswordHash ?? "").Trim();

            if (inputPassword == "0000")
            {
                isPasswordValid = true;
            }
            else if (storedHash.StartsWith("$2a$") || storedHash.StartsWith("$2b$") || storedHash.StartsWith("$2y$"))
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
            }
            else
            {
                isPasswordValid = storedHash == inputPassword;
            }

            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Невірний логін або пароль." });
            }

            // 3. Генерація JWT-токена
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json.");
            var key = Encoding.UTF8.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role ?? "Admin")
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Response.Cookies.Append("X-Access-Token", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            });

            return Ok(new 
            { 
                Message = "Успішна авторизація", 
                Token = tokenString 
            });
        }
    }

    public class LoginDto
    {
        [DefaultValue("admin")]
        public string Username { get; set; } = "admin";

        [DefaultValue("0000")]
        public string Password { get; set; } = "0000";
    }
}
