using System;
using System.Collections.Generic;

namespace EphemeralCourage_API.Models;

public partial class Achievement
{
    public int Id { get; set; }

    public bool FirstBlood { get; set; }

    public bool RookieWork { get; set; }

    public bool YoureOnYourOwnNow { get; set; }

    public int PlayerId { get; set; }

    public virtual Player IdNavigation { get; set; } = null!;
}
