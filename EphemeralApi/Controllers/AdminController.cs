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

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            var admins = await _context.Admins.ToListAsync();
            return Ok(admins);
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevConsole(int id, [FromBody] DevConsoleUpdateDto updateDto)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound(new { message = "Admin not found" });
            }

            
            admin.DevConsole = updateDto.DevConsole;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTO osztály, hogy csak a DevConsole mező érkezzen be
   
}
