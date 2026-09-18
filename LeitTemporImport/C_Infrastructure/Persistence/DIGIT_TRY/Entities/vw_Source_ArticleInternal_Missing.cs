using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

public partial class vw_Source_ArticleInternal_Missing
{
    public int IdArticleReference { get; set; }

    public string IdColorRalFinish { get; set; } = null!;

    public string? ArticleReferenceCode { get; set; }

    public string? ColorCodeInOut { get; set; }

    public decimal? StandardBarLengthMm { get; set; }
}
