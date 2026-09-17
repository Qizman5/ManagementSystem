using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    [Authorize]
    public class UserActionsController : Controller
    {
        private readonly AppDbContext _context;

        public UserActionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UserActions
        public async Task<IActionResult> Index()
        {
            var actions = await _context.UserActions
                .Include(u => u.User)
                .Include(u => u.Item)
                .AsNoTracking()
                .OrderByDescending(u => u.ActionDate)
                .ToListAsync();

            return View(actions);
        }

        // GET: UserActions/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.Items.AsNoTracking(), "Id", "Name");
            ViewData["UserId"] = new SelectList(_context.Users.AsNoTracking(), "Id", "Username");
            return View();
        }

        // POST: UserActions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,ItemId,ActionType,Quantity,Note")] UserAction userAction)
        {
            userAction.ActionDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                var item = await _context.Items.FindAsync(userAction.ItemId);
                if (item == null) return NotFound("Товар не знайдено");

                // Підтримка обох варіантів назв операцій (українською та англійською)
                if (userAction.ActionType == "Прихід" || userAction.ActionType == "Receipt")
                {
                    item.Quantity += userAction.Quantity;
                }
                else if (userAction.ActionType == "Списання" || userAction.ActionType == "Shipment")
                {
                    if (item.Quantity < userAction.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "Недостатня кількість товару на складі.");
                        ViewData["ItemId"] = new SelectList(_context.Items.AsNoTracking(), "Id", "Name", userAction.ItemId);
                        ViewData["UserId"] = new SelectList(_context.Users.AsNoTracking(), "Id", "Username", userAction.UserId);
                        return View(userAction);
                    }
                    item.Quantity -= userAction.Quantity;
                }

                _context.Add(userAction);
                _context.Update(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ItemId"] = new SelectList(_context.Items.AsNoTracking(), "Id", "Name", userAction.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users.AsNoTracking(), "Id", "Username", userAction.UserId);
            return View(userAction);
        }

        // GET: UserActions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var action = await _context.UserActions
                .Include(u => u.Item)
                .Include(u => u.User)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (action == null) return NotFound();

            return View(action);
        }

        // POST: UserActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var action = await _context.UserActions.FindAsync(id);
            if (action != null)
            {
                _context.UserActions.Remove(action);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}