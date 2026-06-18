using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Référence les pages applicatives disponibles dans une application. Sert de base à la gestion des droits d’accès par page.
/// </summary>
public partial class UserAppPage
{
    /// <summary>
    /// Identifiant unique de la page (clé primaire). Reprend l’ID d’origine issu de la base source.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Code unique identifiant fonctionnellement une page applicative (exemple : P10_HOME, P20_STOCK).
    /// </summary>
    public string PageCode { get; set; } = null!;

    /// <summary>
    /// Date et heure de création de l’enregistrement. Gérée automatiquement par SQL Server.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de la ligne. Mise à jour automatiquement via EF Core lors des modifications.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement a été supprimé logiquement (1 = supprimé). Permet un archivage sans suppression réelle.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<UserAppPageRight> UserAppPageRights { get; set; } = new List<UserAppPageRight>();
}
