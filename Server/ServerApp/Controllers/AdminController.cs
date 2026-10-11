using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View(items);
        }

        // GET: Admin/Edit/1
        public IActionResult Edit(int id)
        {
            return RedirectToAction("Edit", "Items", new { id });
        }

        // GET: Admin/Delete/1
        public IActionResult Delete(int id)
        {
            return RedirectToAction("Delete", "Items", new { id });
        }
    }
}
