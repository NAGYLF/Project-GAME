using System;
using System.Collections.Generic;

namespace EphemeralCourage_API.Models;

public partial class Newplayer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsAdmin { get; set; }
}
