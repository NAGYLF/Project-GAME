﻿using System.IdentityModel.Tokens.Jwt;
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
        if (request.Name.Length > 10)
        {
            return BadRequest("A név nem lehet hosszabb 10 karakternél.");
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
            IsAdmin = request.IsAdmin
        };

        _context.Players.Add(newUser);
        await _context.SaveChangesAsync();

        // Ha admin felhasználót regisztrálunk, hozzunk létre egy Admin rekordot is
        if (request.IsAdmin)
        {
            var newAdmin = new Admin
            {
                PlayerId = newUser.Id, // A PlayerId és az Id most ugyanaz lesz
                DevConsole = false      // Alapértelmezetten false
            };

            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();
        }

        return Ok("Sikeres regisztráció.");
    }



    // Felhasználó bejelentkezése
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _context.Players.FirstOrDefaultAsync(p => p.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized("Invalid credentials.");

        string token = GenerateJwtToken(user);
        return Ok(new { Token = token });
    }

    // JWT token generálása bejelentkezés után
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
