using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

/// <summary>
/// Third-level article classification reference.
/// </summary>
public partial class ArticleCategory3
{
    public short Id { get; set; }

    public string Designation { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();
}
