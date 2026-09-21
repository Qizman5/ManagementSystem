using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ServerApp.Controllers
{
    [Authorize(Roles = "Admin")] // Доступ дозволено тільки користувачам з роллю Admin
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Передаємо основну метрику та стан систем у View
            ViewBag.ServerStatus = "Online";
            ViewBag.ClientStatus = "Active";
            
            // Інформація про систему
            ViewBag.Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64).ToString(@"d\'d \'h\'h \'m\'m\'");
            ViewBag.OsVersion = Environment.OSVersion.ToString();
            ViewBag.ProcessMemory = (Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024)).ToString() + " MB";

            return View();
        }
    }
}
