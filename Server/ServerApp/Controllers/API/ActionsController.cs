using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;

namespace ServerApp.Controllers.Api
{
    [ApiController]
    [Route("api/v1/actions")]
    [Authorize(Policy = "BearerOrCookie")]
    public class ActionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserAction>>> GetActions()
        {
            return await _context.UserActions
                .Include(a => a.Worker)
                .Include(a => a.Item)
                .OrderByDescending(a => a.ActionDate)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<UserAction>> CreateAction([FromBody] UserAction action)
        {
            if (action == null) return BadRequest("Дані операції відсутні.");

            _context.UserActions.Add(action);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetActions), new { id = action.Id }, action);
        }
    }
}
