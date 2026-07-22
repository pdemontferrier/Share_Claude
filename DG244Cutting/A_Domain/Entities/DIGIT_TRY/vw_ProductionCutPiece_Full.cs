using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionCutPiece_Full
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
    /// Source : [ProductionSeries] - Champ [IdRec] - Identifiant unique AX (RECID). Permet d’assurer le lien avec la ligne AX originale.
    /// </summary>
    public long PSIdRec { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [RecVersion] - Version du record dans AX. Correspond au champ AX: RECVERSION (utilisé pour le contrôle de concurrence dans AX).
    /// </summary>
    public int PSRecVersion { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [Description] - Description de la série. Correspond au champ AX: EEEA_SERIALDESCRIPTION.
    /// </summary>
    public string PSDescription { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionStartDate] - Date de début de la production. Correspond au champ AX: EEEA_SERIALPLANDATE.
    /// </summary>
    public DateTime? PSProductionStartDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDate] - Date de fin de production. Correspond au champ AX: ATWIN_PRODUCTIONENDDATE.
    /// </summary>
    public DateTime? PSProductionEndDate { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [ProductionEndDay] - Code couleur de l’étiquette, calculé depuis le jour de fin de production. Référence ProductionColorLabelType. 0 = Violet si date absente.
    /// </summary>
    public short PSProductionEndDay { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [SerieCreatedAt] - Date de création initiale de la série. Correspond au champ AX: CREATEDDATETIME.
    /// </summary>
    public DateTime? PSSerieCreatedAt { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsImported] - Indique si les données métier associées à la série ont été importées depuis un fichier Leitxx.mdb. False = non importée, True = importée.
    /// </summary>
    public bool PSIsImported { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsProductionValidated] - Indique si la série a été validée pour lancement. False = à valider, True = validée.
    /// </summary>
    public bool PSIsProductionValidated { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarOptimized] - Indique si la serie a ete optimisee pour la decoupe sur barres de chutes. False = non optimisee, True = optimisee.
    /// </summary>
    public bool PSIsDropBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsDropBarSupplied] - Indique si la serie a recu l&apos;&apos;approvisionnement en barres de chutes (stock de chutes). False = non approvisionnee, True = approvisionnee.
    /// </summary>
    public bool PSIsDropBarSupplied { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarOptimized] - Indique si la serie a ete optimisee pour la decoupe sur barres neuves. False = non optimisee, True = optimisee.
    /// </summary>
    public bool PSIsNewBarOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsNewBarSupplied] - Indique si la serie a recu l&apos;&apos;approvisionnement en barres neuves. False = non approvisionnee, True = approvisionnee.
    /// </summary>
    public bool PSIsNewBarSupplied { get; set; }

    public bool PSIsBarOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingStarted] - Indique si une des decoupes de la serie a ete realisee. False = non commencee, True = commencee.
    /// </summary>
    public bool PSIsCuttingStarted { get; set; }

    /// <summary>
    /// Source : [ProductionSeries] - Champ [IsCuttingCompleted] - Indique si l&apos;&apos;ensemble des decoupes de la serie ont ete realisees. False = non decoupee, True = decoupee.
    /// </summary>
    public bool PSIsCuttingCompleted { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [Id] - Clé technique interne.
    /// </summary>
    public int COId { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [IdOrder] - Numéro commande Tryba. Source : Tempor_Import.Aunummer.
    /// </summary>
    public int COIdOrder { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [IdProductionSeries] - FK série de production. Source : Tempor_Import.SerieNr.
    /// </summary>
    public int COIdProductionSeries { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [PartialSeriesIndex] - Index série partielle. Source : Tempor_Import.TeilserienIndex.
    /// </summary>
    public int COPartialSeriesIndex { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProjectNumber] - Numéro de projet. Source : Tempor_Import.Feld_10_171.
    /// </summary>
    public int? COProjectNumber { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProjectDesignation] - Sous-série + gamme + couleur. Source : Tempor_Import.Feld_10_288.
    /// </summary>
    public string? COProjectDesignation { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ManufacturingSite] - Site de fabrication. Source : Tempor_Import.Feld_10_073.
    /// </summary>
    public string? COManufacturingSite { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ManufacturingPlant] - Usine de fabrication. Source : Tempor_Import.Feld_10_081.
    /// </summary>
    public string? COManufacturingPlant { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [DeliveryDate] - Date de livraison. Source : Tempor_Import.Feld_10_072.
    /// </summary>
    public DateOnly? CODeliveryDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ShippingDate] - Date d’expédition. Source : Tempor_Import.Feld_10_213.
    /// </summary>
    public DateOnly? COShippingDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionStartDate] - Début production. Source : Tempor_Import.Feld_10_082.
    /// </summary>
    public DateOnly? COProductionStartDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionStartWeek] - Semaine début production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_243.
    /// </summary>
    public int? COProductionStartWeek { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndDate] - Fin production. Source : Tempor_Import.Feld_10_212.
    /// </summary>
    public DateOnly? COProductionEndDate { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndWeek] - Semaine fin production (AAWW ex 2503=2025 sem 03). Source : Tempor_Import.Feld_10_054.
    /// </summary>
    public int? COProductionEndWeek { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndWeekday] - Jour semaine fin production. Source : Tempor_Import.Feld_10_545.
    /// </summary>
    public int? COProductionEndWeekday { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [ProductionEndTourId] - Tournée fin production. Source : Tempor_Import.Feld_10_053.
    /// </summary>
    public string? COProductionEndTourId { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [OrderSponsor] - Commanditaire de la commande. Source : Tempor_Import.Feld_10_299.
    /// </summary>
    public string? COOrderSponsor { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointCode] - Numéro client principal. Source : Tempor_Import.Feld_10_326.
    /// </summary>
    public string? COMainSalesPointCode { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPoint] - Code client principal. Source : Tempor_Import.Feld_10_273.
    /// </summary>
    public string? COMainSalesPoint { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointName] - Point de vente principal. Source : Tempor_Import.Feld_10_110.
    /// </summary>
    public string? COMainSalesPointName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [MainSalesPointAddress] - Adresse client principal. Source : Tempor_Import.Feld_10_274.
    /// </summary>
    public string? COMainSalesPointAddress { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [SecondarySalesPointName] - Point de vente secondaire. Source : Tempor_Import.Feld_10_024.
    /// </summary>
    public string? COSecondarySalesPointName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerName] - Nom du client final. Source : Tempor_Import.Feld_10_163.
    /// </summary>
    public string? COCustomerName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerProjectName] - Nom du chantier. Source : Tempor_Import.Feld_10_083.
    /// </summary>
    public string? COCustomerProjectName { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerProjectDesignation] - Désignation projet. Source : Tempor_Import.Feld_10_002.
    /// </summary>
    public string? COCustomerProjectDesignation { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerStreet] - Rue du chantier. Source : Tempor_Import.Feld_10_087.
    /// </summary>
    public string? COCustomerStreet { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerCity] - Ville du chantier. Source : Tempor_Import.Feld_10_086.
    /// </summary>
    public string? COCustomerCity { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerZipCode] - Code postal du chantier. Source : Tempor_Import.Feld_10_085.
    /// </summary>
    public string? COCustomerZipCode { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [CustomerCountry] - Pays du chantier. Source : Tempor_Import.Feld_10_084.
    /// </summary>
    public string? COCustomerCountry { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [DeliveryPosition] - Position de livraison. Source : Tempor_Import.Feld_10_118.
    /// </summary>
    public string? CODeliveryPosition { get; set; }

    /// <summary>
    /// Source : [CustomerOrder] - Champ [QuaiZone] - Zone de quai. Source : Tempor_Import.Feld_10_184.
    /// </summary>
    public string? COQuaiZone { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [Id] - Identifiant technique du châssis (PK).
    /// </summary>
    public int PCId { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [IdCustomerOrder] - FK CustomerOrder. Source : Tempor_Import.Aunummer.
    /// </summary>
    public int PCIdCustomerOrder { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [PartialSeriesIndex] - Index de sous-série. Source : Tempor_Import.TeilserienIndex.
    /// </summary>
    public int PCPartialSeriesIndex { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OrderPosition] - Position du châssis dans la commande. Source : Tempor_Import.Pos.
    /// </summary>
    public short PCOrderPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [BarcodeId] - Identifiant code-barres chassis. Source : Tempor_Import.Feld_10_059.
    /// </summary>
    public string PCBarcodeId { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionChassis] - Champ [Quantity] - Quantité. Source : Tempor_Import.Wert_11.
    /// </summary>
    public short PCQuantity { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SeriesPosition] - Position dans la série. Source : Tempor_Import.Feld_10_030.
    /// </summary>
    public short PCSeriesPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [CustomerPosition] - Position client. Source : Tempor_Import.Feld_10_041.
    /// </summary>
    public string? PCCustomerPosition { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ProductFamily] - Famille produit. Source : Tempor_Import.Feld_10_048.
    /// </summary>
    public string PCProductFamily { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ElementHeight] - Hauteur élément. Source : Tempor_Import.Feld_10_032.
    /// </summary>
    public short? PCElementHeight { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ElementWidth] - Largeur élément. Source : Tempor_Import.Feld_10_031.
    /// </summary>
    public short? PCElementWidth { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [FrameWidthIncludingRV] - Largeur cadre incluant RV. Source : Tempor_Import.Feld_10_077.
    /// </summary>
    public short? PCFrameWidthIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [FrameHeightIncludingRV] - Hauteur cadre incluant RV. Source : Tempor_Import.Feld_10_078.
    /// </summary>
    public short? PCFrameHeightIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OuterWidthIncludingRV] - Largeur extérieure incluant RV. Source : Tempor_Import.Feld_10_079.
    /// </summary>
    public short? PCOuterWidthIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OuterHeightIncludingRV] - Hauteur extérieure incluant RV. Source : Tempor_Import.Feld_10_080.
    /// </summary>
    public short? PCOuterHeightIncludingRV { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WidthWithCorrectionAndMiterTip] - Largeur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_245.
    /// </summary>
    public decimal? PCWidthWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [HeightWithCorrectionAndMiterTip] - Hauteur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_246.
    /// </summary>
    public short? PCHeightWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [ColorNameIntExt] - Couleur intérieur/extérieur. Source : Tempor_Import.Feld_10_011.
    /// </summary>
    public string? PCColorNameIntExt { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WindowText] - Texte de fenêtre. Source : Tempor_Import.Feld_10_113.
    /// </summary>
    public string? PCWindowText { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SashDimensionsLeftRight] - Dimensions vantaux G/D. Source : Tempor_Import.Feld_10_012.
    /// </summary>
    public string? PCSashDimensionsLeftRight { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [WindowSystemCode] - Code système fenêtre. Source : Tempor_Import.Feld_10_019.
    /// </summary>
    public string? PCWindowSystemCode { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [CapacityZone] - Zone de capacité. Source : Tempor_Import.Feld_10_074.
    /// </summary>
    public string? PCCapacityZone { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SlidingType] - Type de coulissant. Source : Tempor_Import.Feld_10_233.
    /// </summary>
    public string? PCSlidingType { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SlidingTypeDetailed] - Type de coulissant détaillé. Source : Tempor_Import.Feld_10_234.
    /// </summary>
    public string? PCSlidingTypeDetailed { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OpeningTypeAbbreviation] - Abréviation type d’ouverture. Source : Tempor_Import.Feld_10_034.
    /// </summary>
    public string? PCOpeningTypeAbbreviation { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [OpeningTypeText] - Texte type d’ouverture. Source : Tempor_Import.Feld_10_013.
    /// </summary>
    public string? PCOpeningTypeText { get; set; }

    /// <summary>
    /// Source : [ProductionChassis] - Champ [SashPreset] - Ventaux prédéfinis. Source : Tempor_Import.Feld_10_282.
    /// </summary>
    public string? PCSashPreset { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [Id] - Identifiant technique du cadre/ouvrant (PK).
    /// </summary>
    public int PFSId { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [IdProductionChassis] - Identifiant technique du châssis parent (FK ProductionChassis).
    /// </summary>
    public int PFSIdProductionChassis { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [ComponentNumber] - Numéro du composant dans le chassis. Source : Tempor_Import.Wert_14.
    /// </summary>
    public short PFSComponentNumber { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [FrameSashWidth] - Largeur cadre/ouvrant. Source : Tempor_Import.Feld_10_038.
    /// </summary>
    public decimal? PFSFrameSashWidth { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [FrameSashHeight] - Hauteur cadre/ouvrant. Source : Tempor_Import.Feld_10_039.
    /// </summary>
    public decimal? PFSFrameSashHeight { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [OpeningTypeText] - Type d’ouverture (texte). Source : Tempor_Import.Feld_10_043.
    /// </summary>
    public string? PFSOpeningTypeText { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [OpeningDirectionIndicator] - Indicateur de sens d’ouverture. Source : Tempor_Import.Feld_10_045.
    /// </summary>
    public string? PFSOpeningDirectionIndicator { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SpecialOpeningTypeCode] - Code type d’ouverture spécifique. Source : Tempor_Import.Feld_10_125.
    /// </summary>
    public string? PFSSpecialOpeningTypeCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [AdjacentFramePartToSash] - Partie de cadre adjacente à l’ouvrant. Source : Tempor_Import.Feld_10_224.
    /// </summary>
    public string? PFSAdjacentFramePartToSash { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [FrameSashWidthTenths] - Largeur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_229.
    /// </summary>
    public decimal? PFSFrameSashWidthTenths { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [FrameSashHeightTenths] - Hauteur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_230.
    /// </summary>
    public decimal? PFSFrameSashHeightTenths { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [FrameThresholdCounterProfile] - Contre-profil cadre/seuil. Source : Tempor_Import.Feld_10_221.
    /// </summary>
    public string? PFSFrameThresholdCounterProfile { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [ReinforcementCode] - Code de renfort. Source : Tempor_Import.Feld_10_015.
    /// </summary>
    public string? PFSReinforcementCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [ReinforcementLength] - Longueur du renfort. Source : Tempor_Import.Feld_10_016.
    /// </summary>
    public short? PFSReinforcementLength { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [ReinforcementLength1NoGrid] - Longueur de renfort 1 sans trame. Source : Tempor_Import.Feld_10_583.
    /// </summary>
    public short? PFSReinforcementLength1NoGrid { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [ReinforcementLength2NoGrid] - Longueur de renfort 2 sans trame. Source : Tempor_Import.Feld_10_584.
    /// </summary>
    public short? PFSReinforcementLength2NoGrid { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [DisplayColorInside] - Couleur d’affichage intérieure. Source : Tempor_Import.Feld_10_585.
    /// </summary>
    public string? PFSDisplayColorInside { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [DisplayColorOutside] - Couleur d’affichage extérieure. Source : Tempor_Import.Feld_10_586.
    /// </summary>
    public string? PFSDisplayColorOutside { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [Seal] - Joint. Source : Tempor_Import.Feld_10_061.
    /// </summary>
    public string? PFSSeal { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SealColor] - Couleur du joint. Source : Tempor_Import.Feld_10_062.
    /// </summary>
    public string? PFSSealColor { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SealSystem] - Système de joint. Source : Tempor_Import.Feld_10_067.
    /// </summary>
    public string? PFSSealSystem { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [InnerSealSashFrame] - Joint ouvrant/cadre intérieur. Source : Tempor_Import.Feld_10_150.
    /// </summary>
    public string? PFSInnerSealSashFrame { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SealVariantText] - Texte de variante de joint. Source : Tempor_Import.Feld_10_564.
    /// </summary>
    public string? PFSSealVariantText { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SealVariantCode] - Code de variante de joint. Source : Tempor_Import.Feld_10_563.
    /// </summary>
    public string? PFSSealVariantCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [BeadSystemInnerSeal] - Joint intérieur issu du système de parcloses. Source : Tempor_Import.Feld_10_294.
    /// </summary>
    public string? PFSBeadSystemInnerSeal { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [PositionDataSealColor] - Couleur du joint issue des données de position. Source : Tempor_Import.Feld_10_489.
    /// </summary>
    public string? PFSPositionDataSealColor { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingSealText] - Texte pour le joint de vitrage. Source : Tempor_Import.Feld_10_017.
    /// </summary>
    public string? PFSGlazingSealText { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingAssignment] - Affectation du vitrage. Source : Tempor_Import.Feld_10_148.
    /// </summary>
    public string? PFSGlazingAssignment { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingCode] - Code vitrage. Source : Tempor_Import.Feld_10_560.
    /// </summary>
    public string? PFSGlazingCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingDimensions] - Dimensions du vitrage. Source : Tempor_Import.Feld_10_134.
    /// </summary>
    public string? PFSGlazingDimensions { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingText] - Texte vitrage. Source : Tempor_Import.Feld_10_018.
    /// </summary>
    public string? PFSGlazingText { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [GlazingBeadsPerSashFrame] - Parcloses par ouvrant/cadre. Source : Tempor_Import.Feld_10_137.
    /// </summary>
    public string? PFSGlazingBeadsPerSashFrame { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [BeadsHeight] - Hauteur des parcloses. Source : Tempor_Import.Feld_10_056.
    /// </summary>
    public decimal? PFSBeadsHeight { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [BeadsWidth] - Largeur des parcloses. Source : Tempor_Import.Feld_10_055.
    /// </summary>
    public decimal? PFSBeadsWidth { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [HardwareSystemText] - Texte pour le système de ferrures. Source : Tempor_Import.Feld_10_014.
    /// </summary>
    public string? PFSHardwareSystemText { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [HardwareSystemCode] - Code du système de ferrures. Source : Tempor_Import.Feld_10_023.
    /// </summary>
    public string? PFSHardwareSystemCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [HandlePosition] - Position de la poignée. Source : Tempor_Import.Feld_10_161.
    /// </summary>
    public string? PFSHandlePosition { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [SashHardwareIndicator] - Indicateur de ferrure d’ouvrant sinon global. Source : Tempor_Import.Feld_10_187.
    /// </summary>
    public string? PFSSashHardwareIndicator { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [MechanismCode] - Code de mécanisme (boîtier/entraînement). Source : Tempor_Import.Feld_10_215.
    /// </summary>
    public string? PFSMechanismCode { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [HardwareRabbetWidthTenths] - Largeur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_231.
    /// </summary>
    public decimal? PFSHardwareRabbetWidthTenths { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [HardwareRabbetHeightTenths] - Hauteur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_232.
    /// </summary>
    public decimal? PFSHardwareRabbetHeightTenths { get; set; }

    /// <summary>
    /// Source : [ProductionFrameSash] - Champ [CremoneType1] - Ferrage type crémone 1. Source : Tempor_Import.Feld_10_262.
    /// </summary>
    public string? PFSCremoneType1 { get; set; }

    /// <summary>
    /// Source : [SpatialPosition] - Champ [Id] - Identifiant technique unique de la position spatiale.
    /// </summary>
    public short? SPId { get; set; }

    /// <summary>
    /// Source : [SpatialPosition] - Champ [Code] - Code source issu de Tempor_Import.Feld_6.
    /// </summary>
    public string? SPCode { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [Id] - Identifiant technique de la pièce à découper (PK).
    /// </summary>
    public int PCPId { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IdProductionFrameSash] - Identifiant du composant châssis (cadre ou ouvrant) parent.
    /// </summary>
    public int PCPIdProductionFrameSash { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IdSpatialPosition] - Identifiant de la position spatiale dans le châssis. Source : Tempor_Import.Feld_6.
    /// </summary>
    public short? PCPIdSpatialPosition { get; set; }

    /// <summary>
    /// Source : [SpatialPosition] - Champ [Description] - Description fonctionnelle lisible de la position ou du type de piece.
    /// </summary>
    public string? SPDescription { get; set; }

    /// <summary>
    /// Source : [SpatialPosition] - Champ [Position] - Position spatiale normalisee (Haut, Bas, Gauche, Droite, Horizontal, Vertical, Croisillon, NA).
    /// </summary>
    public string? SPPosition { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IdArticleInternal] - Identifiant de l’article interne associé à la pièce à découper.
    /// </summary>
    public int? PCPIdArticleInternal { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IdProductionBar] - Identifiant de la barre de production associée à la découpe.
    /// </summary>
    public int? PCPIdProductionBar { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [LookCustomerOrderId] - Identifiant Look3E pour la Commande Client. Source. Source : Tempor_Import.Feld_10_205.
    /// </summary>
    public string PCPLookCustomerOrderId { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [LookChassisId] - Identifiant Look3E pour le Chassis. Source : Tempor_Import.Feld_10_513.
    /// </summary>
    public string? PCPLookChassisId { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [LookCutPieceId] - Identifiant Look3E pour la pièce à découper. Source : Tempor_Import.Feld_23.
    /// </summary>
    public string? PCPLookCutPieceId { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutBarcode] - Code-barre de la pièce. Source : Tempor_Import.Feld_10_165.
    /// </summary>
    public string PCPCutBarcode { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [SideIndex] - Indice côté (0 bas,1 gauche,2 haut,3 droite). Source : Tempor_Import.Feld_10_020.
    /// </summary>
    public short? PCPSideIndex { get; set; }

    /// <summary>
    /// Source : [SidePosition] - Champ [Description] - Designation lisible de la position laterale pour affichage et reporting.
    /// </summary>
    public string? SP1SideIndexDescription { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ComponentPieceNumber] - Numéro de pièce dans le composant. Source : Tempor_Import.Feld_10_114.
    /// </summary>
    public short? PCPComponentPieceNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ChassisPieceNumber] - Numéro de pièce dans le châssis. Source : Tempor_Import.Ref.
    /// </summary>
    public short? PCPChassisPieceNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CustomerOrderLineNumber] - Numéro de ligne de commande client. Source : Tempor_Import.Feld_10_227.
    /// </summary>
    public short? PCPCustomerOrderLineNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [PositionPieceNumber] - Numéro de pièce dans la position. Source : Tempor_Import.Feld_10_006.
    /// </summary>
    public short? PCPPositionPieceNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [SequentialPieceNumber] - Numéro séquentiel de pièce. Source : Tempor_Import.Feld_10_036.
    /// </summary>
    public short? PCPSequentialPieceNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [PartialSeriesSequentialPieceNumber] - Numéro séquentiel dans la série partielle. Source : Tempor_Import.Feld_10_277.
    /// </summary>
    public short? PCPPartialSeriesSequentialPieceNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CustomerOrderLineNumber2] - Numéro de ligne commande client (variante). Source : Tempor_Import.Feld_10_239.
    /// </summary>
    public short? PCPCustomerOrderLineNumber2 { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarFamilyCode] - Code famille de barre. Source : Tempor_Import.Wert_2.
    /// </summary>
    public int? PCPBarFamilyCode { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarProductCodeToPrint] - Code produit à imprimer sur la barre. Source : Tempor_Import.CodeNat.
    /// </summary>
    public string? PCPBarProductCodeToPrint { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarReference] - Référence de la barre. Source : Tempor_Import.Feld_9.
    /// </summary>
    public string? PCPBarReference { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarReference] + [ProfileColorCodeInOut] - Colonne calculee : concatenation &apos;BarReference | ProfileColorCodeInOut&apos;. BarReference : Référence de la barre. Source : Tempor_Import.Feld_9. ProfileColorCodeInOut : Code couleur profil intérieur/extérieur. Source : Tempor_Import.Feld_10_026.
    /// </summary>
    public string PCPReferenceColor { get; set; } = null!;

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarProductFamilyName] - Désignation famille de barre. Source : Tempor_Import.Feld_40.
    /// </summary>
    public string? PCPBarProductFamilyName { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarColorCodeInOut] - Code couleur barre intérieur/extérieur. Source : Tempor_Import.Feld_8.
    /// </summary>
    public string? PCPBarColorCodeInOut { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarHeight] - Hauteur de la barre. Source : Tempor_Import.Wert_41.
    /// </summary>
    public decimal? PCPBarHeight { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarWidth] - Largeur de la barre. Source : Tempor_Import.Wert_42.
    /// </summary>
    public decimal? PCPBarWidth { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [BarLength] - Longueur brute de la barre. Source : Tempor_Import.Wert_21.
    /// </summary>
    public int? PCPBarLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ScrapMaxLength] - Longueur maximale de chute autorisée. Source : Tempor_Import.Wert_39.
    /// </summary>
    public int? PCPScrapMaxLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [SawCutLength] - Longueur de coupe scie. Source : Tempor_Import.Wert_34.
    /// </summary>
    public int? PCPSawCutLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [FinishingCutLength] - Longueur de coupe finition. Source : Tempor_Import.Wert_33.
    /// </summary>
    public int? PCPFinishingCutLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [DiePitch] - Pas de filière. Source : Tempor_Import.Wert_37.
    /// </summary>
    public int? PCPDiePitch { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [OptimizationMinLength] - Longueur minimale pour optimisation. Source : Tempor_Import.Wert_38.
    /// </summary>
    public int? PCPOptimizationMinLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [RemainingUntilLength] - Longueur restante avant seuil. Source : Tempor_Import.Wert_36.
    /// </summary>
    public int? PCPRemainingUntilLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [MachineCode] - Code machine de découpe. Source : Tempor_Import.Feld_10_021.
    /// </summary>
    public string? PCPMachineCode { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ArticleCounter] - Compteur article Leitxx. Source : Tempor_Import.Feld_10_302.
    /// </summary>
    public int? PCPArticleCounter { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileNumber] - Numéro de profil. Source : Tempor_Import.Feld_10_066.
    /// </summary>
    public string? PCPProfileNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileNumberForMachine] - Numéro de profil machine. Source : Tempor_Import.Feld_10_330.
    /// </summary>
    public string? PCPProfileNumberForMachine { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileCodeToPrint] - Code profil à imprimer. Source : Tempor_Import.Feld_10_027.
    /// </summary>
    public string? PCPProfileCodeToPrint { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileName] - Nom du profil. Source : Tempor_Import.Feld_10_100.
    /// </summary>
    public string? PCPProfileName { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileLength] - Longueur du profil. Source : Tempor_Import.Feld_10_010.
    /// </summary>
    public decimal? PCPProfileLength { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileLengthIncludingFOD] - Longueur du profil avec FOD. Source : Tempor_Import.Feld_10_565.
    /// </summary>
    public decimal? PCPProfileLengthIncludingFOD { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [DaylightLengthWithAngleAndCorrection] - Longueur jour avec angle et correction. Source : Tempor_Import.Feld_10_267.
    /// </summary>
    public int? PCPDaylightLengthWithAngleAndCorrection { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileWidth] - Largeur du profil. Source : Tempor_Import.Feld_10_051.
    /// </summary>
    public decimal? PCPProfileWidth { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileHeight] - Hauteur du profil. Source : Tempor_Import.Feld_10_075.
    /// </summary>
    public decimal? PCPProfileHeight { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileColorInside] - Couleur intérieure du profil. Source : Tempor_Import.Feld_10_088.
    /// </summary>
    public string? PCPProfileColorInside { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileColorOutside] - Couleur extérieure du profil. Source : Tempor_Import.Feld_10_089.
    /// </summary>
    public string? PCPProfileColorOutside { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ProfileColorCodeInOut] - Code couleur profil intérieur/extérieur. Source : Tempor_Import.Feld_10_026.
    /// </summary>
    public string? PCPProfileColorCodeInOut { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutDimension] - Dimension de coupe. Source : Tempor_Import.Wert_6
    /// </summary>
    public decimal? PCPCutDimension { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutInclinationLeft] - Inclinaison de coupe gauche. Source : Tempor_Import.Feld_10_104.
    /// </summary>
    public short? PCPCutInclinationLeft { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutInclinationRight] - Inclinaison de coupe droite. Source : Tempor_Import.Feld_10_105.
    /// </summary>
    public short? PCPCutInclinationRight { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutPivotLeft] - Pivot de coupe gauche. Source : Tempor_Import.Feld_10_111.
    /// </summary>
    public short? PCPCutPivotLeft { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutPivotRight] - Pivot de coupe droite. Source : Tempor_Import.Feld_10_112.
    /// </summary>
    public short? PCPCutPivotRight { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [TrolleyNumber] - Numéro de chariot. Source : Tempor_Import.Wagen.
    /// </summary>
    public short? PCPTrolleyNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [TrolleyLevel] - Niveau du chariot. Source : Tempor_Import.Etage.
    /// </summary>
    public short? PCPTrolleyLevel { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [TrolleySlot] - Emplacement du chariot. Source : Tempor_Import.Fach.
    /// </summary>
    public short? PCPTrolleySlot { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [AssemblyCode] - Code de montage. Source : Tempor_Import.Feld_10_009.
    /// </summary>
    public string? PCPAssemblyCode { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [WindowCounter] - Compteur de fenêtre. Source : Tempor_Import.Feld_10_052.
    /// </summary>
    public short? PCPWindowCounter { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ElementCounter] - Compteur d’élément. Source : Tempor_Import.Feld_10_057.
    /// </summary>
    public short? PCPElementCounter { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [FrameFieldNumber] - Numéro de champ du cadre. Source : Tempor_Import.Feld_10_058.
    /// </summary>
    public short? PCPFrameFieldNumber { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [ConnectionProfileCode] - Code profil de liaison. Source : Tempor_Import.Feld_10_022.
    /// </summary>
    public string? PCPConnectionProfileCode { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [DrainageCodeUsedForCalculation] - Code d’évacuation pour calcul. Source : Tempor_Import.Feld_10_181.
    /// </summary>
    public string? PCPDrainageCodeUsedForCalculation { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [TotalQuantityForPosition] - Quantité totale pour la position. Source : Tempor_Import.Feld_10_133.
    /// </summary>
    public short? PCPTotalQuantityForPosition { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [TotalElementsCount] - Nombre total d’éléments. Source : Tempor_Import.Feld_10_144.
    /// </summary>
    public short? PCPTotalElementsCount { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [AssociatedArticleReferenceRight] - Référence article associée à droite. Source : Tempor_Import.Feld_10_346.
    /// </summary>
    public string? PCPAssociatedArticleReferenceRight { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [AssociatedArticleReferenceLeft] - Référence article associée à gauche. Source : Tempor_Import.Feld_10_347.
    /// </summary>
    public string? PCPAssociatedArticleReferenceLeft { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutPositionInBar] - Position de la decoupe au sein de la barre (ordre de coupe).
    /// </summary>
    public int? PCPCutPositionInBar { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IsOptimizedTemp] - Indicateur temporaire d&apos;&apos;optimisation de la decoupe (usage transitoire, en parallele de IsOptimized).
    /// </summary>
    public bool PCPIsOptimizedTemp { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IsOptimized] - Indique si la decoupe a ete selectionnee par le processus d&apos;&apos;optimisation.
    /// </summary>
    public bool PCPIsOptimized { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IsBarSupplied] - Indique si la barre necessaire a la decoupe a ete approvisionnee.
    /// </summary>
    public bool PCPIsBarSupplied { get; set; }

    public bool PCPIsBarOutOfStock { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IsCut] - Indique si la decoupe a ete realisee.
    /// </summary>
    public bool PCPIsCut { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutStartedAt] - Date et heure de debut de la decoupe.
    /// </summary>
    public DateTime? PCPCutStartedAt { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [CutFinishedAt] - Date et heure de validation de la decoupe.
    /// </summary>
    public DateTime? PCPCutFinishedAt { get; set; }

    /// <summary>
    /// Source : [ProductionCutPiece] - Champ [IsDeleted] - Indicateur de suppression logique (soft delete).
    /// </summary>
    public bool PCPIsDeleted { get; set; }
}
