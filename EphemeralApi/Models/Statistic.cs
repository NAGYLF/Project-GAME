using System;
using System.Collections.Generic;

namespace EphemeralApi.Models;

public partial class Statistic
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public int? DeathCount { get; set; }

    public int? Score { get; set; }

    public int? EnemiesKilled { get; set; }

    public virtual Player Player { get; set; } = null!;
}
