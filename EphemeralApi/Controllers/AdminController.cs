using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EphemeralApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using EphemeralApi.Models.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System;

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
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins([FromQuery] string token)
        {
            if (!ValidateToken(token)) return Unauthorized("Heltelytelen vagy hiányzó token.");

            return Ok(await _context.Admins.ToListAsync());
        }

        // Egy adott admin lekérése ID alapján
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminDto>> GetAdminById(int id, [FromQuery] string token)
        {
            if (!ValidateToken(token)) return Unauthorized("Invalid or missing token.");

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
        public async Task<IActionResult> UpdateDevConsole(int id, [FromBody] DevConsoleUpdateDto updateDto, [FromQuery] string token)
        {
            if (!ValidateToken(token)) return Unauthorized("Heltelytelen vagy hiányzó token.");

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
                return NotFound(new { message = "Nincs ilyen Admin" });

            admin.DevConsole = updateDto.DevConsole;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // A token validálása, hogy érvényes-e még
        private bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return false;

                return jwtToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
