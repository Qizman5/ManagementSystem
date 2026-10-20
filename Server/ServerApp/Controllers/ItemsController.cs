using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize] // Доступ мають авторизовані користувачі (Admin та Worker)
    public class ItemsController : Controller
    {
        private readonly AppDbContext _context;

        public ItemsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Items або /Items/Index
        [HttpGet]
        [Route("Items")]
        [Route("Items/Index")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View("~/Views/Items/Index.cshtml", items);
        }

        // GET: /Items/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Items/Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Items/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Items/Create")]
        public async Task<IActionResult> Create([Bind("Name,Quantity,Price,Discount")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: /Items/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Items/Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Items/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Items/Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,Discount")] Item item)
        {
            if (id != item.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Items.Any(e => e.Id == item.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: /Items/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Items/Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Items.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /Items/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Items/Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}