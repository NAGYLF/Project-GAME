using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using EphemeralApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using EphemeralApi.Models.Dtos;
using Org.BouncyCastle.Crypto.Generators;
using System;

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
        if (await _context.Players.AnyAsync(p => p.Email == request.Email))
            return BadRequest("Email already in use.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new Player
        {
            Name = request.Name,
            Email = request.Email,
            Password = passwordHash,
            IsAdmin = request.IsAdmin
        };

        _context.Players.Add(newUser);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Players.FirstOrDefaultAsync(p => p.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized("Invalid credentials.");

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
            new Claim("Username", user.Name)
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
