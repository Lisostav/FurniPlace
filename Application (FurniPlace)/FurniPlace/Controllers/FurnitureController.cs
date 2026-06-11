using FurnitureMarketplace.Data;
using FurnitureMarketplace.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureMarketplace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FurnitureController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FurnitureController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FurnitureItem>>> GetFurnitureItems([FromQuery] string? search, [FromQuery] string? category)
        {
            var query = _context.FurnitureItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f => f.Title.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "Всі")
            {
                query = query.Where(f => f.Category == category);
            }

            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<FurnitureItem>> CreateFurniture([FromForm] FurnitureItem item, IFormFile? image)
        {
            if (image != null && image.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                item.ImageUrl = "/images/" + fileName;
            }

            _context.FurnitureItems.Add(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<FurnitureItem>>> GetUserFurniture(int userId)
        {
            return await _context.FurnitureItems.Where(f => f.UserId == userId).ToListAsync();
        }

        [HttpDelete("{id}/user/{userId}")]
        public async Task<IActionResult> DeleteFurniture(int id, int userId)
        {
            var item = await _context.FurnitureItems.FindAsync(id);
            if (item == null) return NotFound();

            if (item.UserId != userId) return Forbid();

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", item.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

            _context.FurnitureItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}