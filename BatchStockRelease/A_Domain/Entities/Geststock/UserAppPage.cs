using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserAppPage
{
    public int Id { get; set; }

    public string Page { get; set; } = null!;

    public virtual ICollection<UserAppPageDroit> UserAppPageDroits { get; set; } = new List<UserAppPageDroit>();
}
