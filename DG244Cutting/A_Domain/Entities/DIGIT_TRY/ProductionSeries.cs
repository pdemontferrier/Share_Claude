using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table contenant les informations des séries de production issues d’Axapta.
/// </summary>
public partial class ProductionSeries
{
    /// <summary>
    /// Clé technique interne (IDENTITY). N’existe pas dans AX.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Numéro de série AX. Correspond au champ AX: SERIALNOSTR.
    /// </summary>
    public int IdSerialNumber { get; set; }

    /// <summary>
    /// Identifiant unique AX (RECID). Permet d’assurer le lien avec la ligne AX originale.
    /// </summary>
    public long IdRec { get; set; }

    /// <summary>
    /// Version du record dans AX. Correspond au champ AX: RECVERSION (utilisé pour le contrôle de concurrence dans AX).
    /// </summary>
    public int RecVersion { get; set; }

    /// <summary>
    /// Description de la série. Correspond au champ AX: EEEA_SERIALDESCRIPTION.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Date de début de la production. Correspond au champ AX: EEEA_SERIALPLANDATE.
    /// </summary>
    public DateTime? ProductionStartDate { get; set; }

    /// <summary>
    /// Date de fin de production. Correspond au champ AX: ATWIN_PRODUCTIONENDDATE.
    /// </summary>
    public DateTime? ProductionEndDate { get; set; }

    /// <summary>
    /// Code couleur de l’étiquette, calculé depuis le jour de fin de production. Référence ProductionColorLabelType. 0 = Violet si date absente.
    /// </summary>
    public short ProductionEndDay { get; set; }

    /// <summary>
    /// Date de création initiale de la série. Correspond au champ AX: CREATEDDATETIME.
    /// </summary>
    public DateTime? SerieCreatedAt { get; set; }

    /// <summary>
    /// Indique si les données métier associées à la série ont été importées depuis un fichier Leitxx.mdb. False = non importée, True = importée.
    /// </summary>
    public bool IsImported { get; set; }

    /// <summary>
    /// Indique si la série a été validée pour lancement. False = à valider, True = validée.
    /// </summary>
    public bool IsProductionValidated { get; set; }

    /// <summary>
    /// Indique si la série a reçu l’approvisionnement en barres de chutes (stock de chutes). False = non approvisionnée, True = approvisionnée.
    /// </summary>
    public bool IsDropBarSupplied { get; set; }

    /// <summary>
    /// Indique si la série a reçu l’approvisionnement en barres neuves. False = non approvisionnée, True = approvisionnée.
    /// </summary>
    public bool IsNewBarSupplied { get; set; }

    /// <summary>
    /// Indique si une des découpes de la série a été réalisée. False = non commencée, True = commencée.
    /// </summary>
    public bool IsCuttingStarted { get; set; }

    /// <summary>
    /// Indique si l’ensemble des découpes de la série ont été réalisées. False = non découpée, True = découpée.
    /// </summary>
    public bool IsCuttingCompleted { get; set; }

    /// <summary>
    /// Date de création de la ligne dans le système local. N’existe pas dans AX.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification dans le système local. N’existe pas dans AX.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (soft delete). N’existe pas dans AX.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

    public virtual ProductionColorLabelType ProductionEndDayNavigation { get; set; } = null!;
}
