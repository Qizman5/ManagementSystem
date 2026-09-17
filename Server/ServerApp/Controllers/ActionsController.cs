using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

            if (actionModel.Quantity <= 0)
            {
                return BadRequest(new { message = "Кількість товару повинна бути більше 0." });
            }

            var item = await _context.Items.FindAsync(actionModel.ItemId);
            if (item == null)
            {
                return NotFound(new { message = $"Товар з ID {actionModel.ItemId} не знайдено." });
            }

            var actionType = actionModel.ActionType?.Trim().ToLower();

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
            else if (actionType == "income" || actionType == "прихід" || actionType == "receipt")
            {
                item.Quantity += actionModel.Quantity;
                actionModel.ActionType = "Income";
            }
            else
            {
                return BadRequest(new { message = "Некоректний тип операції. Допустимі значення: 'Income' або 'Outcome'." });
            }

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