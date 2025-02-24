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

        // Konstruktor, amely befecskendezi az adatbázis kontextust a vezérlőbe
        public PlayerController(EphemeralCourageContext context)
        {
            _context = context;
        }

        // Lekéri az összes játékost az adatbázisból
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _context.Players.ToListAsync();
        }

        // Lekéri a játékos statisztikáit és teljesítményeit az ID alapján
        [HttpGet("stats/{playerId}")]
        public async Task<IActionResult> GetPlayerStats(int playerId)
        {
            var statistics = await _context.Statistics
                .Where(s => s.PlayerId == playerId)
                .FirstOrDefaultAsync();

            var achievements = await _context.Achievements
                .Where(a => a.PlayerId == playerId)
                .FirstOrDefaultAsync();

            // Ha a statisztika vagy a teljesítmény nem található, hibaüzenet visszaadása
            if (statistics == null || achievements == null)
            {
                return NotFound(new { message = "Nem található a játékos statisztikája ." });
            }

            // A statisztikák és teljesítmények DTO-ba történő leképezése
            var playerStatsDto = new PlayerStatsDto
            {
                
                DeathCount = statistics.DeathCount ?? 0,
                Score = statistics.Score ?? 0,
                EnemiesKilled = statistics.EnemiesKilled ?? 0,
                FirstBlood = achievements.FirstBlood ?? false,
                RookieWork = achievements.RookieWork ?? false,
                YouAreOnYourOwnNow = achievements.YouAreOnYourOwnNow ?? false
            };

            return Ok(playerStatsDto);
        }

        // Lekéri a játékos adatait az ID alapján
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

        // Lekéri a játékos adatait a neve alapján
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

        // Lekéri a játékos adatait az email és név kombinációja alapján
        [HttpGet("GetByEmailAndName")]
        public async Task<ActionResult<Player>> GetPlayerByEmailAndName([FromQuery] string email, [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Mind az email, mind a név megadása kötelező.");
            }

            var player = await _context.Players
                .Where(p => p.Email.ToLower() == email.ToLower() && p.Name.ToLower() == name.ToLower())
                .FirstOrDefaultAsync();

            if (player == null)
            {
                return NotFound("A játékos nem található.");
            }

            return Ok(player);
        }

        // A játékos adatainak frissítése az ID alapján, UpdatePlayerDto használatával az új adatokhoz
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
            player.IsBanned = playerDto.IsBanned;

            // Ha jelszó is van, akkor az új jelszót hash-eljük
            if (!string.IsNullOrWhiteSpace(playerDto.Password))
            {
                player.Password = BCrypt.Net.BCrypt.HashPassword(playerDto.Password);
            }

            _context.Players.Update(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // A játékos törlése az ID alapján, token validálással az engedélyezéshez
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayer(int id, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing.");
            }

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

        // A token validálása, hogy érvényes-e még
        private bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jsonToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null) return false;

                return jsonToken.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }

        // Generál egy időalapú egyszeri jelszót (TOTP)
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
