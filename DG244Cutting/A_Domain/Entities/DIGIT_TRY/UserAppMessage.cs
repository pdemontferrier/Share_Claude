using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Représente les messages échangés entre applications et utilisateurs.
/// </summary>
public partial class UserAppMessage
{
    /// <summary>
    /// Identifiant unique du message.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l’application à l’origine du message. Référence AppList(Id).
    /// </summary>
    public int IdApplicationSender { get; set; }

    /// <summary>
    /// Identifiant de l’utilisateur ayant envoyé le message. Référence User(Id).
    /// </summary>
    public int IdUserSender { get; set; }

    /// <summary>
    /// Identifiant de l’application destinataire du message. Référence AppList(Id).
    /// </summary>
    public int IdApplicationRecipient { get; set; }

    /// <summary>
    /// Date et heure d’envoi du message. Correspond également à la date d’émission.
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Sujet du message. Court texte descriptif utilisé comme titre de notification ou d’alerte.
    /// </summary>
    public string Subject { get; set; } = null!;

    /// <summary>
    /// Contenu textuel du message. Peut inclure des détails techniques, des instructions ou des informations contextuelles.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Indique si le message a été marqué comme lu par le destinataire. 0 = non lu, 1 = lu.
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement (métadonnée technique).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de dernière mise à jour de l’enregistrement. Null tant qu’aucune modification n’est effectuée.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement a été marqué comme supprimé sans suppression physique. 0 = actif, 1 = supprimé.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationRecipientNavigation { get; set; } = null!;

    public virtual AppList IdApplicationSenderNavigation { get; set; } = null!;

    public virtual UserApp IdUserSenderNavigation { get; set; } = null!;
}
