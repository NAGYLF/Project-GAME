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

            // Ellenőrizzük, hogy a játékos admin-e
            if (player.IsAdmin)
            {
                // Ha admin, lekérdezzük az admin táblából az admin adatokat
                var admin = await _context.Admins
                    .Where(a => a.PlayerId == player.Id)
                    .FirstOrDefaultAsync();

                if (admin != null)
                {
                    // Ha van admin rekord, akkor hozzáadjuk az admin adatokat a válaszhoz
                    var playerWithAdmin = new
                    {
                        Player = player,
                        AdminDetails = admin
                    };

                    // JSON serializálás, ciklikus hivatkozások kezelése
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        WriteIndented = true // Formázott JSON válasz
                    };

                    return Ok(JsonSerializer.Serialize(playerWithAdmin, options));
                }
            }

            // Ha nem admin, akkor csak a játékos adatokat adjuk vissza
            return Ok(player);
        }



        [HttpGet("GetByToken")]
        public async Task<IActionResult> GetPlayerByToken([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("A token megadása kötelező.");
            }

            try
            {
                // JWT token olvasása
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    return Unauthorized("Érvénytelen token.");
                }

                // Felhasználói azonosító (UserId) kinyerése a tokenből
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized("A token nem tartalmaz érvényes UserId-t.");
                }

                // Játékos keresése a Player táblában
                var player = await _context.Players.FindAsync(userId);

                if (player == null)
                {
                    return NotFound("A játékos nem található.");
                }

                // Admin rekord keresése, ha létezik a játékoshoz
                var admin = await _context.Admins
                    .Where(a => a.PlayerId == player.Id)
                    .FirstOrDefaultAsync();

                var statistics = await _context.Statistics
                    .Where(s => s.PlayerId == player.Id)
                    .FirstOrDefaultAsync();

                var achievements = await _context.Achievements
                    .Where(a => a.PlayerId == player.Id)
                    .FirstOrDefaultAsync();

                // Ha találunk admin rekordot, akkor visszaadjuk a játékos és admin adatokat együtt
                if (statistics == null)
                {
                    return NotFound("A játékos statisztikái nem találhatóak.");
                }
                if (achievements == null)
                {
                    return NotFound("A játékos teljesítményei nem találhatóak.");
                }

                player.Statistics = new List<Statistic>() { statistics };
                player.Achievements = new List<Achievement>() { achievements };

                if (admin != null)
                {
                    player.Admin = admin;
                }

                return Ok(player);
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba történt a token feldolgozása közben: {ex.Message}");
            }
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
        [HttpPut("{id}/ban")]
        public async Task<IActionResult> UpdatePlayerBan(int id, UpdatePlayerBanDto banDto)
        {
            var player = await _context.Players.FindAsync(id);
            if (player == null)
            {
                return NotFound();
            }

            // Csak az IsBanned mezőt frissítjük
            player.IsBanned = banDto.IsBanned;

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

        [HttpPut("stats")]
        public async Task<IActionResult> UpdatePlayerStatisticsByToken([FromQuery] string token, [FromBody] UpdateStatisticsDto statsDto)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("A token megadása kötelező.");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return Unauthorized("Érvénytelen token.");

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("A token nem tartalmaz érvényes UserId-t.");

                var statistics = await _context.Statistics
                    .Where(s => s.PlayerId == userId)
                    .FirstOrDefaultAsync();

                if (statistics == null)
                    return NotFound(new { message = "A játékos statisztikái nem találhatóak." });

                statistics.DeathCount = statsDto.DeathCount;
                statistics.Score = statsDto.Score;
                statistics.EnemiesKilled = statsDto.EnemiesKilled;

                _context.Statistics.Update(statistics);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba történt a token feldolgozása közben: {ex.Message}");
            }
        }

        [HttpPut("achievements")]
        public async Task<IActionResult> UpdatePlayerAchievementsByToken([FromQuery] string token, [FromBody] UpdateAchievementsDto achievementsDto)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("A token megadása kötelező.");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return Unauthorized("Érvénytelen token.");

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId");

                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized("A token nem tartalmaz érvényes UserId-t.");

                var achievements = await _context.Achievements
                    .Where(a => a.PlayerId == userId)
                    .FirstOrDefaultAsync();

                if (achievements == null)
                    return NotFound(new { message = "A játékos teljesítményei nem találhatóak." });

                achievements.FirstBlood = achievementsDto.FirstBlood;
                achievements.RookieWork = achievementsDto.RookieWork;
                achievements.YouAreOnYourOwnNow = achievementsDto.YouAreOnYourOwnNow;

                _context.Achievements.Update(achievements);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba történt a token feldolgozása közben: {ex.Message}");
            }
        }



    }
}
