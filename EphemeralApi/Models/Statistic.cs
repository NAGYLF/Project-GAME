using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EphemeralApi.Models;

public partial class Statistic
{
    public int Id { get; set; }

    [JsonIgnore]
    public int PlayerId { get; set; }

    public int? DeathCount { get; set; }

    public int? Score { get; set; }

    public int? EnemiesKilled { get; set; }

    [JsonIgnore]
    public virtual Player Player { get; set; } = null!;
}
