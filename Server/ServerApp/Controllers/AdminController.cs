using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(IgnoreApi = true)] // Приховує Controller зі Swagger
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin або Users (Головна сторінка управління)
        [HttpGet]
        [Route("Admin")]
        [Route("Admin/Index")]
        [Route("Users")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View(items);
        }

        // GET: UserActions або Admin/UserActions (Перенаправлення на існуючий UserActionsController)
        [HttpGet]
        [Route("UserActions")]
        [Route("Admin/UserActions")]
        public IActionResult UserActions()
        {
            return RedirectToAction("Index", "UserActions");
        }

        // GET: Admin/CreateProduct або Admin/Create
        [HttpGet("Admin/CreateProduct")]
        [HttpGet("Admin/Create")]
        public IActionResult CreateProduct()
        {
            return RedirectToAction("Create", "Items");
        }

        // GET: Admin/Edit/1
        [HttpGet("Admin/Edit/{id}")]
        public IActionResult Edit(int id)
        {
            return RedirectToAction("Edit", "Items", new { id });
        }

        // GET: Admin/Delete/1
        [HttpGet("Admin/Delete/{id}")]
        public IActionResult Delete(int id)
        {
            return RedirectToAction("Delete", "Items", new { id });
        }
    }
}
