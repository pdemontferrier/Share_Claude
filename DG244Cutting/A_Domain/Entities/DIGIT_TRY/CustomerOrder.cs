using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Commande client issue de Tempor_Import (Leitxx.mdb)
/// </summary>
public partial class CustomerOrder
{
    /// <summary>
    /// Clé technique interne.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Numéro commande Tryba. Source : Tempor_Import.Aunummer.
    /// </summary>
    public int IdOrder { get; set; }

    /// <summary>
    /// FK série de production. Source : Tempor_Import.SerieNr.
    /// </summary>
    public int IdProductionSeries { get; set; }

    /// <summary>
    /// Index série partielle. Source : Tempor_Import.TeilserienIndex.
    /// </summary>
    public int PartialSeriesIndex { get; set; }

    /// <summary>
    /// Identifiant Look3E pour la Commande Client. Source : Tempor_Import.Feld_10_205.
    /// </summary>
    public string? LookCustomerOrderId { get; set; }

    /// <summary>
    /// Numéro de projet. Source : Tempor_Import.Feld_10_171.
    /// </summary>
    public int? ProjectNumber { get; set; }

    /// <summary>
    /// Sous-série + gamme + couleur. Source : Tempor_Import.Feld_10_288.
    /// </summary>
    public string? ProjectDesignation { get; set; }

    /// <summary>
    /// Site de fabrication. Source : Tempor_Import.Feld_10_073.
    /// </summary>
    public string? ManufacturingSite { get; set; }

    /// <summary>
    /// Usine de fabrication. Source : Tempor_Import.Feld_10_081.
    /// </summary>
    public string? ManufacturingPlant { get; set; }

    /// <summary>
    /// Date de livraison. Source : Tempor_Import.Feld_10_072.
    /// </summary>
    public DateOnly? DeliveryDate { get; set; }

    /// <summary>
    /// Date d’expédition. Source : Tempor_Import.Feld_10_213.
    /// </summary>
    public DateOnly? ShippingDate { get; set; }

    /// <summary>
    /// Début production. Source : Tempor_Import.Feld_10_082.
    /// </summary>
    public DateOnly? ProductionStartDate { get; set; }

    /// <summary>
    /// Semaine début production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_243.
    /// </summary>
    public int? ProductionStartWeek { get; set; }

    /// <summary>
    /// Fin production. Source : Tempor_Import.Feld_10_212.
    /// </summary>
    public DateOnly? ProductionEndDate { get; set; }

    /// <summary>
    /// Semaine fin production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_054.
    /// </summary>
    public int? ProductionEndWeek { get; set; }

    /// <summary>
    /// Jour semaine fin production. Source : Tempor_Import.Feld_10_545.
    /// </summary>
    public int? ProductionEndWeekday { get; set; }

    /// <summary>
    /// Tournée fin production. Source : Tempor_Import.Feld_10_053.
    /// </summary>
    public string? ProductionEndTourId { get; set; }

    /// <summary>
    /// Commanditaire de la commande. Source : Tempor_Import.Feld_10_299.
    /// </summary>
    public string? OrderSponsor { get; set; }

    /// <summary>
    /// Numéro client principal. Source : Tempor_Import.Feld_10_326.
    /// </summary>
    public string? MainSalesPointCode { get; set; }

    /// <summary>
    /// Code client principal. Source : Tempor_Import.Feld_10_273.
    /// </summary>
    public string? MainSalesPoint { get; set; }

    /// <summary>
    /// Point de vente principal. Source : Tempor_Import.Feld_10_110.
    /// </summary>
    public string? MainSalesPointName { get; set; }

    /// <summary>
    /// Adresse client principal. Source : Tempor_Import.Feld_10_274.
    /// </summary>
    public string? MainSalesPointAddress { get; set; }

    /// <summary>
    /// Point de vente secondaire. Source : Tempor_Import.Feld_10_024.
    /// </summary>
    public string? SecondarySalesPointName { get; set; }

    /// <summary>
    /// Nom du client final. Source : Tempor_Import.Feld_10_163.
    /// </summary>
    public string? CustomerName { get; set; }

    /// <summary>
    /// Nom du chantier. Source : Tempor_Import.Feld_10_083.
    /// </summary>
    public string? CustomerProjectName { get; set; }

    /// <summary>
    /// Désignation projet. Source : Tempor_Import.Feld_10_002.
    /// </summary>
    public string? CustomerProjectDesignation { get; set; }

    /// <summary>
    /// Rue du chantier. Source : Tempor_Import.Feld_10_087.
    /// </summary>
    public string? CustomerStreet { get; set; }

    /// <summary>
    /// Ville du chantier. Source : Tempor_Import.Feld_10_086.
    /// </summary>
    public string? CustomerCity { get; set; }

    /// <summary>
    /// Code postal du chantier. Source : Tempor_Import.Feld_10_085.
    /// </summary>
    public string? CustomerZipCode { get; set; }

    /// <summary>
    /// Pays du chantier. Source : Tempor_Import.Feld_10_084.
    /// </summary>
    public string? CustomerCountry { get; set; }

    /// <summary>
    /// Position de livraison. Source : Tempor_Import.Feld_10_118.
    /// </summary>
    public string? DeliveryPosition { get; set; }

    /// <summary>
    /// Zone de quai. Source : Tempor_Import.Feld_10_184.
    /// </summary>
    public string? QuaiZone { get; set; }

    /// <summary>
    /// Date de création SQL.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de mise à jour SQL.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Suppression logique.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ProductionSeries IdProductionSeriesNavigation { get; set; } = null!;

    public virtual ICollection<ProductionChassi> ProductionChassis { get; set; } = new List<ProductionChassi>();
}
