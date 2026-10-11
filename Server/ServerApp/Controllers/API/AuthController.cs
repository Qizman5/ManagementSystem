using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ServerApp.Models;

namespace ServerApp.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Необхідно вказати Email та Password.");

            // Шукаємо серед Workers замість застарілого User
            Worker? worker = await _context.Workers
                .FirstOrDefaultAsync(w => w.Email == dto.Email || w.Username == dto.Email);

            if (worker == null)
                return Unauthorized("Невірний Email або пароль.");

            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, worker.PasswordHash);
            }
            catch
            {
                isPasswordValid = worker.PasswordHash == dto.Password;
            }

            if (!isPasswordValid)
                return Unauthorized("Невірний Email або пароль.");

            var token = GenerateJwtToken(worker);

            return Ok(new
            {
                token,
                user = new
                {
                    worker.Id,
                    Name = worker.Name ?? worker.Username ?? "Worker",
                    worker.Email,
                    Role = worker.Role ?? "Worker"
                }
            });
        }

        private string GenerateJwtToken(Worker worker)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "SuperSecretKeyForWmsSystem2026Secure!");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, worker.Id.ToString()),
                new Claim(ClaimTypes.Name, worker.Name ?? worker.Username ?? "Worker"),
                new Claim(ClaimTypes.Email, worker.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, worker.Role ?? "Worker")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
