using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: /Auth/Login (Сторінка з формою)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /auth/login (Видача JWT-токена)
        [HttpPost("auth/login")]
        public IActionResult Login(string username, string password)
        {
            // 1. Перевірка адміністратора за кодом з AdminCredentials
            if (username == AdminCredentials.Username && password == AdminCredentials.Password)
            {
                // 2. Генерація JWT-токена
                var token = GenerateJwtToken(username);

                // 3. Збереження токена в Cookie для MVC-авторизації в браузері
                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Невірний логін або пароль!";
            return View();
        }

        // GET: /auth/logout (Вихід з системи)
        [HttpGet("auth/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("X-Access-Token");
            return RedirectToAction("Login");
        }

        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
