using System;
using System.Collections.Generic;

namespace EphemeralApi.Models;

public partial class Admin
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public bool DevConsole { get; set; }

    public virtual Player Player { get; set; } = null!;
}
