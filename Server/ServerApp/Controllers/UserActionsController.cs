using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers
{
    // 🛑 ДОСТУП ДОЗВОЛЕНО ЛИШЕ АДМІНІСТРАТОРАМ
    [Authorize(Roles = "Admin")]
    public class UserActionsController : Controller
    {
        private readonly AppDbContext _context;

        public UserActionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /UserActions
        public async Task<IActionResult> Index()
        {
            var actions = await _context.UserActions
                .Include(a => a.Worker)
                .Include(a => a.Item)
                .AsNoTracking()
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();

            return View(actions);
        }

        // GET: /UserActions/Create
        public IActionResult Create()
        {
            PopulateSelectLists();
            return View();
        }

        // POST: /UserActions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkerId,ItemId,ActionType,Quantity,Note")] UserAction userAction)
        {
            userAction.ActionDate = DateTime.UtcNow;

            if (ModelState.IsValid)
            {
                var item = await _context.Items.FindAsync(userAction.ItemId);
                if (item == null) return NotFound("Товар не знайдено.");

                var actionType = userAction.ActionType?.Trim().ToLower();

                if (actionType == "прихід" || actionType == "income" || actionType == "receipt")
                {
                    item.Quantity += userAction.Quantity;
                    userAction.ActionType = "Прихід";
                }
                else if (actionType == "списання" || actionType == "outcome" || actionType == "shipment")
                {
                    if (item.Quantity < userAction.Quantity)
                    {
                        ModelState.AddModelError("Quantity", "Недостатня кількість товару на складі.");
                        PopulateSelectLists(userAction.ItemId, userAction.WorkerId);
                        return View(userAction);
                    }
                    item.Quantity -= userAction.Quantity;
                    userAction.ActionType = "Списання";
                }
                else
                {
                    ModelState.AddModelError("ActionType", "Некоректний тип операції.");
                    PopulateSelectLists(userAction.ItemId, userAction.WorkerId);
                    return View(userAction);
                }

                userAction.PriceAtMoment = item.Price;

                _context.UserActions.Add(userAction);
                _context.Items.Update(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateSelectLists(userAction.ItemId, userAction.WorkerId);
            return View(userAction);
        }

        // GET: /UserActions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var action = await _context.UserActions
                .Include(u => u.Item)
                .Include(u => u.Worker)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (action == null) return NotFound();

            return View(action);
        }

        // POST: /UserActions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        private void PopulateSelectLists(int? selectedItemId = null, int? selectedWorkerId = null)
        {
            ViewData["ItemId"] = new SelectList(_context.Items.AsNoTracking(), "Id", "Name", selectedItemId);
            ViewData["WorkerId"] = new SelectList(_context.Workers.AsNoTracking(), "Id", "Name", selectedWorkerId);
        }
    }
}
