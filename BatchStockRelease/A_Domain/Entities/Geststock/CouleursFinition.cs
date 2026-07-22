using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CouleursFinition
{
    public string Id { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public virtual ICollection<CouleursRalFinition> CouleursRalFinitions { get; set; } = new List<CouleursRalFinition>();
}
