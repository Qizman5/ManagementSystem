using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/items")]
    [Tags("ItemsApi")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ItemsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetItems([FromQuery] int limit = 25, [FromQuery] int offset = 0)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = (double)i.Price,
                    Discount = (double)i.Discount,
                    FinalPrice = (double)(i.Price * (1m - (i.Discount / 100m)))
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/v1/items/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            var dto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = (double)item.Price,
                Discount = (double)item.Discount,
                FinalPrice = (double)(item.Price * (1m - (item.Discount / 100m)))
            };

            return Ok(dto);
        }

        // POST: api/v1/items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItem([FromBody] CreateItemDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = new Item
            {
                Name = dto.Name ?? string.Empty,
                Quantity = dto.Quantity,
                Price = (decimal)dto.Price,
                Discount = (decimal)dto.Discount
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var resultDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = (double)item.Price,
                Discount = (double)item.Discount,
                FinalPrice = (double)(item.Price * (1m - (item.Discount / 100m)))
            };

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, resultDto);
        }

        // PUT: api/v1/items/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ItemDto>> UpdateItem(int id, [FromBody] UpdateItemDto dto)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            item.Name = dto.Name ?? item.Name;
            item.Quantity = dto.Quantity;
            item.Price = (decimal)dto.Price;
            item.Discount = (decimal)dto.Discount;

            await _context.SaveChangesAsync();

            var resultDto = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Price = (double)item.Price,
                Discount = (double)item.Discount,
                FinalPrice = (double)(item.Price * (1m - (item.Discount / 100m)))
            };

            return Ok(resultDto);
        }

        // DELETE: api/v1/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO-моделі для Swagger документів
    public class CreateItemDto
    {
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }

    public class UpdateItemDto
    {
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public double FinalPrice { get; set; }
    }
}
