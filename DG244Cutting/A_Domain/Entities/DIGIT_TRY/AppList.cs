using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Liste des applications disponibles dans le système. Utilisée pour la gestion des droits, des sessions et de la traçabilité applicative.
/// </summary>
public partial class AppList
{
    /// <summary>
    /// Identifiant unique de l’application.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation fonctionnelle de l’application.
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Commentaire interne ou description de l’usage.
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// Indique si l’application est activée et visible pour l’utilisateur.
    /// </summary>
    public bool Accessible { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de dernière modification.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Suppression logique (1 = supprimé, 0 = actif).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<LifecycleAction> LifecycleActions { get; set; } = new List<LifecycleAction>();

    public virtual ICollection<UserAppAccess> UserAppAccesses { get; set; } = new List<UserAppAccess>();

    public virtual ICollection<UserAppErrorLog> UserAppErrorLogs { get; set; } = new List<UserAppErrorLog>();

    public virtual ICollection<UserAppEventStore> UserAppEventStores { get; set; } = new List<UserAppEventStore>();

    public virtual ICollection<UserAppMessage> UserAppMessageIdApplicationRecipientNavigations { get; set; } = new List<UserAppMessage>();

    public virtual ICollection<UserAppMessage> UserAppMessageIdApplicationSenderNavigations { get; set; } = new List<UserAppMessage>();

    public virtual ICollection<UserAppPageRight> UserAppPageRights { get; set; } = new List<UserAppPageRight>();

    public virtual ICollection<UserAppSessionCommand> UserAppSessionCommands { get; set; } = new List<UserAppSessionCommand>();

    public virtual ICollection<UserAppSession> UserAppSessions { get; set; } = new List<UserAppSession>();
}
