using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table enregistrant les sessions utilisateur pour les applications internes.
/// </summary>
public partial class UserAppSession
{
    /// <summary>
    /// Identifiant unique de la session.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Clé étrangère vers AppList. Identifie l’application concernée par la session.
    /// </summary>
    public int IdApplication { get; set; }

    /// <summary>
    /// Clé étrangère vers User. Identifie l’utilisateur connecté.
    /// </summary>
    public int IdUser { get; set; }

    /// <summary>
    /// Nom d’utilisateur de l’appareil client.
    /// </summary>
    public string? DeviceUser { get; set; }

    /// <summary>
    /// Identifiant unique de l’appareil (machine, terminal).
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Adresse IP de l’appareil client.
    /// </summary>
    public string? DeviceIp { get; set; }

    /// <summary>
    /// État actuel de la session (1 = connectée, 0 = déconnectée).
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Date et heure de connexion de la session.
    /// </summary>
    public DateTime ConnectionDate { get; set; }

    /// <summary>
    /// Date et heure de déconnexion de la session (NULL si toujours connectée).
    /// </summary>
    public DateTime? DisconnectionDate { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de la session.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la session est supprimée logiquement (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList IdApplicationNavigation { get; set; } = null!;

    public virtual UserApp IdUserNavigation { get; set; } = null!;
}
