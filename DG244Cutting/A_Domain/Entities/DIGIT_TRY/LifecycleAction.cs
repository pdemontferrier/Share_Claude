using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Historique des actions liées au cycle de vie d’une série de production.
/// </summary>
public partial class LifecycleAction
{
    /// <summary>
    /// Clé technique interne autoincrémentée.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du type d’action du cycle de vie (référentiel LifecycleActionType).
    /// </summary>
    public short IdLifecycleActionType { get; set; }

    /// <summary>
    /// Identifiant de la source métier concernée par l’action (référentiel LifecycleActionSource).
    /// </summary>
    public short IdLifecycleActionSource { get; set; }

    /// <summary>
    /// Identifiant technique de l’entité métier concernée par l’action, selon la source déclarée.
    /// </summary>
    public int IdSource { get; set; }

    /// <summary>
    /// Identifiant de l’application ayant généré l’action (FK → AppList.Id).
    /// </summary>
    public int IdApplication { get; set; }

    /// <summary>
    /// Identifiant de l’utilisateur ayant déclenché l’action (FK → UserApp.Id).
    /// </summary>
    public int IdUser { get; set; }

    /// <summary>
    /// Commentaire libre associé à l’action du cycle de vie.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Nom utilisateur du poste client (session Windows ou nom OS).
    /// </summary>
    public string? DeviceUser { get; set; }

    /// <summary>
    /// Identifiant ou nom du poste client. Correspond généralement au nom réseau du terminal.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Adresse IP du poste client ayant généré l’action. Permet une traçabilité réseau en cas de diagnostic ou d’audit.
    /// </summary>
    public string? DeviceIp { get; set; }

    /// <summary>
    /// Horodatage exact de l’action de cycle de vie. Différent de CreatedAt : représente le moment réel où l’événement a été produit.
    /// </summary>
    public DateTime ActionTimestamp { get; set; }

    /// <summary>
    /// Horodatage de création de la ligne dans la base. Souvent identique à ActionTimestamp mais peut différer selon les sources.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Horodatage de la dernière mise à jour. Resté NULL si la ligne n’a jamais été modifiée après création.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Suppression logique (soft delete). 0 = actif, 1 = supprimé. Permet de conserver l’historique complet des actions.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationNavigation { get; set; } = null!;

    public virtual LifecycleActionSource IdLifecycleActionSourceNavigation { get; set; } = null!;

    public virtual LifecycleActionType IdLifecycleActionTypeNavigation { get; set; } = null!;

    public virtual UserApp IdUserNavigation { get; set; } = null!;
}
