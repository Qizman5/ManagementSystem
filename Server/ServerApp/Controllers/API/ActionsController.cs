using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/actions")]
    [Route("api/actions")] // Додатковий маршрут про всяк випадок
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] IDictionary<string, object>? rawData)
        {
            if (rawData == null)
            {
                return BadRequest(new { message = "Порожні дані запиту." });
            }

            // Зчитуємо назву товару з будь-якого можливого ключа JSON
            string itemName = string.Empty;
            foreach (var key in new[] { "itemName", "ItemName", "productName", "Name", "name", "title" })
            {
                if (rawData.TryGetValue(key, out var val) && val != null)
                {
                    itemName = val.ToString() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(itemName)) break;
                }
            }

            if (string.IsNullOrWhiteSpace(itemName))
            {
                return BadRequest(new { message = "Не вказано назву товару." });
            }

            // Зчитуємо кількість
            int quantity = 1;
            foreach (var key in new[] { "quantity", "Quantity", "qty" })
            {
                if (rawData.TryGetValue(key, out var val) && val != null)
                {
                    int.TryParse(val.ToString(), out quantity);
                    break;
                }
            }

            // Зчитуємо примітку
            string? note = null;
            if (rawData.TryGetValue("note", out var noteVal) || rawData.TryGetValue("Note", out noteVal))
            {
                note = noteVal?.ToString();
            }

            // 1. Пошук або створення товару у базі MySQL
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == itemName.Trim().ToLower());

            if (item == null)
            {
                item = new Item
                {
                    Name = itemName.Trim(),
                    Quantity = quantity > 0 ? quantity : 1,
                    Price = 100.00m,
                    Discount = 0.00m
                };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item.Quantity += quantity;
                _context.Items.Update(item);
            }

            // 2. Отримання або створення дефолтного працівника
            var worker = await _context.Workers.FirstOrDefaultAsync();
            if (worker == null)
            {
                worker = new Worker
                {
                    Name = "System Admin",
                    Username = "admin",
                    Email = "admin@wms.com",
                    PasswordHash = "0000",
                    Role = "Admin"
                };
                _context.Workers.Add(worker);
                await _context.SaveChangesAsync();
            }

            // 3. Запис у журнал операцій
            var userAction = new UserAction
            {
                WorkerId = worker.Id,
                ProductId = item.Id,
                ActionType = "Transfer",
                Quantity = quantity,
                PriceAtMoment = item.Price,
                Note = note,
                ActionDate = DateTime.Now
            };

            _context.UserActions.Add(userAction);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Операцію збережено!" });
        }
    }
}
