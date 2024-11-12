﻿using Microsoft.AspNetCore.Mvc;
using EphemeralCourage_API.Models;
using static EphemeralCourage_API.Models.Dto;

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

    [HttpPost]
    public ActionResult<Player> Post(CreatePlayer createPlayerDto)
    {
        var Player = new Player
        {
            Name = createPlayerDto.Name,
            Email = createPlayerDto.Email,
            Password = createPlayerDto.Passworld,
            IsAdmin = createPlayerDto.IsAdmin,
        };

        using (var context = new DatasDbContext())
        {
            context.Players.Add(Player);
            context.SaveChanges();

            return StatusCode(201, Player);
        }
    }


    [HttpPost(" Add Dev Account")]
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
}
/*
Add-Migration CretateDatabase
Update-Database
*/