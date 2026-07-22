using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Définit les références articles.
/// </summary>
public partial class ArticleReference
{
    /// <summary>
    /// Clé primaire de la référence article.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Clé étrangère : catégorie principale de l’article (Catégorie 1).
    /// </summary>
    public short IdArticleCategory1 { get; set; }

    /// <summary>
    /// Clé étrangère : catégorie secondaire de l’article (Catégorie 2).
    /// </summary>
    public short IdArticleCategory2 { get; set; }

    /// <summary>
    /// Clé étrangère : catégorie tertiaire de l’article (Catégorie 3).
    /// </summary>
    public short IdArticleCategory3 { get; set; }

    /// <summary>
    /// Clé étrangère : machine de découpe associée à la référence.
    /// </summary>
    public int IdCuttingMachine { get; set; }

    /// <summary>
    /// Clé étrangère : type d’identification (pièce, barre, carton…).
    /// </summary>
    public short IdArticleIdentificationType { get; set; }

    /// <summary>
    /// Clé étrangère : unité de stockage (mètre, pièce, carton…).
    /// </summary>
    public int? IdArticleStorageUnit { get; set; }

    /// <summary>
    /// Clé étrangère : emplacement horizontal par défaut des chutes.
    /// </summary>
    public int IdScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Clé étrangère : emplacement vertical par défaut des chutes.
    /// </summary>
    public int IdScrapLocationVertical { get; set; }

    /// <summary>
    /// Clé étrangère optionnelle : fournisseur associé à la référence.
    /// </summary>
    public int? IdSupplier { get; set; }

    /// <summary>
    /// Code alphanumérique unique identifiant la référence article issu du champ Tempor_Import.CodeNat.
    /// </summary>
    public string Reference { get; set; } = null!;

    /// <summary>
    /// Désignation lisible et descriptive de la référence article issu du champ Tempor_Import.Feld_10_100.
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Catégorie famille de la référence article, issue du champ Tempor_Import.Feld_40.
    /// </summary>
    public string? FamilyCategory { get; set; }

    /// <summary>
    /// Code famille de la référence article, issu du champ Tempor_Import.Feld_16.
    /// </summary>
    public string? CodeFamily { get; set; }

    /// <summary>
    /// Code article de référence issu du champ Tempor_Import.Feld_4.
    /// </summary>
    public string? CodeArticleReference { get; set; }

    /// <summary>
    /// Code article principal issu du champ Tempor_Import.Feld_41.
    /// </summary>
    public string? CodeArticle { get; set; }

    /// <summary>
    /// Code article spécifique à la machine de découpe, issu du champ Tempor_Import.Feld_10_330.
    /// </summary>
    public string? CodeArticleCuttingMachine { get; set; }

    /// <summary>
    /// Longueur standard de stockage de l’article, en millimètres issu du champ Tempor_Import.Wert_40.
    /// </summary>
    public decimal? StandardBarLengthMm { get; set; }

    /// <summary>
    /// Largeur de la barre en millimètres issu du champ Tempor_Import.Feld_10_051.
    /// </summary>
    public decimal? BarWidthMm { get; set; }

    /// <summary>
    /// Hauteur de la barre en millimètres issu du champ Tempor_Import.Feld_10_075.
    /// </summary>
    public decimal? BarHeightMm { get; set; }

    /// <summary>
    /// Indique si la gestion des chutes est active pour cette référence.
    /// </summary>
    public bool ManageScraps { get; set; }

    /// <summary>
    /// Longueur minimale en millimètres au-delà de laquelle une chute est conservée.
    /// </summary>
    public decimal? MinScrapLength { get; set; }

    /// <summary>
    /// Longueur maximale en millimètres admise pour un stockage vertical.
    /// </summary>
    public int MaxVerticalLength { get; set; }

    /// <summary>
    /// Ordre d&apos;affichage de la référence dans les listes.
    /// </summary>
    public short SortOrder { get; set; }

    /// <summary>
    /// Date de création de la ligne dans le système local.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification dans le système local.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleInternal> ArticleInternals { get; set; } = new List<ArticleInternal>();

    public virtual ArticleCategory1 IdArticleCategory1Navigation { get; set; } = null!;

    public virtual ArticleCategory2 IdArticleCategory2Navigation { get; set; } = null!;

    public virtual ArticleCategory3 IdArticleCategory3Navigation { get; set; } = null!;

    public virtual ArticleIdentificationType IdArticleIdentificationTypeNavigation { get; set; } = null!;

    public virtual ArticleStorageUnit? IdArticleStorageUnitNavigation { get; set; }

    public virtual CuttingMachine IdCuttingMachineNavigation { get; set; } = null!;

    public virtual CuttingScrapLocation IdScrapLocationHorizontalNavigation { get; set; } = null!;

    public virtual CuttingScrapLocation IdScrapLocationVerticalNavigation { get; set; } = null!;
}
