using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
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

            if (string.IsNullOrEmpty(inputLogin) || string.IsNullOrEmpty(inputPassword))
            {
                ModelState.AddModelError("", "Заповніть усі поля.");
                return View(model);
            }

            // 1. Резервний вхід головного системного адміністратора
            if ((inputLogin == "admin" || inputLogin == "arotar2005@gmail.com") && (inputPassword == "0000" || inputPassword == "Admin123!"))
            {
                await AuthenticateUser(1, "arotar2005@gmail.com", "Admin");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Admin");
            }

            // 2. Вхід користувачів із бази даних MySQL
            var worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.Username == inputLogin || w.Email == inputLogin);

            if (worker == null)
            {
                ModelState.AddModelError("", "Користувача з таким логіном або поштою не знайдено.");
                return View(model);
            }

            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(inputPassword, worker.PasswordHash);
            }
            catch
            {
                isPasswordValid = worker.PasswordHash == inputPassword;
            }

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Невірний пароль.");
                return View(model);
            }

            string usernameToUse = !string.IsNullOrEmpty(worker.Name) ? worker.Name : (!string.IsNullOrEmpty(worker.Username) ? worker.Username : worker.Email);
            string userRole = !string.IsNullOrEmpty(worker.Role) ? worker.Role : "Worker";

            // Створення сесії Cookie із записом відповідної ролі
            await AuthenticateUser(worker.Id, usernameToUse, userRole);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Перенаправлення залежно від ролі:
            if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            // Звичайний користувач (Worker) направляється виключно на товари
            return RedirectToAction("Index", "Items");
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Перевірка дублікатів за Email або Username
            var existingWorker = await _context.Workers
                .AnyAsync(w => w.Email == model.Email || (w.Username != null && w.Username == model.Username));

            if (existingWorker)
            {
                ModelState.AddModelError("", "Користувач з такою поштою або логіном вже існує!");
                return View(model);
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Створення нового користувача з обов'язковою ролью Worker
            var newWorker = new Worker
            {
                Name = model.Name,
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = "Worker" // Нові користувачі завжди отримують роль Worker
            };

            _context.Workers.Add(newWorker);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST/GET: /Account/Logout
        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        private async Task AuthenticateUser(int userId, string username, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username ?? string.Empty),
                new Claim(ClaimTypes.Role, role ?? "Worker")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            };

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );
        }
    }
}
