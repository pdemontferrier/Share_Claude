using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Représente le stock actif des chutes disponibles dans l’atelier aluminium.
/// </summary>
public partial class CuttingScrapStock
{
    /// <summary>
    /// Identifiant unique de la chute en stock.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence vers l’article interne dont provient la chute.
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Identifie l’emplacement physique (StockBin) où est rangée la chute.
    /// </summary>
    public int? IdStockBin { get; set; }

    /// <summary>
    /// Utilisateur ayant réalisé l’action d’enregistrement ou de modification.
    /// </summary>
    public int? IdOperator { get; set; }

    /// <summary>
    /// Longueur de la chute (en millimètres).
    /// </summary>
    public int LengthMm { get; set; }

    /// <summary>
    /// Largeur de la chute (en millimètres).
    /// </summary>
    public int WidthMm { get; set; }

    /// <summary>
    /// Dernier prix connu associé à cette chute.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Indique si la chute a été scannée lors de sa prise en charge.
    /// </summary>
    public bool IsScanned { get; set; }

    /// <summary>
    /// Date d’entrée de la chute en stock.
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Numéro de lot ou de projet ayant réservé cette chute.
    /// </summary>
    public string? ReservedFor { get; set; }

    /// <summary>
    /// Code-barres unique permettant l’identification de la chute.
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Indique si la chute est en attente d’intégration dans le système.
    /// </summary>
    public bool WaitForIntegration { get; set; }

    /// <summary>
    /// Date à laquelle la chute a été intégrée dans le système.
    /// </summary>
    public DateTime? IntegrationDate { get; set; }

    /// <summary>
    /// Origine déclarée de la chute (poste, machine, opération).
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Indique si la chute est actuellement comptabilisée dans l’inventaire.
    /// </summary>
    public bool IsInInventory { get; set; }

    /// <summary>
    /// Date de la dernière opération d’inventaire concernant cette chute.
    /// </summary>
    public DateTime? InventoryDate { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Marque la chute comme supprimée (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual UserApp? IdOperatorNavigation { get; set; }

    public virtual StockBin? IdStockBinNavigation { get; set; }
}
