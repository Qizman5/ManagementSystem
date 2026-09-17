using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ServerApp.Controllers
{
    [Authorize] // Захищає всі action-методи даного контролера
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}