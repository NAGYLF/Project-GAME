using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EphemeralApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using EphemeralApi.Models.Dtos;

namespace EphemeralApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly EphemeralCourageContext _context;

        public AdminController(EphemeralCourageContext context)
        {
            _context = context;
        }

        // Összes admin lekérése
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return Ok(await _context.Admins.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdminDto>> GetAdminById(int id)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Id == id); // Csak az Admin adatokat kérjük le

            if (admin == null)
            {
                return NotFound(new { message = "Nincs ilyen Admin" });
            }

            // Admin objektumot DTO-ra leképezés
            var adminDto = new AdminDto
            {
                Id = admin.Id,
                DevConsole = admin.DevConsole
            };

            return Ok(adminDto);
        }

        // Admin DevConsole beállításának frissítése
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevConsole(int id, [FromBody] DevConsoleUpdateDto updateDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound(new { message = "Admin not found" });

            admin.DevConsole = updateDto.DevConsole;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
