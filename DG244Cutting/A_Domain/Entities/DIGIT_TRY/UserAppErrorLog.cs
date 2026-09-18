using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Journal des erreurs applicatives générées par les services et applications du système. Stocke les messages, la chaîne d&apos;appel, les informations techniques.
/// </summary>
public partial class UserAppErrorLog
{
    /// <summary>
    /// Identifiant unique du journal d&apos;erreur.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence vers l&apos;application source de l&apos;erreur (FK vers AppList).
    /// </summary>
    public int? IdApplication { get; set; }

    /// <summary>
    /// Utilisateur connecté lors de l&apos;erreur (FK vers User). Peut être NULL si l&apos;erreur survient avant authentification.
    /// </summary>
    public int? IdUser { get; set; }

    /// <summary>
    /// Date et heure où l&apos;erreur a été enregistrée (horodatage système).
    /// </summary>
    public DateTime ErrorTimestamp { get; set; }

    /// <summary>
    /// Chaîne complète des appels (services, use cases, handlers) ayant conduit à l&apos;erreur.
    /// </summary>
    public string? CallChain { get; set; }

    /// <summary>
    /// Code d&apos;erreur fonctionnel ou technique associé à l&apos;incident.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Message d&apos;erreur lisible par l&apos;utilisateur ou le développeur.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Détails complets de l&apos;exception (.NET stacktrace ou message interne).
    /// </summary>
    public string? ErrorException { get; set; }

    /// <summary>
    /// Nom utilisateur sur l&apos;appareil local où l&apos;erreur s&apos;est produite.
    /// </summary>
    public string? DeviceUser { get; set; }

    /// <summary>
    /// Identifiant unique de l&apos;appareil (PC, terminal, machine).
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Adresse IP de l&apos;appareil ayant généré l&apos;erreur.
    /// </summary>
    public string? DeviceIp { get; set; }

    /// <summary>
    /// Date de création de l&apos;enregistrement dans le journal d&apos;erreur.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de la dernière mise à jour de l&apos;enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l&apos;enregistrement est marqué comme supprimé (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual AppList? IdApplicationNavigation { get; set; }

    public virtual UserApp? IdUserNavigation { get; set; }
}
