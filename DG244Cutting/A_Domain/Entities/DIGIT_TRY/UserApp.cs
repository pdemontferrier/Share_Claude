using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table des utilisateurs des systèmes digitaux. Contient les informations d’identité, de contact, d’emploi, de gestion et de sécurité.
/// </summary>
public partial class UserApp
{
    /// <summary>
    /// Identifiant unique de l’utilisateur.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom de famille de l’utilisateur.
    /// </summary>
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Prénom de l’utilisateur.
    /// </summary>
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Identifiant de connexion unique de l’utilisateur.
    /// </summary>
    public string Login { get; set; } = null!;

    /// <summary>
    /// Initiales de l’utilisateur.
    /// </summary>
    public string Initials { get; set; } = null!;

    /// <summary>
    /// Mot de passe hashé de l’utilisateur. Ne doit jamais contenir le mot de passe en clair.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Date de naissance de l’utilisateur.
    /// </summary>
    public DateOnly Birthday { get; set; }

    /// <summary>
    /// Téléphone professionnel de l’utilisateur.
    /// </summary>
    public string PhonePro { get; set; } = null!;

    /// <summary>
    /// Téléphone fixe professionnel de l’utilisateur.
    /// </summary>
    public string PhoneProFixed { get; set; } = null!;

    /// <summary>
    /// Téléphone personnel de l’utilisateur.
    /// </summary>
    public string? PhonePersonal { get; set; }

    /// <summary>
    /// Adresse e-mail professionnelle de l’utilisateur.
    /// </summary>
    public string EmailProfessional { get; set; } = null!;

    /// <summary>
    /// Adresse e-mail personnelle de l’utilisateur.
    /// </summary>
    public string? EmailPersonal { get; set; }

    /// <summary>
    /// Type de contrat de l’utilisateur (CDD, CDI, intérim…).
    /// </summary>
    public int ContractType { get; set; }

    /// <summary>
    /// Identifiant du secteur d’activité auquel appartient l’utilisateur.
    /// </summary>
    public int? SectorId { get; set; }

    /// <summary>
    /// Identifiant de la société employant l’utilisateur.
    /// </summary>
    public int CompanyId { get; set; }

    /// <summary>
    /// Date d’entrée de l’utilisateur dans l’entreprise.
    /// </summary>
    public DateOnly EntryDate { get; set; }

    /// <summary>
    /// Adresse postale professionnelle de l’utilisateur.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Code postal de l’adresse professionnelle.
    /// </summary>
    public int? PostalCode { get; set; }

    /// <summary>
    /// Ville de l’adresse professionnelle.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Pays de l’adresse professionnelle.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Pourcentage de productivité utilisé pour les calculs internes.
    /// </summary>
    public short? ProductivityRate { get; set; }

    /// <summary>
    /// Matricule de l’utilisateur dans le système RH Pléiade.
    /// </summary>
    public string? PleiadeNumber { get; set; }

    /// <summary>
    /// Login Windows permettant l’authentification AD.
    /// </summary>
    public string? WindowsLogin { get; set; }

    /// <summary>
    /// Indique si le compte utilisateur est actif dans les systèmes digitaux.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indique si l’utilisateur doit réinitialiser son mot de passe lors de sa prochaine connexion.
    /// </summary>
    public bool IsResetRequired { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleInternalConsumption> ArticleInternalConsumptions { get; set; } = new List<ArticleInternalConsumption>();

    public virtual ICollection<CuttingScrapArchive> CuttingScrapArchiveIdOperatorEntryNavigations { get; set; } = new List<CuttingScrapArchive>();

    public virtual ICollection<CuttingScrapArchive> CuttingScrapArchiveIdOperatorExitNavigations { get; set; } = new List<CuttingScrapArchive>();

    public virtual ICollection<CuttingScrapStock> CuttingScrapStocks { get; set; } = new List<CuttingScrapStock>();

    public virtual ICollection<LifecycleAction> LifecycleActions { get; set; } = new List<LifecycleAction>();

    public virtual ICollection<UserAppAccess> UserAppAccesses { get; set; } = new List<UserAppAccess>();

    public virtual ICollection<UserAppErrorLog> UserAppErrorLogs { get; set; } = new List<UserAppErrorLog>();

    public virtual ICollection<UserAppEventStore> UserAppEventStores { get; set; } = new List<UserAppEventStore>();

    public virtual ICollection<UserAppMessage> UserAppMessages { get; set; } = new List<UserAppMessage>();

    public virtual ICollection<UserAppPageRight> UserAppPageRights { get; set; } = new List<UserAppPageRight>();

    public virtual ICollection<UserAppSessionCommand> UserAppSessionCommandIdUserIssuerNavigations { get; set; } = new List<UserAppSessionCommand>();

    public virtual ICollection<UserAppSessionCommand> UserAppSessionCommandIdUserTargetNavigations { get; set; } = new List<UserAppSessionCommand>();

    public virtual ICollection<UserAppSession> UserAppSessions { get; set; } = new List<UserAppSession>();
}
