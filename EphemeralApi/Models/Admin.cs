using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EphemeralApi.Models;

public partial class Admin
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public bool DevConsole { get; set; }


    [JsonIgnore]
    public virtual Player Player { get; set; } = null!;
}
