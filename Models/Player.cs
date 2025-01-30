using System;
using System.Collections.Generic;

namespace EphemeralCourage_API.Models;

public partial class Player
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public bool IsAdmin { get; set; }

    public virtual Achievement? Achievement { get; set; }

    public virtual Statistic? Statistic { get; set; }
}
