using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table technique d’Event Store : historisation automatique des écritures en base sous forme de snapshots JSON. Chaque enregistrement capture l’état d’une entité persistée (TableDesignation/TableId/Data) et le contexte applicatif (AppId, AppUserId, Device*), avec traçabilité par callChain complète (AppCallChain).
/// </summary>
public partial class UserAppEventStore
{
    /// <summary>
    /// Clé primaire technique (IDENTITY) de l’enregistrement Event Store.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation de la table/entité concernée (généralement typeof(T).Name côté application).
    /// </summary>
    public string? TableDesignation { get; set; }

    /// <summary>
    /// Identifiant de l’enregistrement de la table/entité concernée. Renseigné après persistance (Id &gt; 0).
    /// </summary>
    public int? TableId { get; set; }

    /// <summary>
    /// Horodatage applicatif de l’événement (DTO_AppContext.AppDateTime) permettant de tracer l’instant logique de l’écriture.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Snapshot JSON de l’entité (état sérialisé) tel qu’enregistré lors de l’opération d’écriture.
    /// </summary>
    public string? Data { get; set; }

    /// <summary>
    /// Identifiant de l’application à l’origine de l’écriture (FK vers dbo.AppList).
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// Chaîne d’appel complète (callChain), permettant la traçabilité bout-en-bout de l’action.
    /// </summary>
    public string? AppCallChain { get; set; }

    /// <summary>
    /// Utilisateur applicatif à l’origine de l’écriture (FK vers dbo.UserApp).
    /// </summary>
    public int? AppUserId { get; set; }

    /// <summary>
    /// Identifiant utilisateur côté poste/terminal (contexte device) ayant déclenché l’écriture.
    /// </summary>
    public string? DeviceUser { get; set; }

    /// <summary>
    /// Identifiant du poste/terminal (contexte device) ayant déclenché l’écriture.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Adresse IP du poste/terminal (contexte device) ayant déclenché l’écriture.
    /// </summary>
    public string? DeviceIp { get; set; }

    /// <summary>
    /// Date de création technique de l’enregistrement (sysdatetime()).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour technique de l’enregistrement (NULL si jamais mis à jour).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Flag de suppression logique standard Projet 104 (0 = actif, 1 = supprimé). En pratique l’Event Store reste généralement non supprimé.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList? App { get; set; }

    public virtual UserApp? AppUser { get; set; }
}
