using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Stores available finish types for aluminium profiles (mat, glossy, structured, anodized, etc.).
/// </summary>
public partial class ColorFinish
{
    /// <summary>
    /// Unique code identifying the surface finish (e.g., G for Glossy, M for Mat).
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Name or description of the surface finish.
    /// </summary>
    public string Designation { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ColorRalFinish> ColorRalFinishIdExternalFinishNavigations { get; set; } = new List<ColorRalFinish>();

    public virtual ICollection<ColorRalFinish> ColorRalFinishIdInternalFinishNavigations { get; set; } = new List<ColorRalFinish>();
}
