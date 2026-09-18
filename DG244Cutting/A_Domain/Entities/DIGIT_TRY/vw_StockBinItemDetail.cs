using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_StockBinItemDetail
{
    /// <summary>
    /// Identifiant unique du StockBinItem.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l’enregistrement ArticleInternal auquel appartient l’article en stock.
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Quantité disponible dans le StockBin.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Référence commerciale de l’article (provenant de ArticleReference).
    /// </summary>
    public string Reference { get; set; } = null!;

    /// <summary>
    /// Identifiant de la finition RAL associée à l’article (intérieur / extérieur).
    /// </summary>
    public string? IdColorRalFinish { get; set; }

    /// <summary>
    /// Désignation commerciale de l’article (ArticleReference).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Type d’identification de l’article (pièce, barre, carton…).
    /// </summary>
    public string? IdentificationType { get; set; }

    public double? StandardBarLengthMm { get; set; }

    /// <summary>
    /// Code de l’unité de stockage (ML, PC, CT…).
    /// </summary>
    public string? StorageCode { get; set; }

    /// <summary>
    /// Désignation textuelle de l’unité de stockage.
    /// </summary>
    public string? StorageUnit { get; set; }

    /// <summary>
    /// Catégorie de niveau 1 de l’article.
    /// </summary>
    public string? CategoryLevel1 { get; set; }

    /// <summary>
    /// Catégorie de niveau 2 de l’article.
    /// </summary>
    public string? CategoryLevel2 { get; set; }

    /// <summary>
    /// Catégorie de niveau 3 de l’article.
    /// </summary>
    public string? CategoryLevel3 { get; set; }

    /// <summary>
    /// Code machine de découpe associée à l’article, si applicable.
    /// </summary>
    public string? CuttingMachine { get; set; }

    /// <summary>
    /// Désignation de la zone de stockage où se trouve le bac.
    /// </summary>
    public string? ZoneDesignation { get; set; }

    /// <summary>
    /// Priorité de tri de la zone.
    /// </summary>
    public short? ZoneSortOrder { get; set; }

    /// <summary>
    /// Désignation de l’adresse physique du bac.
    /// </summary>
    public string? AddressDesignation { get; set; }

    /// <summary>
    /// Ordre de tri de l’adresse dans la zone.
    /// </summary>
    public short? AddressSortOrder { get; set; }

    /// <summary>
    /// Identifiant du bac où l’article est stocké.
    /// </summary>
    public int IdStockBin { get; set; }

    /// <summary>
    /// Désignation textuelle du bac de stockage.
    /// </summary>
    public string StockBinDesignation { get; set; } = null!;

    /// <summary>
    /// Désignation du support physique du bac (rack, étagère, palette…).
    /// </summary>
    public string? SupportTypeDesignation { get; set; }

    /// <summary>
    /// Type du bac (petit bac, casier, boîte…).
    /// </summary>
    public string? StockBinType { get; set; }

    /// <summary>
    /// Date du dernier inventaire physique pour cet article dans ce bac.
    /// </summary>
    public DateOnly InventoryDate { get; set; }

    /// <summary>
    /// Date à partir de laquelle l’article est considéré comme accessible en production.
    /// </summary>
    public DateTime AccessibleDate { get; set; }

    /// <summary>
    /// Indique si l’article est accessible pour la production (1 = oui, 0 = non).
    /// </summary>
    public bool IsAccessible { get; set; }
}
