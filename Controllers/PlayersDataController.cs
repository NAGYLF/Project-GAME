using Microsoft.AspNetCore.Mvc;
using EphemeralCourage_API.Models;
using static EphemeralCourage_API.Models.Dto;
using Microsoft.EntityFrameworkCore;
using OtpNet;

[Route("UnityController")]
[ApiController]
public class PlayersDataController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Player>> Get()
    {
        using (var context = new DatasDbContext())
        {
            return StatusCode(201, context.Players.ToList());
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

    [HttpDelete]
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