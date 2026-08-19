using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ArticleInternal_Full
{
    /// <summary>
    /// Source : [ArticleReference] - Champ [Id] - Clé primaire de la référence article.
    /// </summary>
    public int ARId { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory1] - Catégorie principale de l’article (Catégorie 1).
    /// </summary>
    public short ARIdArticleCategory1 { get; set; }

    /// <summary>
    /// Source : [ArticleCategory1] - Champ [Designation] - Désignation de la catégorie article de niveau 1.
    /// </summary>
    public string? AC1Designation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory2] - Catégorie secondaire de l’article (Catégorie 2).
    /// </summary>
    public short ARIdArticleCategory2 { get; set; }

    /// <summary>
    /// Source : [ArticleCategory2] - Champ [Designation] - Désignation de la catégorie article de niveau 2.
    /// </summary>
    public string? AC2Designation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory3] - Catégorie tertiaire de l’article (Catégorie 3).
    /// </summary>
    public short ARIdArticleCategory3 { get; set; }

    /// <summary>
    /// Source : [ArticleCategory3] - Champ [Designation] - Désignation de la catégorie article de niveau 3.
    /// </summary>
    public string? AC3Designation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdCuttingMachine] - Machine de découpe associée à la référence.
    /// </summary>
    public int ARIdCuttingMachine { get; set; }

    /// <summary>
    /// Source : [CuttingMachine] - Champ [MachineCode] - Code de la machine de découpe (ex : DG244).
    /// </summary>
    public string? CMMachineCode { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleIdentificationType] - Type d’identification de l’article.
    /// </summary>
    public short ARIdArticleIdentificationType { get; set; }

    /// <summary>
    /// Source : [ArticleIdentificationType] - Champ [Code] - Code du type d’identification.
    /// </summary>
    public string? AITCode { get; set; }

    /// <summary>
    /// Source : [ArticleIdentificationType] - Champ [Designation] - Type d’identification : pièce, barre, boîte…
    /// </summary>
    public string? AITDesignation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleStorageUnit] - Unité de stockage de la référence.
    /// </summary>
    public int? ARIdArticleStorageUnit { get; set; }

    /// <summary>
    /// Source : [ArticleStorageUnit] - Champ [Code] - Code de l’unité de stockage (ex : ML, PC, KG…).
    /// </summary>
    public string? ASUCode { get; set; }

    /// <summary>
    /// Source : [ArticleStorageUnit] - Champ [Designation] - Désignation complète de l’unité de stockage.
    /// </summary>
    public string? ASUDesignation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationHorizontal] - Emplacement horizontal des chutes.
    /// </summary>
    public int ARIdScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Source : [CuttingScrapLocation] - Champ [Designation] - Désignation de l’emplacement horizontal des chutes.
    /// </summary>
    public string? CSLHDesignation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationVertical] - Emplacement vertical des chutes.
    /// </summary>
    public int ARIdScrapLocationVertical { get; set; }

    /// <summary>
    /// Source : [CuttingScrapLocation] - Champ [Designation] - Désignation de l’emplacement vertical des chutes.
    /// </summary>
    public string? CSLVDesignation { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [Reference] - Code alphanumérique unique. Source Tempor_Import.CodeNat.
    /// </summary>
    public string ARReference { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [Designation] - Désignation de la référence. Source Feld_10_100.
    /// </summary>
    public string ARDesignation { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [FamilyCategoryPrincipal] - Catégorie métier principale.
    /// </summary>
    public string? ARFamilyCategoryPrincipal { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [FamilyCategorySecondary] - Catégorie famille. Source Feld_40.
    /// </summary>
    public string? ARFamilyCategorySecondary { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeFamily] - Code famille. Source Tempor_Import.Feld_16.
    /// </summary>
    public string? ARCodeFamily { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleReference] - Code article de référence. Source Feld_4.
    /// </summary>
    public string? ARCodeArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticle] - Code article principal. Source Feld_41.
    /// </summary>
    public string? ARCodeArticle { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleCuttingMachine] - Code article machine. Source Feld_10_330.
    /// </summary>
    public string? ARCodeArticleCuttingMachine { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [StandardBarLengthMm] - Longueur standard de barre de la référence (mm).
    /// </summary>
    public decimal? ARStandardBarLengthMm { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [BarWidthMm] - Largeur de la barre en mm. Source Feld_10_051.
    /// </summary>
    public decimal? ARBarWidthMm { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [BarHeightMm] - Hauteur de la barre en mm. Source Feld_10_075.
    /// </summary>
    public decimal? ARBarHeightMm { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [ManageScraps] - Référence gérée avec suivi des chutes.
    /// </summary>
    public bool ARManageScraps { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MinScrapLength] - Longueur minimale de chute réutilisable.
    /// </summary>
    public decimal? ARMinScrapLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MaxVerticalLength] - Longueur verticale maximale de stockage.
    /// </summary>
    public int ARMaxVerticalLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [SortOrder] - Ordre de tri pour l’affichage et les traitements.
    /// </summary>
    public short ARSortOrder { get; set; }

    /// <summary>
    /// Source : [ColorRalFinish] - Champ [Id] - Identifiant du couple RAL et finition.
    /// </summary>
    public string? CRFId { get; set; }

    /// <summary>
    /// Source : [ColorRalFinish] - Champ [IdInternalRal] - Code RAL de la face intérieure.
    /// </summary>
    public int? CRFIdInternalRal { get; set; }

    /// <summary>
    /// Source : [ColorRalFinish] - Champ [IdInternalFinish] - Code finition de la face intérieure.
    /// </summary>
    public string? CRFIdInternalFinish { get; set; }

    /// <summary>
    /// Source : [ColorRalFinish] - Champ [IdExternalRal] - Code RAL de la face extérieure.
    /// </summary>
    public int? CRFIdExternalRal { get; set; }

    /// <summary>
    /// Source : [ColorRalFinish] - Champ [IdExternalFinish] - Code finition de la face extérieure.
    /// </summary>
    public string? CRFIdExternalFinish { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [Id] - Identifiant interne unique de l’article interne (PK).
    /// </summary>
    public int AIId { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdArticleReference] - Référence article associée (FK).
    /// </summary>
    public int AIIdArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdColorRalFinish] - Couple RAL et finition de l’article interne (FK).
    /// </summary>
    public string? AIIdColorRalFinish { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [StandardBarLengthMm] - Longueur de stockage de référence (mm).
    /// </summary>
    public double? AIStandardBarLengthMm { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [ManageScraps] - Article interne géré avec suivi des chutes.
    /// </summary>
    public bool AIManageScraps { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [CreatedAt] - Date de création de l’enregistrement.
    /// </summary>
    public DateTime AICreatedAt { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [UpdatedAt] - Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? AIUpdatedAt { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IsDeleted] - Indique une suppression logique de l’enregistrement.
    /// </summary>
    public bool AIIsDeleted { get; set; }
}
