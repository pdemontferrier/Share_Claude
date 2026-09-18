using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class UserAppPageDroit
{
    public int Id { get; set; }

    public int IdUser { get; set; }

    public int IdApp { get; set; }

    public string Page { get; set; } = null!;

    public bool UserCanAccess { get; set; }

    public bool UserCanRead { get; set; }

    public bool UserCanUpdate { get; set; }

    public bool UserCanCreate { get; set; }

    public bool UserCanDelete { get; set; }

    public bool UserCanControl { get; set; }

    public bool UserCanValidate { get; set; }

    public bool UserCanSupervise { get; set; }

    public bool UserCanMonitor { get; set; }

    public bool UserCanAdmin { get; set; }

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual UserAppPage PageNavigation { get; set; } = null!;
}
