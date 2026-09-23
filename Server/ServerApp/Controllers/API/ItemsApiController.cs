using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/items")]
    [Produces("application/json")]
    [Authorize(Policy = "BearerOrCookie")]
    public class ItemsApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemsApiController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/items
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Item>))]
        public async Task<ActionResult<IEnumerable<Item>>> GetItems([FromQuery] int limit = 25, [FromQuery] int offset = 0)
        {
            if (limit > 100) limit = 100;

            return await _context.Items
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        // GET: api/v1/items/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> GetItemById(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound(new { message = $"Товар з ID {id} не знайдено." });
            }

            return Ok(item);
        }

        // POST: api/v1/items
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Item>> CreateItem([FromBody] Item item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, item);
        }

        // PUT: api/v1/items/5
        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Item))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Item>> UpdateItem(int id, [FromBody] Item item)
        {
            if (id != item.Id)
            {
                return BadRequest(new { message = "ID у шляху та у тілі запиту не збігаються." });
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
                    return NotFound(new { message = $"Товар з ID {id} не знайдено." });
                }
                throw;
            }

            return Ok(item);
        }

        // DELETE: api/v1/items/5
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
