using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using EphemeralCourage_API;
using EphemeralCourage_API.Models;

[Route("UnityController")]
[ApiController]
public class ProductController : ControllerBase
{
    Connect conn = new();

    [HttpGet]
    public List<Player> Get()
    {

        List<Player> products = new List<Player>();

        conn.Connection.Open();
        string sql = "SELECT * FROM players";

        MySqlCommand cmd = new MySqlCommand(sql, conn.Connection);
        MySqlDataReader reader = cmd.ExecuteReader();
        reader.Read();
        do
        {
            var result = new Player()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Password = reader.GetString(3),
            };
            products.Add(result);
        }
        while (reader.Read());

        conn.Connection.Close();
        return products;
    }
}
