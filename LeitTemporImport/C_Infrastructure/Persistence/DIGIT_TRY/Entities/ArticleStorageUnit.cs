using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

/// <summary>
/// Defines the physical storage measurement units for articles (piece, meter, kg, etc.). IDs preserved from GestStock for compatibility.
/// </summary>
public partial class ArticleStorageUnit
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();
}
