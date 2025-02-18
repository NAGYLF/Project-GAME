using EphemeralApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EphemeralApi.Models.Dtos;
using System.Net.Mail;
using MailKit.Net.Smtp;
using OtpNet;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace EphemeralApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly EphemeralCourageContext _context;

        public PlayerController(EphemeralCourageContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        [HttpGet("stats/{playerId}")]
        public async Task<IActionResult> GetPlayerStats(int playerId)
        {
            var statistics = await _context.Statistics
                .Where(s => s.PlayerId == playerId)
                .FirstOrDefaultAsync();

            var achievements = await _context.Achievements
                .Where(a => a.PlayerId == playerId)
                .FirstOrDefaultAsync();

            if (statistics == null || achievements == null)
            {
                return NotFound(new { message = "Nem található a játékos statisztikája ." });
            }

            var playerStatsDto = new PlayerStatsDto
            {
                PlayerId = playerId,
                DeathCount = statistics.DeathCount ?? 0,
                Score = statistics.Score ?? 0,
                EnemiesKilled = statistics.EnemiesKilled ?? 0,
                FirstBlood = achievements.FirstBlood ?? false,
                RookieWork = achievements.RookieWork ?? false,
                YouAreOnYourOwnNow = achievements.YouAreOnYourOwnNow ?? false
            };

            return Ok(playerStatsDto);
        }

        [HttpGet("/GetbyId/{id}")]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            var player = await _context.Players.FindAsync(id);

            if (player == null)
            {
                return NotFound(new { message = "A játékos nem található." });
            }

            return Ok(player);
        }

        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<Player>> GetPlayerByName(string name)
        {
            var player = await _context.Players
                .Where(p => p.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return NotFound("A játékos nem található.");
            }

            return Ok(player);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlayer(int id, UpdatePlayerDto playerDto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            player.Name = playerDto.Name;
            player.Email = playerDto.Email;
            player.IsAdmin = playerDto.IsAdmin;

            // Ha a felhasználó megadott új jelszót, hasheljük
            if (!string.IsNullOrWhiteSpace(playerDto.Password))
            {
                player.Password = BCrypt.Net.BCrypt.HashPassword(playerDto.Password); // Hashelés BCrypt-tel
            }

            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing.");
            }

            // Ellenőrzés: Token validálása
            var isValid = ValidateToken(token);

            if (!isValid)
            {
                return Unauthorized("Invalid token.");
            }

            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ValidateToken(string token)
        {
            // Token validálás logikája
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null) return false;

                return jsonToken.ValidTo > DateTime.UtcNow; // Lejárati idő ellenőrzése
            }
            catch
            {
                return false;
            }
        }



        [HttpGet("code")]
        public ActionResult<object> GetTotpCode()
        {
            var base32Secret = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            var secretBytes = Base32Encoding.ToBytes(base32Secret);
            var totp = new Totp(secretBytes, step: 30, totpSize: 6);
            var code = totp.ComputeTotp();
            var step = 30;
            var unixTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            var secondsLeft = step - (unixTime % step);
            return Ok(new { code, secondsLeft });
        }
    }
}
