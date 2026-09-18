using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Définit les zones de stockage physiques de l’atelier aluminium.
/// </summary>
public partial class StockZone
{
    /// <summary>
    /// Identifiant unique de la zone de stockage.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation lisible de la zone (ex : ZONE A, TAMPON, etc.).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Priorité de picking associée à la zone. Les zones à priorité faible sont traitées en premier.
    /// </summary>
    public short Priority { get; set; }

    /// <summary>
    /// Date et heure de création de la zone dans le système.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de dernière modification.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la zone est supprimée logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<StockZoneAddress> StockZoneAddresses { get; set; } = new List<StockZoneAddress>();
}
