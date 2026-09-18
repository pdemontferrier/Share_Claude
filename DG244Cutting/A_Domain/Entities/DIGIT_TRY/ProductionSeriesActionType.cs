using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Référentiel des types d’action du cycle de vie d’une série
/// </summary>
public partial class ProductionSeriesActionType
{
    /// <summary>
    /// Identifiant interne unique du type d’action
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Code unique représentant l’action dans le système
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Désignation lisible de l’action pour les utilisateurs
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Description courte du rôle et de l’usage du type d’action
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date de création du type d’action dans le système
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour du type d’action
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si le type d’action est supprimé logiquement
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ProductionSeriesAction> ProductionSeriesActions { get; set; } = new List<ProductionSeriesAction>();
}
