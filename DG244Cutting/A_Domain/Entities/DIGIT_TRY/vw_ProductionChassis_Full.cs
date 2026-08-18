using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionChassis_Full
{
    /// <summary>
    /// Source : [ProductionSeries] - Champ [Id] - Clé technique interne de la série (PK).
    /// </summary>
    public int PSId { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdSerialNumber] - Numéro de série AX (SERIALNOSTR).
    /// </summary>
    public int PSIdSerialNumber { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IdRec] - Identifiant unique AX (RECID).
    /// </summary>
    public long PSIdRec { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [RecVersion] - Version du record dans AX (RECVERSION).
    /// </summary>
    public int PSRecVersion { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [Description] - Description de la série.
    /// </summary>
    public string PSDescription { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionStartDate] - Date de début de production de la série.
    /// </summary>
    public DateTime? PSProductionStartDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDate] - Date de fin de production de la série.
    /// </summary>
    public DateTime? PSProductionEndDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDay] - Code couleur d’étiquette selon le jour de fin de production.
    /// </summary>
    public short PSProductionEndDay { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [SerieCreatedAt] - Date de création initiale de la série dans AX.
    /// </summary>
    public DateTime? PSSerieCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsImported] - Indique si les données métier de la série ont été importées.
    /// </summary>
    public bool PSIsImported { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsProductionValidated] - Indique si la série a été validée pour lancement.
    /// </summary>
    public bool PSIsProductionValidated { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarOptimized] - Indique si la série a été optimisée sur barres de chutes.
    /// </summary>
    public bool PSIsDropBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarSupplied] - Indique si la série a été approvisionnée en barres de chutes.
    /// </summary>
    public bool PSIsDropBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarOptimized] - Indique si la série a été optimisée sur barres neuves.
    /// </summary>
    public bool PSIsNewBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarSupplied] - Indique si la série a été approvisionnée en barres neuves.
    /// </summary>
    public bool PSIsNewBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsBarOutOfStock] - Indique si la série est en rupture de stock de barres.
    /// </summary>
    public bool PSIsBarOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingStarted] - Indique si au moins une découpe de la série a été réalisée.
    /// </summary>
    public bool PSIsCuttingStarted { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingCompleted] - Indique si toutes les découpes de la série ont été réalisées.
    /// </summary>
    public bool PSIsCuttingCompleted { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [Id] - Clé technique interne de la commande client (PK).
    /// </summary>
    public int COId { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [IdOrder] - Numéro de commande Tryba.
    /// </summary>
    public int COIdOrder { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [IdProductionSeries] - FK vers la série de production.
    /// </summary>
    public int COIdProductionSeries { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [PartialSeriesIndex] - Index de la série partielle.
    /// </summary>
    public int COPartialSeriesIndex { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProjectNumber] - Numéro de projet.
    /// </summary>
    public int? COProjectNumber { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProjectDesignation] - Désignation projet : sous-série, gamme et couleur.
    /// </summary>
    public string? COProjectDesignation { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ManufacturingSite] - Site de fabrication.
    /// </summary>
    public string? COManufacturingSite { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ManufacturingPlant] - Usine de fabrication.
    /// </summary>
    public string? COManufacturingPlant { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [DeliveryDate] - Date de livraison.
    /// </summary>
    public DateOnly? CODeliveryDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ShippingDate] - Date d’expédition.
    /// </summary>
    public DateOnly? COShippingDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionStartDate] - Date de début de production.
    /// </summary>
    public DateOnly? COProductionStartDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionStartWeek] - Semaine de début de production au format AAWW.
    /// </summary>
    public int? COProductionStartWeek { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndDate] - Date de fin de production.
    /// </summary>
    public DateOnly? COProductionEndDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndWeek] - Semaine de fin de production au format AAWW.
    /// </summary>
    public int? COProductionEndWeek { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndWeekday] - Jour de la semaine de fin de production.
    /// </summary>
    public int? COProductionEndWeekday { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndTourId] - Tournée de fin de production.
    /// </summary>
    public string? COProductionEndTourId { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [OrderSponsor] - Commanditaire de la commande.
    /// </summary>
    public string? COOrderSponsor { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointCode] - Numéro du client principal.
    /// </summary>
    public string? COMainSalesPointCode { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPoint] - Code du client principal.
    /// </summary>
    public string? COMainSalesPoint { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointName] - Nom du point de vente principal.
    /// </summary>
    public string? COMainSalesPointName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointAddress] - Adresse du client principal.
    /// </summary>
    public string? COMainSalesPointAddress { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [SecondarySalesPointName] - Nom du point de vente secondaire.
    /// </summary>
    public string? COSecondarySalesPointName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerName] - Nom du client final.
    /// </summary>
    public string? COCustomerName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerProjectName] - Nom du chantier.
    /// </summary>
    public string? COCustomerProjectName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerProjectDesignation] - Désignation du projet client.
    /// </summary>
    public string? COCustomerProjectDesignation { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerStreet] - Rue du chantier.
    /// </summary>
    public string? COCustomerStreet { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerCity] - Ville du chantier.
    /// </summary>
    public string? COCustomerCity { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerZipCode] - Code postal du chantier.
    /// </summary>
    public string? COCustomerZipCode { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerCountry] - Pays du chantier.
    /// </summary>
    public string? COCustomerCountry { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [DeliveryPosition] - Position de livraison.
    /// </summary>
    public string? CODeliveryPosition { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [QuaiZone] - Zone de quai.
    /// </summary>
    public string? COQuaiZone { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [Id] - Identifiant technique du châssis (PK).
    /// </summary>
    public int PCId { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [IdCustomerOrder] - FK vers la commande client.
    /// </summary>
    public int PCIdCustomerOrder { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [PartialSeriesIndex] - Index de la série partielle.
    /// </summary>
    public int PCPartialSeriesIndex { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OrderPosition] - Position du châssis dans la commande.
    /// </summary>
    public short PCOrderPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [BarcodeId] - Identifiant code-barres du châssis.
    /// </summary>
    public string PCBarcodeId { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionChassis] - Champ [Quantity] - Quantité de châssis.
    /// </summary>
    public short PCQuantity { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SeriesPosition] - Position du châssis dans la série.
    /// </summary>
    public short PCSeriesPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [CustomerPosition] - Position client.
    /// </summary>
    public string? PCCustomerPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ProductFamily] - Famille de produit.
    /// </summary>
    public string PCProductFamily { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ElementHeight] - Hauteur de l’élément.
    /// </summary>
    public short? PCElementHeight { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ElementWidth] - Largeur de l’élément.
    /// </summary>
    public short? PCElementWidth { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [FrameWidthIncludingRV] - Largeur du cadre incluant RV.
    /// </summary>
    public short? PCFrameWidthIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [FrameHeightIncludingRV] - Hauteur du cadre incluant RV.
    /// </summary>
    public short? PCFrameHeightIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OuterWidthIncludingRV] - Largeur extérieure incluant RV.
    /// </summary>
    public short? PCOuterWidthIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OuterHeightIncludingRV] - Hauteur extérieure incluant RV.
    /// </summary>
    public short? PCOuterHeightIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WidthWithCorrectionAndMiterTip] - Largeur avec correction et coupe à la pointe.
    /// </summary>
    public decimal? PCWidthWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [HeightWithCorrectionAndMiterTip] - Hauteur avec correction et coupe à la pointe.
    /// </summary>
    public short? PCHeightWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ColorNameIntExt] - Couleur intérieure et extérieure.
    /// </summary>
    public string? PCColorNameIntExt { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WindowText] - Texte descriptif de la fenêtre.
    /// </summary>
    public string? PCWindowText { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SashDimensionsLeftRight] - Dimensions des vantaux gauche et droit.
    /// </summary>
    public string? PCSashDimensionsLeftRight { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WindowSystemCode] - Code du système de fenêtre.
    /// </summary>
    public string? PCWindowSystemCode { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [CapacityZone] - Zone de capacité.
    /// </summary>
    public string? PCCapacityZone { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SlidingType] - Type de coulissant.
    /// </summary>
    public string? PCSlidingType { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SlidingTypeDetailed] - Type de coulissant détaillé.
    /// </summary>
    public string? PCSlidingTypeDetailed { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OpeningTypeAbbreviation] - Abréviation du type d’ouverture.
    /// </summary>
    public string? PCOpeningTypeAbbreviation { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OpeningTypeText] - Libellé du type d’ouverture.
    /// </summary>
    public string? PCOpeningTypeText { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SashPreset] - Vantaux prédéfinis.
    /// </summary>
    public string? PCSashPreset { get; set; }
}
