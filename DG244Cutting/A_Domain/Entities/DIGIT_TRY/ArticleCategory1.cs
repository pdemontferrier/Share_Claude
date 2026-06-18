using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// First-level article classification reference.
/// </summary>
public partial class ArticleCategory1
{
    public short Id { get; set; }

    public string Designation { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();
}
