using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Liste des adresses internes appartenant à une zone de stockage.
/// </summary>
public partial class StockZoneAddress
{
    /// <summary>
    /// Identifiant unique de l’adresse interne de zone.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence à la zone de stockage à laquelle appartient cette adresse interne.
    /// </summary>
    public int IdStockZone { get; set; }

    /// <summary>
    /// Désignation humaine et lisible de l’adresse dans la zone (ex : Z1-A01, Z2-C05).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Ordre de priorité pour le picking ou les opérations logistiques à l’intérieur de la zone.
    /// </summary>
    public short Priority { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Dernière date de mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’adresse est supprimée logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual StockZone IdStockZoneNavigation { get; set; } = null!;

    public virtual ICollection<StockBin> StockBins { get; set; } = new List<StockBin>();
}
