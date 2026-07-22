using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_CuttingScrapStock_Full
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
    /// Source : [ArticleReference] - Champ [IdArticleCategory2] - Catégorie secondaire de l’article (Catégorie 2).
    /// </summary>
    public short ARIdArticleCategory2 { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleCategory3] - Catégorie tertiaire de l’article (Catégorie 3).
    /// </summary>
    public short ARIdArticleCategory3 { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdCuttingMachine] - Machine de découpe associée à la référence.
    /// </summary>
    public int ARIdCuttingMachine { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleIdentificationType] - Type d’identification (pièce, barre, carton…).
    /// </summary>
    public short ARIdArticleIdentificationType { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdArticleStorageUnit] - Unité de stockage (mètre, pièce, carton…).
    /// </summary>
    public int? ARIdArticleStorageUnit { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationHorizontal] - Emplacement horizontal par défaut des chutes.
    /// </summary>
    public int ARIdScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdScrapLocationVertical] - Emplacement vertical par défaut des chutes.
    /// </summary>
    public int ARIdScrapLocationVertical { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [IdSupplier] - Fournisseur associé à la référence (optionnel).
    /// </summary>
    public int? ARIdSupplier { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [Reference] - Code alphanumérique unique. Source Tempor_Import.CodeNat.
    /// </summary>
    public string ARReference { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [Designation] - Désignation de la référence. Source Tempor_Import.Feld_10_100.
    /// </summary>
    public string ARDesignation { get; set; } = null!;

    /// <summary>
    /// Source : [ArticleReference] - Champ [FamilyCategory] - Catégorie famille. Source Tempor_Import.Feld_40.
    /// </summary>
    public string? ARFamilyCategory { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeFamily] - Code famille. Source Tempor_Import.Feld_16.
    /// </summary>
    public string? ARCodeFamily { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleReference] - Code article de référence. Source Tempor_Import.Feld_4.
    /// </summary>
    public string? ARCodeArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticle] - Code article principal. Source Tempor_Import.Feld_41.
    /// </summary>
    public string? ARCodeArticle { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [CodeArticleCuttingMachine] - Code article machine de découpe. Source Feld_10_330.
    /// </summary>
    public string? ARCodeArticleCuttingMachine { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MinScrapLength] - Longueur minimale de chute réutilisable.
    /// </summary>
    public decimal? ARMinScrapLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [MaxVerticalLength] - Longueur verticale maximale supportée par le stockage.
    /// </summary>
    public int ARMaxVerticalLength { get; set; }

    /// <summary>
    /// Source : [ArticleReference] - Champ [SortOrder] - Ordre de tri pour l’affichage et les traitements.
    /// </summary>
    public short ARSortOrder { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [Id] - Identifiant interne unique de l’article interne (PK).
    /// </summary>
    public int AIId { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdArticleReference] - Identifiant de la référence article (FK ArticleReference.Id).
    /// </summary>
    public int AIIdArticleReference { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [IdColorRalFinish] - Identifiant couleur/finition (FK ColorRalFinish.Id).
    /// </summary>
    public string? AIIdColorRalFinish { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [StandardBarLengthMm] - Longueur de stockage de référence de l’article (mm).
    /// </summary>
    public double? AIStandardBarLengthMm { get; set; }

    /// <summary>
    /// Source : [ArticleInternal] - Champ [ManageScraps] - Article interne géré avec suivi des chutes.
    /// </summary>
    public bool AIManageScraps { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [Id] - Identifiant unique de la chute en stock.
    /// </summary>
    public int CSSId { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IdArticleInternal] - Référence vers l’article interne dont provient la chute.
    /// </summary>
    public int CSSIdArticleInternal { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IdCuttingScrapLocation] - Identifie l’emplacement dédié aux chutes (CuttingScrapLocation) où est rangée la chute.
    /// </summary>
    public int? CSSIdCuttingScrapLocation { get; set; }

    /// <summary>
    /// Source : [CuttingScrapLocation] - Champ [Designation] - Désignation de l’emplacement dédié où est rangée la chute.
    /// </summary>
    public string? CSLDesignation { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IdOperator] - Utilisateur ayant réalisé l’action d’enregistrement ou de modification.
    /// </summary>
    public int? CSSIdOperator { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [LengthMm] - Longueur de la chute (en millimètres).
    /// </summary>
    public int CSSLengthMm { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [WidthMm] - Largeur de la chute (en millimètres).
    /// </summary>
    public int CSSWidthMm { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [Price] - Dernier prix connu associé à cette chute.
    /// </summary>
    public double CSSPrice { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IsScanned] - Indique si la chute a été scannée lors de sa prise en charge.
    /// </summary>
    public bool CSSIsScanned { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [EntryDate] - Date d’entrée de la chute en stock.
    /// </summary>
    public DateTime CSSEntryDate { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [ReservedFor] - Numéro de lot ou de projet ayant réservé cette chute.
    /// </summary>
    public string? CSSReservedFor { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [Barcode] - Code-barres unique permettant l’identification de la chute.
    /// </summary>
    public string? CSSBarcode { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [WaitForIntegration] - Indique si la chute est en attente d’intégration dans le système.
    /// </summary>
    public bool CSSWaitForIntegration { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IntegrationDate] - Date à laquelle la chute a été intégrée dans le système.
    /// </summary>
    public DateTime? CSSIntegrationDate { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [Origin] - Origine déclarée de la chute (poste, machine, opération).
    /// </summary>
    public string? CSSOrigin { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IsInInventory] - Indique si la chute est actuellement comptabilisée dans l’inventaire.
    /// </summary>
    public bool CSSIsInInventory { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [InventoryDate] - Date de la dernière opération d’inventaire concernant cette chute.
    /// </summary>
    public DateTime? CSSInventoryDate { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [CreatedAt] - Date de création de l’enregistrement.
    /// </summary>
    public DateTime CSSCreatedAt { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [UpdatedAt] - Date de dernière modification de l’enregistrement.
    /// </summary>
    public DateTime? CSSUpdatedAt { get; set; }

    /// <summary>
    /// Source : [CuttingScrapStock] - Champ [IsDeleted] - Marque la chute comme supprimée (soft delete).
    /// </summary>
    public bool CSSIsDeleted { get; set; }
}
