using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/actions")]
    [AllowAnonymous] // Дозволяємо запити від мобільного застосунку
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] ActionDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.ItemName))
            {
                return BadRequest(new { message = "Некоректні дані товару." });
            }

            // 1. Шукаємо товар за назвою або створюємо новий
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name == dto.ItemName);

            if (item == null)
            {
                item = new Item
                {
                    Name = dto.ItemName,
                    Quantity = dto.Quantity > 0 ? dto.Quantity : 1,
                    Price = 100.00m,
                    Discount = 0.00m
                };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Оновлюємо кількість залежно від типу операції
                if (dto.ActionType == "Income" || dto.ActionType == "Прихід")
                {
                    item.Quantity += dto.Quantity;
                }
                else if (dto.ActionType == "Outcome" || dto.ActionType == "Списання")
                {
                    item.Quantity = Math.Max(0, item.Quantity - dto.Quantity);
                }
                _context.Items.Update(item);
            }

            // 2. Отримуємо першого доступного працівника для запису
            var worker = await _context.Workers.FirstOrDefaultAsync();
            if (worker == null)
            {
                return BadRequest(new { message = "У системі відсутні зареєстровані працівники." });
            }

            // 3. Фіксуємо операцію в історії
            var action = new UserAction
            {
                WorkerId = worker.Id,
                ProductId = item.Id,
                ActionType = dto.ActionType ?? "Transfer",
                Quantity = dto.Quantity,
                PriceAtMoment = item.Price,
                Note = dto.Note,
                ActionDate = DateTime.Now
            };

            _context.UserActions.Add(action);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Операцію успішно збережено!" });
        }
    }

    public class ActionDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
