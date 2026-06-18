using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table defining how each article is physically identified (piece, bar, box, etc.).
/// </summary>
public partial class ArticleIdentificationType
{
    public short Id { get; set; }

    public string Code { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();
}
