using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CouleursRal
{
    public int Id { get; set; }

    public string? DesignationAnglais { get; set; }

    public string? DesignationFrancais { get; set; }

    public string? CodeHexa { get; set; }

    public string? CodeRvb { get; set; }

    public string? CodeCmjn { get; set; }

    public string? ReferenceBase { get; set; }

    public virtual ICollection<CouleursRalFinition> CouleursRalFinitions { get; set; } = new List<CouleursRalFinition>();
}
