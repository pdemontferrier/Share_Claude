using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Référentiel des entités sources concernées par les actions du cycle de vie.
/// </summary>
public partial class LifecycleActionSource
{
    /// <summary>
    /// Identifiant interne unique de la source d’action.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Code court identifiant la source métier de l’action.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Nom logique de la source métier (table associée).
    /// </summary>
    public string SourceName { get; set; } = null!;

    /// <summary>
    /// Description du périmètre fonctionnel de la source.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date de création de la source d’action.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de la source d’action.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la source d’action est supprimée logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<LifecycleAction> LifecycleActions { get; set; } = new List<LifecycleAction>();
}
