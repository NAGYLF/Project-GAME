using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EphemeralApi.Models;

public partial class Achievement
{
    public int Id { get; set; }

    [JsonIgnore]
    public int PlayerId { get; set; } 

    public bool? FirstBlood { get; set; }

    public bool? RookieWork { get; set; }

    public bool? YouAreOnYourOwnNow { get; set; }

    [JsonIgnore]
    public virtual Player Player { get; set; } = null!;
}
