using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using BCrypt.Net;

namespace ServerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // --- ВХІД (LOGIN) ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var inputLogin = model.Username?.Trim();
            var inputPassword = model.Password?.Trim();

            // 1. ГОЛОВНИЙ АДМІНІСТРАТОР (Хардкод bypass)
            if ((inputLogin == "admin" || inputLogin == "arotar2005@gmail.com") && inputPassword == "0000")
            {
                await AuthenticateUser("arotar2005@gmail.com", "Admin");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            // 2. ПЕРЕВІРКА КОРИСТУВАЧІВ З БАЗИ ДАНИХ (Users)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == inputLogin || u.Email == inputLogin);

            if (user == null)
            {
                ModelState.AddModelError("", "Користувача з таким логіном або поштою не знайдено");
                return View(model);
            }

            // Перевіряємо хеш BCrypt або чистий текст (на випадок старих записів)
            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(inputPassword, user.PasswordHash);
            }
            catch
            {
                isPasswordValid = user.PasswordHash == inputPassword;
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Невірний пароль");
                return View(model);
            }

            // Успішна авторизація звичайного користувача
            await AuthenticateUser(user.Username, user.Role ?? "Worker");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Items");
        }

        // --- РЕЄСТРАЦІЯ (REGISTER) ---
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _context.Users
                .AnyAsync(u => u.Email == model.Email || u.Username == model.Username);

            if (existingUser)
            {
                ModelState.AddModelError("", "Користувач з такою поштою або логіном вже існує!");
                return View(model);
            }

            // Хешуємо пароль перед збереженням у БД
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "Worker"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // --- ВИХІД (LOGOUT) ---
        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Повне очищення кукі сесії
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Перенаправлення на сторінку входу з очищенням стану
            return RedirectToAction("Login", "Account");
        }

        // Допоміжний метод аутентифікації (Cookie) із забороною постійного кешування сесії
        private async Task AuthenticateUser(string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Встановлюємо IsPersistent = false, щоб сесія не зберігалася в браузері після виходу
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // Очищаємо попередні сесії
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
        }
    }
}
