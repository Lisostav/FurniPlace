using FurnitureMarketplace.Data;
using FurnitureMarketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureMarketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context) => _context = context;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Login == user.Login))
                return BadRequest("Користувач з таким логіном вже існує!");

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { userId = user.Id, login = user.Login });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginData)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Login == loginData.Login && u.Password == loginData.Password);

            if (user == null)
                return Unauthorized("Невірний логін або пароль");

            return Ok(new { userId = user.Id, login = user.Login });
        }
    }
}