using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ArticleReference_Missing
{
    /// <summary>
    /// Référence article issue de Tempor_Import.CodeNat.
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Désignation article issue de Tempor_Import.Feld_10_100.
    /// </summary>
    public string? Designation { get; set; }

    /// <summary>
    /// Longueur standard de stockage issue de Tempor_Import.Wert_40.
    /// </summary>
    public decimal? StandardBarLengthMm { get; set; }

    /// <summary>
    /// Catégorie métier résolue depuis Feld_6 via ArticleCategoryMapping.
    /// </summary>
    public string? FamilyCategoryPrincipal { get; set; }

    /// <summary>
    /// Catégorie famille secondaire issue de Tempor_Import.Feld_40.
    /// </summary>
    public string? FamilyCategorySecondary { get; set; }

    /// <summary>
    /// Code famille issu de Tempor_Import.Feld_16.
    /// </summary>
    public string? CodeFamily { get; set; }

    /// <summary>
    /// Code article de référence issu de Tempor_Import.Feld_4.
    /// </summary>
    public string? CodeArticleReference { get; set; }

    /// <summary>
    /// Code article principal issu de Tempor_Import.Feld_41.
    /// </summary>
    public string? CodeArticle { get; set; }

    /// <summary>
    /// Code article machine de découpe issu de Tempor_Import.Feld_10_330.
    /// </summary>
    public string? CodeArticleCuttingMachine { get; set; }
}
