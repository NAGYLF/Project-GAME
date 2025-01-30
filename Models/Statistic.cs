using System;
using System.Collections.Generic;

namespace EphemeralCourage_API.Models;

public partial class Statistic
{
    public int Id { get; set; }

    public int Score { get; set; }

    public int EnemiesKilled { get; set; }

    public int TimePlayed { get; set; }

    public int DeathCount { get; set; }

    public int PlayerId { get; set; }

    public virtual Player IdNavigation { get; set; } = null!;
}
