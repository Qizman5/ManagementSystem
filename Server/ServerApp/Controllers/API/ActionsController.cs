using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Data;
using ServerApp.Models;

namespace ServerApp.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] CreateUserActionDto dto)
        {
            if (dto == null) return BadRequest("Некоректні дані");

            // 1. Шукаємо товар за ID або за назвою
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == dto.ItemId)
                       ?? await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == dto.ItemName.ToLower());

            // 2. Якщо товару немає — автоматично створюємо його в warehouse_db
            if (item == null)
            {
                if (string.IsNullOrWhiteSpace(dto.ItemName))
                {
                    return BadRequest("Назву товару не вказано");
                }

                item = new Item
                {
                    Name = dto.ItemName,
                    Description = string.IsNullOrWhiteSpace(dto.Note) ? "Створено через операцію" : dto.Note,
                    Quantity = dto.ActionType == "Income" ? dto.Quantity : 0,
                    Price = 0,
                    Discount = 0
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Оновлюємо кількість товару на складі
                if (dto.ActionType == "Income")
                {
                    item.Quantity += dto.Quantity;
                }
                else if (dto.ActionType == "Expense")
                {
                    if (item.Quantity < dto.Quantity)
                    {
                        return BadRequest($"Недостатньо товару на складі. Доступно: {item.Quantity}");
                    }
                    item.Quantity -= dto.Quantity;
                }
            }

            // 3. Записуємо дію
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

            return Ok(new { Message = "Операцію успішно збережено", ItemId = item.Id });
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
