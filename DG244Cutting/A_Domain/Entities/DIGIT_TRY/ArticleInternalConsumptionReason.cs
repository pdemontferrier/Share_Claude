using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table de référence contenant les motifs de consommation des articles internes.
/// </summary>
public partial class ArticleInternalConsumptionReason
{
    /// <summary>
    /// Identifiant unique du motif de consommation.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation textuelle du motif de consommation (exemples : &quot;Inventaire&quot;, &quot;Sortie atelier&quot;, &quot;Correction stock&quot;). Doit être unique.
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Date et heure de création de l’enregistrement, générée automatiquement par le système.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de la dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (0 = actif, 1 = supprimé).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleInternalConsumption> ArticleInternalConsumptions { get; set; } = new List<ArticleInternalConsumption>();
}
