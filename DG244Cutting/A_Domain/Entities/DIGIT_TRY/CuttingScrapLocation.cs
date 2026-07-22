using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Defines storage locations for cutting scraps (horizontal or vertical).
/// </summary>
public partial class CuttingScrapLocation
{
    public int Id { get; set; }

    public string Designation { get; set; } = null!;

    /// <summary>
    /// Indicates whether the scrap location is horizontal (1) or not (0).
    /// </summary>
    public bool IsHorizontal { get; set; }

    /// <summary>
    /// Indicates whether the scrap location is vertical (1) or not (0).
    /// </summary>
    public bool IsVertical { get; set; }

    public int MaxQuantity { get; set; }

    public int? OrderIndex { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferenceIdScrapLocationHorizontalNavigations { get; set; } = new List<ArticleReference>();

    public virtual ICollection<ArticleReference> ArticleReferenceIdScrapLocationVerticalNavigations { get; set; } = new List<ArticleReference>();

    public virtual ICollection<CuttingScrapStock> CuttingScrapStocks { get; set; } = new List<CuttingScrapStock>();
}
