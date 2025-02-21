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

        // GET: api/admin
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            var admins = await _context.Admins.ToListAsync();
            return Ok(admins);
        }

        // PUT: api/admin/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevConsole(int id, [FromBody] DevConsoleUpdateDto updateDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            // Csak a DevConsole mezőt frissítjük
            admin.DevConsole = updateDto.DevConsole;
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content (sikeres, de nincs visszatérő adat)
        }
    }

    // DTO osztály, hogy csak a DevConsole mező érkezzen be
   
}
