﻿using System;
using System.Collections.Generic;

namespace EphemeralCourage_API.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Rang { get; set; } = null!;

    public virtual Player IdNavigation { get; set; } = null!;
}
