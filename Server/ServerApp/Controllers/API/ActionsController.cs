using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("StrictPolicy")] // Підключаємо захист Rate Limiter
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/actions
        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] UserAction actionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 1. Перевірка на від'ємне значення або нуль для ID товару та кількості
            if (actionModel.ItemId <= 0)
            {
                return BadRequest(new { message = "ID товару має бути додатним числом більше 0." });
            }

            if (actionModel.Quantity <= 0)
            {
                return BadRequest(new { message = "Кількість товару повинна бути більше 0." });
            }

            // 2. Пошук товару в базі даних
            var item = await _context.Items.FindAsync(actionModel.ItemId);
            if (item == null)
            {
                return NotFound(new { message = $"Товар з ID {actionModel.ItemId} не знайдено." });
            }

            var actionType = actionModel.ActionType?.Trim().ToLower();

            // 3. Обробка списання / відвантаження
            if (actionType == "outcome" || actionType == "списання" || actionType == "shipment")
            {
                if (item.Quantity < actionModel.Quantity)
                {
                    return BadRequest(new
                    {
                        message = "Недостатньо товару на складі для списання.",
                        availableQuantity = item.Quantity,
                        requestedQuantity = actionModel.Quantity
                    });
                }

                item.Quantity -= actionModel.Quantity;
                actionModel.ActionType = "Outcome";
            }
            // 4. Обробка приходу
            else if (actionType == "income" || actionType == "прихід" || actionType == "receipt")
            {
                item.Quantity += actionModel.Quantity;
                actionModel.ActionType = "Income";
            }
            else
            {
                return BadRequest(new { message = "Некоректний тип операції. Допустимі значення: 'Income' або 'Outcome'." });
            }

            // 5. Фіксація дати та збереження у БД
            actionModel.ActionDate = DateTime.UtcNow;

            _context.UserActions.Add(actionModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(CreateAction), new { id = actionModel.Id }, new
            {
                message = "Операцію успішно виконано.",
                actionId = actionModel.Id,
                newStockQuantity = item.Quantity
            });
        }
    }
}
