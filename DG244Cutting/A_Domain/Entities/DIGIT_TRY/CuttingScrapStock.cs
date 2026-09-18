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
    /// Identifie l&apos;emplacement dedie aux chutes (CuttingScrapLocation) ou est rangee la chute.
    /// </summary>
    public int? IdCuttingScrapLocation { get; set; }

    /// <summary>
    /// Utilisateur ayant realise l&apos;action d&apos;enregistrement ou de modification.
    /// </summary>
    public int? IdOperator { get; set; }

    /// <summary>
    /// Longueur de la chute (en millimetres).
    /// </summary>
    public int LengthMm { get; set; }

    /// <summary>
    /// Largeur de la chute (en millimetres).
    /// </summary>
    public int WidthMm { get; set; }

    /// <summary>
    /// Dernier prix connu associe a cette chute.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Indique si la chute a ete scannee lors de sa prise en charge.
    /// </summary>
    public bool IsScanned { get; set; }

    /// <summary>
    /// Date d&apos;entree de la chute en stock.
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Numero de lot ou de projet ayant reserve cette chute.
    /// </summary>
    public string? ReservedFor { get; set; }

    /// <summary>
    /// Code-barres unique permettant l&apos;identification de la chute.
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Indique si la chute est en attente d&apos;integration dans le systeme.
    /// </summary>
    public bool WaitForIntegration { get; set; }

    /// <summary>
    /// Date a laquelle la chute a ete integree dans le systeme.
    /// </summary>
    public DateTime? IntegrationDate { get; set; }

    /// <summary>
    /// Origine declaree de la chute (poste, machine, operation).
    /// </summary>
    public string? Origin { get; set; }

    /// <summary>
    /// Indique si la chute est actuellement comptabilisee dans l&apos;inventaire.
    /// </summary>
    public bool IsInInventory { get; set; }

    /// <summary>
    /// Date de la derniere operation d&apos;inventaire concernant cette chute.
    /// </summary>
    public DateTime? InventoryDate { get; set; }

    /// <summary>
    /// Date de creation de l&apos;enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de derniere modification de l&apos;enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Marque la chute comme supprimee (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual CuttingScrapLocation? IdCuttingScrapLocationNavigation { get; set; }

    public virtual UserApp? IdOperatorNavigation { get; set; }

    public virtual ICollection<ProductionBar> ProductionBars { get; set; } = new List<ProductionBar>();
}
