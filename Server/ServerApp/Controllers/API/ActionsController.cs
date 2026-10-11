using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System.Text.Json.Serialization;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/actions")]
    [AllowAnonymous] // Дозволяємо мобільному клієнту надсилати операції без обов'язкового токена
    [ApiExplorerSettings(IgnoreApi = true)]
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
            // Отримуємо назву товару з будь-якого з можливих полів DTO
            var rawItemName = dto?.ItemName ?? dto?.ProductName ?? dto?.Name ?? dto?.Title;

            if (dto == null || string.IsNullOrWhiteSpace(rawItemName))
            {
                return BadRequest(new { success = false, message = "Вкажіть назву товару." });
            }

            var cleanName = rawItemName.Trim();

            // 1. Шукаємо товар за назвою або створюємо новий у БД
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Name.ToLower() == cleanName.ToLower());

            if (item == null)
            {
                item = new Item
                {
                    Name = cleanName,
                    Quantity = dto.Quantity > 0 ? dto.Quantity : 1,
                    Price = 100.00m,
                    Discount = 0.00m
                };
                _context.Items.Add(item);
                await _context.SaveChangesAsync(); // 👈 Новий товар додається в MySQL!
            }
            else
            {
                var actionTypeLower = (dto.ActionType ?? "").Trim().ToLower();

                // Якщо це прихід або переміщення — збільшуємо кількість
                if (actionTypeLower.Contains("income") || actionTypeLower.Contains("прихід") || actionTypeLower.Contains("transfer"))
                {
                    item.Quantity += dto.Quantity;
                }
                else if (actionTypeLower.Contains("outcome") || actionTypeLower.Contains("списання"))
                {
                    item.Quantity = Math.Max(0, item.Quantity - dto.Quantity);
                }
                _context.Items.Update(item);
            }

            // 2. Отримуємо працівника для логування
            var worker = await _context.Workers.FirstOrDefaultAsync() ?? new Worker { Name = "System Admin", Email = "admin@wms.com", PasswordHash = "0000" };

            // 3. Записуємо операцію в журнал UserActions
            var action = new UserAction
            {
                WorkerId = worker.Id,
                ProductId = item.Id,
                ActionType = string.IsNullOrWhiteSpace(dto.ActionType) ? "Transfer" : dto.ActionType,
                Quantity = dto.Quantity,
                PriceAtMoment = item.Price,
                Note = dto.Note,
                ActionDate = DateTime.Now
            };

            _context.UserActions.Add(action);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Операцію успішно збережено в базі даних!" });
        }
    }

    public class ActionDto
    {
        [JsonPropertyName("itemName")]
        public string? ItemName { get; set; }

        [JsonPropertyName("productName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("actionType")]
        public string? ActionType { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }
    }
}
