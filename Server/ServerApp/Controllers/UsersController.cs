using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    // 🛑 ДОСТУП ДОЗВОЛЕНО ЛИШЕ АДМІНІСТРАТОРАМ
    [Authorize(Roles = "Admin")] 
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var workers = await _context.Workers.AsNoTracking().ToListAsync();
            return View(workers);
        }
    }
}
