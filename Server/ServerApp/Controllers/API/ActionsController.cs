using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] CreateUserActionDto dto)
        {
            if (dto == null) return BadRequest("Некоректні дані запиту.");

            // 1. Пошук товару
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == dto.ItemId)
                       ?? await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == dto.ItemName.ToLower());

            // 2. Перевірка: якщо товару немає, а виконується Витрата або Переміщення
            if (item == null && dto.ActionType != "Income")
            {
                return BadRequest($"Товар '{dto.ItemName}' не знайдено на складі. Спершу виконайте Прихід.");
            }

            // 3. Створення нового товару (якщо це Прихід)
            if (item == null)
            {
                item = new Item
                {
                    Name = dto.ItemName,
                    Description = string.IsNullOrWhiteSpace(dto.Note) ? "Додано через операцію" : dto.Note,
                    Quantity = dto.Quantity,
                    Price = 0,
                    Discount = 0
                };
                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 4. Оновлення кількості товару
                if (dto.ActionType == "Income")
                {
                    item.Quantity += dto.Quantity;
                }
                else if (dto.ActionType == "Expense" || dto.ActionType == "Transfer")
                {
                    if (item.Quantity < dto.Quantity)
                    {
                        return BadRequest($"Недостатньо товару '{item.Name}' на складі. Наявна кількість: {item.Quantity}.");
                    }
                    item.Quantity -= dto.Quantity;
                }
            }

            // 5. Логування дії
            var userAction = new UserAction
            {
                UserId = dto.UserId,
                ItemId = item.Id,
                ActionType = dto.ActionType,
                Quantity = dto.Quantity,
                Note = dto.Note
            };

            _context.UserActions.Add(userAction);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Операцію успішно виконано!", ItemId = item.Id });
        }
    }

    public class CreateUserActionDto
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
