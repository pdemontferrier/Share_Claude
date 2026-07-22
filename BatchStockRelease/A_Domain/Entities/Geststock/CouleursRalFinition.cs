using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CouleursRalFinition
{
    public string Id { get; set; } = null!;

    public int IdCouleurRalInt { get; set; }

    public string IdCouleurFinitionInt { get; set; } = null!;

    public int IdCouleurRalExt { get; set; }

    public string IdCouleurFinitionExt { get; set; } = null!;

    public virtual CouleursFinition IdCouleurFinitionIntNavigation { get; set; } = null!;

    public virtual CouleursRal IdCouleurRalIntNavigation { get; set; } = null!;
}
