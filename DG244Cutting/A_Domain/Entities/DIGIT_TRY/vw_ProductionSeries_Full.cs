using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionSeries_Full
{
    /// <summary>
    /// Source : [ProductionSeries] - Champ [Id] - Clé technique interne (IDENTITY). N’existe pas dans AX.
    /// </summary>
    public int PSId { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdSerialNumber] - Numéro de série AX. Correspond au champ AX: SERIALNOSTR.
    /// </summary>
    public int PSIdSerialNumber { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdRec] - Identifiant unique AX (RECID). Lien avec la ligne AX originale.
    /// </summary>
    public long PSIdRec { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [RecVersion] - Version du record dans AX (contrôle de concurrence AX).
    /// </summary>
    public int PSRecVersion { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [Description] - Description de la série. Champ AX: EEEA_SERIALDESCRIPTION.
    /// </summary>
    public string PSDescription { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionStartDate] - Date de début de production. Champ AX: EEEA_SERIALPLANDATE.
    /// </summary>
    public DateTime? PSProductionStartDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDate] - Date de fin de production. Champ AX: ATWIN_PRODUCTIONENDDATE.
    /// </summary>
    public DateTime? PSProductionEndDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDay] - Code couleur étiquette. 0 = Violet si date absente.
    /// </summary>
    public short PSProductionEndDay { get; set; }

    /// <summary>
    /// Source : [ProductionColorLabelType] - Champ [Id] - Identifiant ENUM de la couleur d’étiquette de la série.
    /// </summary>
    public short? PCLTId { get; set; }

    /// <summary>
    /// Source : [ProductionColorLabelType] - Champ [Label] - Libellé textuel de la couleur d’étiquette de la série.
    /// </summary>
    public string? PCLTLabel { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [SerieCreatedAt] - Date de création initiale de la série. Champ AX: CREATEDDATETIME.
    /// </summary>
    public DateTime? PSSerieCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsImported] - Données importées depuis un fichier Leitxx.mdb (False/True).
    /// </summary>
    public bool PSIsImported { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsProductionValidated] - Série validée pour lancement (False/True).
    /// </summary>
    public bool PSIsProductionValidated { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarOptimized] - Série optimisée pour découpe sur barres de chutes.
    /// </summary>
    public bool PSIsDropBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarSupplied] - Série approvisionnée en barres de chutes.
    /// </summary>
    public bool PSIsDropBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarOptimized] - Série optimisée pour découpe sur barres neuves.
    /// </summary>
    public bool PSIsNewBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarSupplied] - Série approvisionnée en barres neuves.
    /// </summary>
    public bool PSIsNewBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsBarOutOfStock] - Série comportant au moins une barre en rupture de stock.
    /// </summary>
    public bool PSIsBarOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingStarted] - Découpe de la série démarrée (False/True).
    /// </summary>
    public bool PSIsCuttingStarted { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingCompleted] - Découpe de la série terminée (False/True).
    /// </summary>
    public bool PSIsCuttingCompleted { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [CreatedAt] - Date de création de l’enregistrement (système).
    /// </summary>
    public DateTime PSCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [UpdatedAt] - Date de dernière mise à jour (système).
    /// </summary>
    public DateTime? PSUpdatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDeleted] - Indicateur de suppression logique (soft delete).
    /// </summary>
    public bool PSIsDeleted { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Nombre de pièces de la série sur le périmètre de vw_ProductionCutPiece_Full.
    /// </summary>
    public int? PCPPieceCount { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Nombre de pièces découpées (IsCut = 1) sur le périmètre de la vue.
    /// </summary>
    public int? PCPPieceCutCount { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Nombre de pièces restant à découper (IsCut = 0) sur le périmètre de la vue.
    /// </summary>
    public int? PCPPieceNotCutCount { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Nombre de pièces refusées (IsCutRefused = 1) sur le périmètre de la vue.
    /// </summary>
    public int? PCPPieceRefusedCount { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Taux d’avancement en pourcentage, pièces découpées sur pièces totales.
    /// </summary>
    public decimal? PCPCutProgressPercent { get; set; }

    /// <summary>
    /// Calcul : [ProductionCutPiece] - Pourcentage restant à découper, complément à 100 du taux d’avancement.
    /// </summary>
    public decimal? PCPCutRemainingPercent { get; set; }
}
