using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ArticleReference_Missing
{
    /// <summary>
    /// Référence article issue de Tempor_Import.Feld_10_066.
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Désignation article issue de Tempor_Import.Feld_10_100.
    /// </summary>
    public string? Designation { get; set; }

    public decimal? StandardBarLengthMm { get; set; }

    public string? FamilyCategory { get; set; }

    public string? CodeFamily { get; set; }

    public string? CodeArticleReference { get; set; }

    public string? CodeArticle { get; set; }

    public string? CodeArticleCuttingMachine { get; set; }
}
