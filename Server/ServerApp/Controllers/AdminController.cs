using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin або Admin/Index (Товари)
        [HttpGet]
        [Route("Admin")]
        [Route("Admin/Index")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View(items);
        }

        // GET: Users або Admin/Users (Користувачі)
        [HttpGet]
        [Route("Users")]
        [Route("Admin/Users")]
        public async Task<IActionResult> Users()
        {
            var workers = await _context.Workers.AsNoTracking().ToListAsync();
            return View(workers); // Повертає Views/Admin/Users.cshtml
        }

        // GET: UserActions або Admin/UserActions (Журнал дій)
        [HttpGet]
        [Route("UserActions")]
        [Route("Admin/UserActions")]
        public async Task<IActionResult> UserActions()
        {
            var actions = await _context.UserActions
                .Include(a => a.Worker)
                .Include(a => a.Item)
                .OrderByDescending(a => a.ActionDate)
                .AsNoTracking()
                .ToListAsync();

            return View(actions); // Повертає Views/Admin/UserActions.cshtml
        }

        [HttpGet("Admin/CreateProduct")]
        [HttpGet("Admin/Create")]
        public IActionResult CreateProduct()
        {
            return RedirectToAction("Create", "Items");
        }

        [HttpGet("Admin/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            return RedirectToAction("Edit", "Items", new { id });
        }

        [HttpGet("Admin/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            return RedirectToAction("Delete", "Items", new { id });
        }
    }
}
