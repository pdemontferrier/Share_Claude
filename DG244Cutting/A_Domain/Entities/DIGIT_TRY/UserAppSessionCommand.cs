using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Représente les commandes envoyées d’une application ou d’un utilisateur vers un autre utilisateur.
/// </summary>
public partial class UserAppSessionCommand
{
    /// <summary>
    /// Identifiant unique de la commande (clé primaire).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Utilisateur destinataire de la commande. Référence la table User. Identifie celui qui reçoit l’ordre ou la notification.
    /// </summary>
    public int IdUserTarget { get; set; }

    /// <summary>
    /// Application destinataire de la commande (ex : BatchCutting, BatchStockRelease). Permet de router la commande vers l’application correcte.
    /// </summary>
    public int IdApplicationTarget { get; set; }

    /// <summary>
    /// Utilisateur émetteur de la commande. Référence la table User. Permet de tracer l’origine d’une commande ou action distante.
    /// </summary>
    public int IdUserIssuer { get; set; }

    /// <summary>
    /// Type de commande envoyée (ex : RefreshData, LogoutUser, LockSession, NotifyWarning). Définit l’action à exécuter côté client.
    /// </summary>
    public string? CommandType { get; set; }

    /// <summary>
    /// Date et heure à laquelle la commande a été générée. Sert de base au traitement FIFO des commandes.
    /// </summary>
    public DateTime CommandDate { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement. Renseignée automatiquement par SQL Server via sysdatetime().
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour. Gérée côté EF Core ou via triggers si nécessaire.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement. 0 = actif, 1 = supprimé. Utilisé pour éviter les suppressions physiques.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationTargetNavigation { get; set; } = null!;

    public virtual UserApp IdUserIssuerNavigation { get; set; } = null!;

    public virtual UserApp IdUserTargetNavigation { get; set; } = null!;
}
