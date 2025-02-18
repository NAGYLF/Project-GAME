using System;
using System.Collections.Generic;

namespace EphemeralApi.Models;

public partial class Player
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public virtual ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<Statistic> Statistics { get; set; } = new List<Statistic>();
}
