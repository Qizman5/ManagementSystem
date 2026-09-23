using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiExplorerSettings(IgnoreApi = true)] // <-- Приховує контролер і зв'язані з ним схеми зі Swagger
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [EnableRateLimiting("StrictPolicy")]
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/actions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAction>>> GetActions()
        {
            return await _context.UserActions.ToListAsync();
        }

        // POST: api/actions
        [HttpPost]
        public async Task<IActionResult> CreateAction([FromBody] UserAction actionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (actionModel.ItemId <= 0)
            {
                return BadRequest(new { message = "ID товару має бути додатним числом більше 0." });
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

            return StatusCode(201, new
            {
                message = "Операцію успішно виконано.",
                actionId = actionModel.Id,
                newStockQuantity = item.Quantity
            });
        }
    }
}
