using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Articles internes : variante physique/couleur d’une référence, utilisée pour le stock et la production.
/// </summary>
public partial class ArticleInternal
{
    /// <summary>
    /// Identifiant interne unique de l’article interne (PK).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la référence article (FK ArticleReference.Id).
    /// </summary>
    public int IdArticleReference { get; set; }

    /// <summary>
    /// Identifiant couleur/finition (FK ColorRalFinish.Id).
    /// </summary>
    public string? IdColorRalFinish { get; set; }

    /// <summary>
    /// Longueur de stockage de référence de l’article, exprimée en millimètres.
    /// </summary>
    public double? StandardBarLengthMm { get; set; }

    /// <summary>
    /// Indique si l’article interne est géré avec suivi des chutes.
    /// </summary>
    public bool ManageScraps { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleInternalConsumption> ArticleInternalConsumptions { get; set; } = new List<ArticleInternalConsumption>();

    public virtual ICollection<CuttingScrapArchive> CuttingScrapArchives { get; set; } = new List<CuttingScrapArchive>();

    public virtual ICollection<CuttingScrapStock> CuttingScrapStocks { get; set; } = new List<CuttingScrapStock>();

    public virtual ArticleReference IdArticleReferenceNavigation { get; set; } = null!;

    public virtual ColorRalFinish? IdColorRalFinishNavigation { get; set; }

    public virtual ICollection<ProductionCutPiece> ProductionCutPieces { get; set; } = new List<ProductionCutPiece>();

    public virtual ICollection<StockBinItem> StockBinItems { get; set; } = new List<StockBinItem>();
}
