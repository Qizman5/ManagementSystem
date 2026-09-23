using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    /// <summary>
    /// RESTful API для управління ресурсом 'items' (Товари складе)
    /// Відповідає специфікації OpenAPI та стандартам Integrate.io
    /// </summary>
    [ApiController]
    [Route("api/v1/items")]
    [Produces("application/json")]
    [Consumes("application/json")]
    [Authorize(Policy = "BearerOrCookie")]
    public class ItemsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// READ (List): Отримати список товарів з фільтрацією та пагінацією
        /// GET /api/v1/items?search=name&limit=25&offset=0
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Item>))]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems(
            [FromQuery] string? search = null,
            [FromQuery] int limit = 25,
            [FromQuery] int offset = 0)
        {
            if (limit > 100) limit = 100;

            var query = _context.Items.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(i => i.Name.Contains(search) || i.Description.Contains(search));
            }

            var items = await query.Skip(offset).Take(limit).ToListAsync();
            return Ok(items);
        }

        /// <summary>
        /// READ (Single): Отримати конкретний ресурс за його ID
        /// GET /api/v1/items/{id}
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound(new { error = "ResourceNotFound", message = $"Товар з ID {id} не знайдено." });
            }

            return Ok(item);
        }

        /// <summary>
        /// CREATE: Створити новий ресурс
        /// POST /api/v1/items
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Item>> CreateItem([FromBody] Item item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            // Повертає статус 201 Created та заголовок 'Location' із посиланням на новий ресурс
            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        /// <summary>
        /// UPDATE (Full): Повне оновлення ресурсу
        /// PUT /api/v1/items/{id}
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> UpdateItem(int id, [FromBody] Item item)
        {
            if (id != item.Id)
            {
                return BadRequest(new { error = "MismatchedId", message = "ID у маршруті та у тілі запиту не збігаються." });
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Items.Any(e => e.Id == id))
                {
                    return NotFound(new { error = "ResourceNotFound", message = $"Товар з ID {id} не знайдено." });
                }
                throw;
            }

            return Ok(item);
        }

        /// <summary>
        /// DELETE: Видалення ресурсу
        /// DELETE /api/v1/items/{id}
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound(new { error = "ResourceNotFound", message = $"Товар з ID {id} не знайдено." });
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            // За стандартом REST відповідає 204 No Content при успішному видаленні
            return NoContent();
        }
    }
}
