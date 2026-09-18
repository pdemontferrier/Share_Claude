using System;
using System.Collections.Generic;

namespace BatchCutting_DG.A_Domain.EntitiesGestStock;

public partial class UserAppPage
{
    public int Id { get; set; }

    public string Page { get; set; } = null!;

    public virtual ICollection<UserAppPageDroit> UserAppPageDroits { get; set; } = new List<UserAppPageDroit>();
}
