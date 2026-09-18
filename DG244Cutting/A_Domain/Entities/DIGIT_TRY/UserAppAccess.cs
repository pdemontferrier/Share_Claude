using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Associe un utilisateur à une application, avec un niveau d’accès configurable. Gère les autorisations globales par application.
/// </summary>
public partial class UserAppAccess
{
    /// <summary>
    /// Identifiant unique de l’enregistrement (clé primaire).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence vers l’utilisateur auquel les droits applicatifs sont attribués.
    /// </summary>
    public int IdUser { get; set; }

    /// <summary>
    /// Référence vers l’application à laquelle l’utilisateur est associé.
    /// </summary>
    public int IdApplication { get; set; }

    /// <summary>
    /// Niveau d’accès de l’utilisateur à l’application (1 = accès standard, valeurs supérieures = droits étendus selon la politique interne).
    /// </summary>
    public short AccessLevel { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification de l’enregistrement (mise à jour automatique via EF Core).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement (1 = supprimé). Permet une gestion d’historisation sans suppression physique.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationNavigation { get; set; } = null!;

    public virtual UserApp IdUserNavigation { get; set; } = null!;
}
