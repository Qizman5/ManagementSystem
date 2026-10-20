using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    // Доступ до ВСІХ дій цього контролера дозволено ЛИШЕ користувачам з роллю "Admin"
    [Authorize(Roles = "Admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. ГОЛОВНА ПАНЕЛЬ ТА ТОВАРИ
        // ==========================================

        // GET: /Admin або /Admin/Index
        [HttpGet]
        [Route("Admin")]
        [Route("Admin/Index")]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Items.AsNoTracking().ToListAsync();
            return View(items);
        }

        // ==========================================
        // 2. УПРАВЛІННЯ КОРИСТУВАЧАМИ (USERS)
        // ==========================================

        // GET: /Users або /Admin/Users
        [HttpGet]
        [Route("Users")]
        [Route("Admin/Users")]
        public async Task<IActionResult> Users()
        {
            var workers = await _context.Workers.AsNoTracking().ToListAsync();
            return View("~/Views/Users/Index.cshtml", workers);
        }

        // GET: /Users/Edit/5 або /Admin/EditUser/5
        [HttpGet]
        [Route("Users/Edit/{id}")]
        [Route("Admin/EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Users/Edit/{id}")]
        [Route("Admin/EditUser/{id}")]
        public async Task<IActionResult> EditUser(int id, Worker worker)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Workers.Any(w => w.Id == worker.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Users));
            }
            return View(worker);
        }

        // GET: /Users/Delete/5
        [HttpGet]
        [Route("Users/Delete/{id}")]
        [Route("Admin/DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var worker = await _context.Workers.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
            if (worker == null)
            {
                return NotFound();
            }
            return View(worker);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("DeleteUser")]
        [ValidateAntiForgeryToken]
        [Route("Users/Delete/{id}")]
        [Route("Admin/DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUserConfirmed(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker != null)
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Users));
        }

        // ==========================================
        // 3. ЖУРНАЛ ДІЙ КОРИСТУВАЧІВ (USER ACTIONS)
        // ==========================================

        // GET: /UserActions або /Admin/UserActions
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

            return View("~/Views/UserActions/Index.cshtml", actions);
        }

        // GET: /UserActions/Delete/5
        [HttpGet]
        [Route("UserActions/Delete/{id}")]
        public async Task<IActionResult> DeleteUserAction(int id)
        {
            var action = await _context.UserActions.FindAsync(id);
            if (action != null)
            {
                _context.UserActions.Remove(action);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(UserActions));
        }

        // ==========================================
        // 4. ПЕРЕНАПРАВЛЕННЯ НА ТОВАРИ (ITEMS)
        // ==========================================

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
