using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Référentiel des types d’action du cycle de vie (import, validation, découpe, expédition, etc.).
/// </summary>
public partial class LifecycleActionType
{
    /// <summary>
    /// Identifiant interne unique du type d’action.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Code unique représentant le type d’action.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Désignation lisible du type d’action.
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Description du rôle et du contexte du type d’action.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date de création du type d’action.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour du type d’action.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si le type d’action est supprimé logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<LifecycleAction> LifecycleActions { get; set; } = new List<LifecycleAction>();
}
