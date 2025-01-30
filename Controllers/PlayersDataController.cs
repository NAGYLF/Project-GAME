using Microsoft.AspNetCore.Mvc;
using EphemeralCourage_API.Models;
using static EphemeralCourage_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using System.Text.Json.Serialization;
using System.Text.Json;

[Route("UnityController")]
[ApiController]
public class PlayersDataController : ControllerBase
{
    [HttpGet("PlayerWebList")]// A Playerek Web oldalhoz tartozó adatait kérdezzük le pl(Id,Név,Email,Stb)
    public ActionResult<List<Player>> GetPlayers()
    {
        using (var context = new DatasDbContext())
        {
            var players = context.Players.ToList();
            return StatusCode(200, players);
        }
    }
    [HttpGet("PlayerStatisticsList")]// A Playerek Statisztikáit kérjük le (Megölt ellenfelek,Pont, Stb)
    public ActionResult<List<object>> GetStatisticsAndAchievements()
    {
        using (var context = new DatasDbContext())
        {
            var statisticsAndAchievements = context.Players
                .Select(p => new
                {
                    p.Id,
                    Achievement = p.Achievement,
                    Statistic = p.Statistic
                })
                .ToList();

            return StatusCode(200, statisticsAndAchievements);
        }
    }
    [HttpGet("PlayerWebListById")]// A Playerek Web oldalhoz tartozó adatait kérdezzük le Id alapján

    public ActionResult<Player> GetPlayerById(int id)
    {
        using (var context = new DatasDbContext())
        {
            var player = context.Players.FirstOrDefault(p => p.Id == id);

            if (player == null)
            {
                return NotFound();
            }

            return StatusCode(200, player);
        }
    }
    [HttpGet("PlayerStatisticsListById")]// A Playerek Statisztikáit kérjük le Id alapján
    public ActionResult<object> GetStatisticsAndAchievementsById(int id)
    {
        using (var context = new DatasDbContext())
        {
            var data = context.Players
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    Achievement = p.Achievement,
                    Statistic = p.Statistic
                })
                .FirstOrDefault();

            if (data == null)
            {
                return NotFound();
            }

            return StatusCode(200, data);
        }
    }
    [HttpGet("{name},{password}")]
    public ActionResult<Player> GetByNameAndPassword(string name, string password)
    {
        using (var context = new DatasDbContext())
        {
            var player = context.Players.FirstOrDefault(x => x.Name == name && x.Password == password);
            if (player == null)
            {
                return NotFound(new { message = "Player not found" });
            }
            return Ok(player);
        }
    }
    [HttpPost]
    public ActionResult<Player> Post(CreatePlayer createPlayerDto)
    {
        var Player = new Player
        {
            Name = createPlayerDto.Name,
            Email = createPlayerDto.Email,
            Password = createPlayerDto.Passworld,
            IsAdmin = createPlayerDto.IsAdmin
        };
        using (var context = new DatasDbContext())
        {
            context.Players.Add(Player);
            context.SaveChanges();
            return StatusCode(201, Player);
        }
    }

    [HttpPost("Add Dev Account")]
    public ActionResult<Player> Post()
    {
        var Player = new Player
        {
            Name = "TestPlayer",
            Email = "Test@gmail.com",
            Password = "TestAdmin",
            IsAdmin = true
        };
        using (var context = new DatasDbContext())
        {
            context.Players.Add(Player);
            context.SaveChanges();
            return StatusCode(201, Player);
        }
    }

    [HttpPut("{id}")]
    public ActionResult<Player> Put(int id, UpdatePlayer updatePlayerDto)
    {
        using (var context = new DatasDbContext())
        {
            var existingPlayer = context.Players.FirstOrDefault(x => x.Id == id);
            existingPlayer.Name = updatePlayerDto.Name;
            existingPlayer.Email = updatePlayerDto.Email;
            existingPlayer.Password = updatePlayerDto.Passworld;
            existingPlayer.IsAdmin = updatePlayerDto.IsAdmin;
            context.Players.Update(existingPlayer);
            context.SaveChanges();
            return StatusCode(200, existingPlayer);
        }
    }

    [HttpDelete("PlayerDelete")]
    public ActionResult<object> Delete(int id)
    {
        using (var context = new DatasDbContext())
        {
            var Tantargy = context.Players.FirstOrDefault(x => x.Id == id);
            if (Tantargy != null)
            {
                context.Players.Remove(Tantargy);
                context.SaveChanges();
                return StatusCode(200, new { message = "Sikeres törlés!" });
            }
        }
        return StatusCode(404, new { message = "Nincs ilyen player!" });
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
/*
Add-Migration CretateDatabase
Update-Database
*/