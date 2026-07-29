using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table storing configuration data for each cutting machine in the aluminium workshop.
/// </summary>
public partial class CuttingMachine
{
    /// <summary>
    /// Cle primaire technique, auto-incrementee, de la machine de decoupe.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique code identifying the cutting machine (e.g., DG244_01).
    /// </summary>
    public string MachineCode { get; set; } = null!;

    /// <summary>
    /// Libelle descriptif de la machine de decoupe.
    /// </summary>
    public string? Designation { get; set; }

    /// <summary>
    /// Nom reseau de la console de commande de la machine de decoupe.
    /// </summary>
    public string? ConsoleName { get; set; }

    /// <summary>
    /// Adresse IP de la console de commande de la machine de decoupe.
    /// </summary>
    public string? ConsoleIpAddress { get; set; }

    /// <summary>
    /// Port de communication (COM) de la console de commande de la machine.
    /// </summary>
    public string? ConsoleComPort { get; set; }

    /// <summary>
    /// Nom reseau du poste PC pilotant la machine de decoupe.
    /// </summary>
    public string? PcName { get; set; }

    /// <summary>
    /// Adresse IP du poste PC pilotant la machine de decoupe.
    /// </summary>
    public string? PcIpAddress { get; set; }

    /// <summary>
    /// Port de communication (COM) du poste PC pilotant la machine.
    /// </summary>
    public string? PcComPort { get; set; }

    /// <summary>
    /// Nom reseau de l&apos;imprimante associee a la machine de decoupe.
    /// </summary>
    public string? PrinterName { get; set; }

    /// <summary>
    /// Adresse IP de l&apos;imprimante associee a la machine de decoupe.
    /// </summary>
    public string? PrinterIpAddress { get; set; }

    /// <summary>
    /// Nom du fichier de la base donnees ELU CAD (.epd) qui pour chaque profil contient les machines d&apos;usinages et ses macros.
    /// </summary>
    public string? ProfileDatabaseFileName { get; set; }

    /// <summary>
    /// Nom du fichier du conteneur (.ncd) dedie a la machine d&apos;usinage dans la base de données et contenant les macros d&apos;usinage.
    /// </summary>
    public string? MacroDatabaseFileName { get; set; }

    /// <summary>
    /// Date de creation de la ligne dans le systeme local. N&apos;existe pas dans AX.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de derniere modification dans le systeme local. N&apos;existe pas dans AX.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (soft delete). N&apos;existe pas dans AX.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();

    public virtual ICollection<ProductionMachiningWork> ProductionMachiningWorks { get; set; } = new List<ProductionMachiningWork>();
}
