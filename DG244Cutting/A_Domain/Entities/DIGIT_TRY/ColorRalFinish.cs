using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Associates RAL color references with surface finish types (internal and external).
/// </summary>
public partial class ColorRalFinish
{
    public string Id { get; set; } = null!;

    /// <summary>
    /// RAL color reference for the internal face.
    /// </summary>
    public int IdInternalRal { get; set; }

    /// <summary>
    /// Finish reference for the internal face (mat, glossy, structured, etc.).
    /// </summary>
    public string IdInternalFinish { get; set; } = null!;

    public int IdExternalRal { get; set; }

    public string IdExternalFinish { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleInternal> ArticleInternals { get; set; } = new List<ArticleInternal>();

    public virtual ColorFinish IdExternalFinishNavigation { get; set; } = null!;

    public virtual ColorRal IdExternalRalNavigation { get; set; } = null!;

    public virtual ColorFinish IdInternalFinishNavigation { get; set; } = null!;

    public virtual ColorRal IdInternalRalNavigation { get; set; } = null!;
}
