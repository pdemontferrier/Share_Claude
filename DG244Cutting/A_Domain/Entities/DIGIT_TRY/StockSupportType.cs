using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Type de support physique permettant de stocker des articles, des conteneurs ou tout type de matériel dans l’entrepôt.
/// </summary>
public partial class StockSupportType
{
    public int Id { get; set; }

    /// <summary>
    /// Désignation du type de support (rack, étagère, casier, palette, box au sol, etc.).
    /// </summary>
    public string Designation { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<StockBin> StockBins { get; set; } = new List<StockBin>();
}
