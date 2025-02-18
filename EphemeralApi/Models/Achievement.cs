using System;
using System.Collections.Generic;

namespace EphemeralApi.Models;

public partial class Achievement
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public bool? FirstBlood { get; set; }

    public bool? RookieWork { get; set; }

    public bool? YouAreOnYourOwnNow { get; set; }

    public virtual Player Player { get; set; } = null!;
}
