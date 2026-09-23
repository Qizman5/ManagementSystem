using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.DTOs;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/items")]
    [Produces("application/json")]
    public class ItemsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsApiController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Отримати список усіх товарів (з підтримкою пагінації)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ItemDto>))]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems([FromQuery] int limit = 25, [FromQuery] int offset = 0)
        {
            if (limit > 100) limit = 100;

            var items = await _context.Items
                .Skip(offset)
                .Take(limit)
                .Select(item => new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount
                })
                .ToListAsync();

            return Ok(items);
        }

        /// <summary>
        /// Отримати конкретний товар за його ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemDto>> GetItemById(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound(new { message = $"Товар з ID {id} не знайдено." });
            }

            return Ok(new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                Discount = item.Discount
            });
        }

        /// <summary>
        /// Створити новий товар
        /// </summary>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = new Item
            {
                Name = dto.Name,
                Quantity = dto.Quantity,
                Price = dto.Price,
                Discount = dto.Discount
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var resultDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                Discount = item.Discount
            };

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, resultDto);
        }

        /// <summary>
        /// Оновити існуючий товар (PUT)
        /// </summary>
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemDto>> UpdateItem(int id, [FromBody] UpdateItemDto dto)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound(new { message = $"Товар з ID {id} не знайдено." });

            item.Name = dto.Name;
            item.Quantity = dto.Quantity;
            item.Price = dto.Price;
            item.Discount = dto.Discount;

            await _context.SaveChangesAsync();

            return Ok(new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = item.Price,
                Discount = item.Discount
            });
        }

        /// <summary>
        /// Видалити товар
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound(new { message = $"Товар з ID {id} не знайдено." });

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
