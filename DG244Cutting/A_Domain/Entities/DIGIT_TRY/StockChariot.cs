using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Représente les chariots physiques utilisés pour la préparation et le déplacement des articles en atelier.
/// </summary>
public partial class StockChariot
{
    /// <summary>
    /// Identifiant unique du chariot.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation lisible du chariot (ex : Chariot 1).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si le chariot est supprimé logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }
}
