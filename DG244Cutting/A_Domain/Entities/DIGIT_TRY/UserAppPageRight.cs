using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table enregistrant les droits d’’accès par utilisateur, application et page.
/// </summary>
public partial class UserAppPageRight
{
    /// <summary>
    /// Identifiant unique interne.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l’utilisateur concerné.
    /// </summary>
    public int IdUser { get; set; }

    /// <summary>
    /// Identifiant de l’application cible.
    /// </summary>
    public int IdApplication { get; set; }

    /// <summary>
    /// Code fonctionnel de la page. Exception du projet : utilisé comme clé étrangère plutôt que l’Id technique.
    /// </summary>
    public string PageCode { get; set; } = null!;

    /// <summary>
    /// L’utilisateur peut accéder à la page.
    /// </summary>
    public bool CanAccess { get; set; }

    /// <summary>
    /// L’utilisateur peut lire les données.
    /// </summary>
    public bool CanRead { get; set; }

    /// <summary>
    /// L’utilisateur peut modifier les données.
    /// </summary>
    public bool CanUpdate { get; set; }

    /// <summary>
    /// L’utilisateur peut créer des données.
    /// </summary>
    public bool CanCreate { get; set; }

    /// <summary>
    /// L’utilisateur peut supprimer des données.
    /// </summary>
    public bool CanDelete { get; set; }

    /// <summary>
    /// L’utilisateur peut effectuer des actions de contrôle.
    /// </summary>
    public bool CanControl { get; set; }

    /// <summary>
    /// L’utilisateur peut valider des actions.
    /// </summary>
    public bool CanValidate { get; set; }

    /// <summary>
    /// L’utilisateur peut superviser les opérations.
    /// </summary>
    public bool CanSupervise { get; set; }

    /// <summary>
    /// L’utilisateur peut suivre en temps réel.
    /// </summary>
    public bool CanMonitor { get; set; }

    /// <summary>
    /// L’utilisateur dispose des droits administratifs.
    /// </summary>
    public bool CanAdmin { get; set; }

    /// <summary>
    /// Date de création de la ligne.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la ligne est supprimée logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationNavigation { get; set; } = null!;

    public virtual UserApp IdUserNavigation { get; set; } = null!;

    public virtual UserAppPage PageCodeNavigation { get; set; } = null!;
}
