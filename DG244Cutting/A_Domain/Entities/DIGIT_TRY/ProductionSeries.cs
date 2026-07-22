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
    /// Indique si la serie a ete optimisee pour la decoupe sur barres de chutes. False = non optimisee, True = optimisee.
    /// </summary>
    public bool IsDropBarOptimized { get; set; }

    /// <summary>
    /// Indique si la serie a recu l&apos;approvisionnement en barres de chutes (stock de chutes). False = non approvisionnee, True = approvisionnee.
    /// </summary>
    public bool IsDropBarSupplied { get; set; }

    /// <summary>
    /// Indique si la serie a ete optimisee pour la decoupe sur barres neuves. False = non optimisee, True = optimisee.
    /// </summary>
    public bool IsNewBarOptimized { get; set; }

    /// <summary>
    /// Indique si la serie a recu l&apos;approvisionnement en barres neuves. False = non approvisionnee, True = approvisionnee.
    /// </summary>
    public bool IsNewBarSupplied { get; set; }

    /// <summary>
    /// Indique si la serie est en rupture de stock de barres. False = stock disponible, True = rupture.
    /// </summary>
    public bool IsBarOutOfStock { get; set; }

    /// <summary>
    /// Indique si une des decoupes de la serie a ete realisee. False = non commencee, True = commencee.
    /// </summary>
    public bool IsCuttingStarted { get; set; }

    /// <summary>
    /// Indique si l&apos;ensemble des decoupes de la serie ont ete realisees. False = non decoupee, True = decoupee.
    /// </summary>
    public bool IsCuttingCompleted { get; set; }

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

    public virtual ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

    public virtual ICollection<ProductionBar> ProductionBars { get; set; } = new List<ProductionBar>();

    public virtual ProductionColorLabelType ProductionEndDayNavigation { get; set; } = null!;
}
