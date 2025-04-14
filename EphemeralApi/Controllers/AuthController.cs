using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using EphemeralApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using EphemeralApi.Models.Dtos;
using System.Text.RegularExpressions;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly EphemeralCourageContext _context;
    private readonly IConfiguration _config;

    public AuthController(EphemeralCourageContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreatePlayerDto request)
    {
        // Email formátum ellenőrzése
        if (!request.Email.Contains("@") || !request.Email.Contains("."))
        {
            return BadRequest("Hibás Email. Ügyeljen a @-ra és a . karakterre.");
        }

        // Jelszó komplexitás ellenőrzése (kis- és nagybetű, szám, speciális karakter)
        var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$");
        if (!passwordRegex.IsMatch(request.Password))
        {
            return BadRequest("Hibás jelszó. Ügyeljen ezekre: legyen benne kis és nagy betű, szám és speciális karakter.");
        }

        // Minimális jelszóhossz ellenőrzés
        if (request.Password.Length < 7)
        {
            return BadRequest("A jelszónak legalább 7 karakter hosszúnak kell lennie.");
        }

        // Maximális névhossz ellenőrzés
        if (request.Name.Length > 30)
        {
            return BadRequest("A név nem lehet hosszabb 30 karakternél.");
        }

        // Ellenőrizzük, hogy az email már létezik-e
        if (await _context.Players.AnyAsync(p => p.Email == request.Email))
        {
            return BadRequest("Ezt az emailt már regisztrálták.");
        }

        // Jelszó hash-elése
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Új felhasználó létrehozása
        var newUser = new Player
        {
            Name = request.Name,
            Email = request.Email,
            Password = passwordHash,
            IsAdmin = request.IsAdmin,
            IsBanned = request.IsBanned,
        };

        _context.Players.Add(newUser);
        await _context.SaveChangesAsync(); // Itt kapja meg az Id-t (pl. 24)

        // Achievement rekord – ugyanazzal az Id-vel
        var newAchievement = new Achievement
        {
            Id = newUser.Id,
            PlayerId = newUser.Id,
            FirstBlood = false,
            RookieWork = false,
            YouAreOnYourOwnNow = false
        };
        _context.Achievements.Add(newAchievement);

        // Statistic rekord – ugyanazzal az Id-vel
        var newStatistic = new Statistic
        {
            Id = newUser.Id,
            PlayerId = newUser.Id,
            DeathCount = 0,
            Score = 0,
            EnemiesKilled = 0
        };
        _context.Statistics.Add(newStatistic);

        // Admin rekord, ha admin a felhasználó
        if (request.IsAdmin)
        {
            var newAdmin = new Admin
            {
                Id = newUser.Id,
                PlayerId = newUser.Id,
                DevConsole = false
            };
            _context.Admins.Add(newAdmin);
        }

        await _context.SaveChangesAsync();

        return Ok("Sikeres regisztráció.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Players.FirstOrDefaultAsync(p => p.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized("Hibás bejelentkezési adatok.");

        string token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(Player user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.Email),
            new Claim("IsAdmin", user.IsAdmin.ToString()),
            new Claim("Username", user.Name),
            new Claim("IsBanned", user.IsBanned.ToString()),
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
