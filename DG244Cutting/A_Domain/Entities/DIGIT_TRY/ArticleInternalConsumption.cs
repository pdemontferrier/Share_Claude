using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table enregistrant toutes les consommations d’articles internes réalisées dans l’atelier.
/// </summary>
public partial class ArticleInternalConsumption
{
    /// <summary>
    /// Identifiant unique de la consommation.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l’article interne consommé (FK → ArticleInternal.Id).
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Utilisateur ayant effectué ou déclaré la consommation (FK → UserApp.Id).
    /// </summary>
    public int IdUserApp { get; set; }

    /// <summary>
    /// Motif de la consommation (FK → ArticleInternalConsumptionReason.Id).
    /// </summary>
    public int IdConsumptionReason { get; set; }

    /// <summary>
    /// Quantité consommée lors de l’opération.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Date et heure de la consommation (UTC).
    /// </summary>
    public DateTime ConsumptionDate { get; set; }

    /// <summary>
    /// Désignation lisible du motif (copie à des fins d’audit).
    /// </summary>
    public string Reason { get; set; } = null!;

    /// <summary>
    /// Numéro de consommation manuelle (optionnel).
    /// </summary>
    public string? ManualConsumptionNumber { get; set; }

    /// <summary>
    /// Numéro de commande associé (si existant).
    /// </summary>
    public string? OrderNumber { get; set; }

    /// <summary>
    /// Nom du conteneur depuis lequel la consommation a été réalisée.
    /// </summary>
    public string? ContainerName { get; set; }

    /// <summary>
    /// Nom de l’adresse ou emplacement concerné (si disponible).
    /// </summary>
    public string? LocationName { get; set; }

    /// <summary>
    /// Date de création de la ligne (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de la ligne (UTC).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la ligne est supprimée logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual ArticleInternalConsumptionReason IdConsumptionReasonNavigation { get; set; } = null!;

    public virtual UserApp IdUserAppNavigation { get; set; } = null!;
}
