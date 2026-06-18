using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionCutPiece_Full
{
    public int PSId { get; set; }

    public int PSIdSerialNumber { get; set; }

    public long PSIdRec { get; set; }

    public int PSRecVersion { get; set; }

    public string PSDescription { get; set; } = null!;

    public DateTime? PSProductionStartDate { get; set; }

    public DateTime? PSProductionEndDate { get; set; }

    public short PSProductionEndDay { get; set; }

    public DateTime? PSSerieCreatedAt { get; set; }

    public bool PSIsImported { get; set; }

    public bool PSIsProductionValidated { get; set; }

    public bool PSIsDropBarSupplied { get; set; }

    public bool PSIsNewBarSupplied { get; set; }

    public bool PSIsCuttingStarted { get; set; }

    public bool PSIsCuttingCompleted { get; set; }

    public int COId { get; set; }

    public int COIdOrder { get; set; }

    public int COIdProductionSeries { get; set; }

    public int COPartialSeriesIndex { get; set; }

    public int? COProjectNumber { get; set; }

    public string? COProjectDesignation { get; set; }

    public string? COManufacturingSite { get; set; }

    public string? COManufacturingPlant { get; set; }

    public DateOnly? CODeliveryDate { get; set; }

    public DateOnly? COShippingDate { get; set; }

    public DateOnly? COProductionStartDate { get; set; }

    public int? COProductionStartWeek { get; set; }

    public DateOnly? COProductionEndDate { get; set; }

    public int? COProductionEndWeek { get; set; }

    public int? COProductionEndWeekday { get; set; }

    public string? COProductionEndTourId { get; set; }

    public string? COOrderSponsor { get; set; }

    public string? COMainSalesPointCode { get; set; }

    public string? COMainSalesPoint { get; set; }

    public string? COMainSalesPointName { get; set; }

    public string? COMainSalesPointAddress { get; set; }

    public string? COSecondarySalesPointName { get; set; }

    public string? COCustomerName { get; set; }

    public string? COCustomerProjectName { get; set; }

    public string? COCustomerProjectDesignation { get; set; }

    public string? COCustomerStreet { get; set; }

    public string? COCustomerCity { get; set; }

    public string? COCustomerZipCode { get; set; }

    public string? COCustomerCountry { get; set; }

    public string? CODeliveryPosition { get; set; }

    public string? COQuaiZone { get; set; }

    public int PCId { get; set; }

    public int PCIdCustomerOrder { get; set; }

    public int PCPartialSeriesIndex { get; set; }

    public short PCOrderPosition { get; set; }

    public string PCBarcodeId { get; set; } = null!;

    public short PCQuantity { get; set; }

    public short PCSeriesPosition { get; set; }

    public string? PCCustomerPosition { get; set; }

    public string PCProductFamily { get; set; } = null!;

    public short? PCElementHeight { get; set; }

    public short? PCElementWidth { get; set; }

    public short? PCFrameWidthIncludingRV { get; set; }

    public short? PCFrameHeightIncludingRV { get; set; }

    public short? PCOuterWidthIncludingRV { get; set; }

    public short? PCOuterHeightIncludingRV { get; set; }

    public decimal? PCWidthWithCorrectionAndMiterTip { get; set; }

    public short? PCHeightWithCorrectionAndMiterTip { get; set; }

    public string? PCColorNameIntExt { get; set; }

    public string? PCWindowText { get; set; }

    public string? PCSashDimensionsLeftRight { get; set; }

    public string? PCWindowSystemCode { get; set; }

    public string? PCCapacityZone { get; set; }

    public string? PCSlidingType { get; set; }

    public string? PCSlidingTypeDetailed { get; set; }

    public string? PCOpeningTypeAbbreviation { get; set; }

    public string? PCOpeningTypeText { get; set; }

    public string? PCSashPreset { get; set; }

    public int PFSId { get; set; }

    public int PFSIdProductionChassis { get; set; }

    public short PFSComponentNumber { get; set; }

    public decimal? PFSFrameSashWidth { get; set; }

    public decimal? PFSFrameSashHeight { get; set; }

    public string? PFSOpeningTypeText { get; set; }

    public string? PFSOpeningDirectionIndicator { get; set; }

    public string? PFSSpecialOpeningTypeCode { get; set; }

    public string? PFSAdjacentFramePartToSash { get; set; }

    public decimal? PFSFrameSashWidthTenths { get; set; }

    public decimal? PFSFrameSashHeightTenths { get; set; }

    public string? PFSFrameThresholdCounterProfile { get; set; }

    public string? PFSReinforcementCode { get; set; }

    public short? PFSReinforcementLength { get; set; }

    public short? PFSReinforcementLength1NoGrid { get; set; }

    public short? PFSReinforcementLength2NoGrid { get; set; }

    public string? PFSDisplayColorInside { get; set; }

    public string? PFSDisplayColorOutside { get; set; }

    public string? PFSSeal { get; set; }

    public string? PFSSealColor { get; set; }

    public string? PFSSealSystem { get; set; }

    public string? PFSInnerSealSashFrame { get; set; }

    public string? PFSSealVariantText { get; set; }

    public string? PFSSealVariantCode { get; set; }

    public string? PFSBeadSystemInnerSeal { get; set; }

    public string? PFSPositionDataSealColor { get; set; }

    public string? PFSGlazingSealText { get; set; }

    public string? PFSGlazingAssignment { get; set; }

    public string? PFSGlazingCode { get; set; }

    public string? PFSGlazingDimensions { get; set; }

    public string? PFSGlazingText { get; set; }

    public string? PFSGlazingBeadsPerSashFrame { get; set; }

    public decimal? PFSBeadsHeight { get; set; }

    public decimal? PFSBeadsWidth { get; set; }

    public string? PFSHardwareSystemText { get; set; }

    public string? PFSHardwareSystemCode { get; set; }

    public string? PFSHandlePosition { get; set; }

    public string? PFSSashHardwareIndicator { get; set; }

    public string? PFSMechanismCode { get; set; }

    public decimal? PFSHardwareRabbetWidthTenths { get; set; }

    public decimal? PFSHardwareRabbetHeightTenths { get; set; }

    public string? PFSCremoneType1 { get; set; }

    public short? SPId { get; set; }

    public string? SPCode { get; set; }

    public int PCPId { get; set; }

    public int PCPIdProductionFrameSash { get; set; }

    public short? PCPIdSpatialPosition { get; set; }

    public string? SPDescription { get; set; }

    public string? SPPosition { get; set; }

    public int? PCPIdArticleInternal { get; set; }

    public int? PCPIdProductionBar { get; set; }

    public string PCPLookCustomerOrderId { get; set; } = null!;

    public string? PCPLookChassisId { get; set; }

    public string? PCPLookCutPieceId { get; set; }

    public string PCPCutBarcode { get; set; } = null!;

    public short? PCPSideIndex { get; set; }

    public string? SP1SideIndexDescription { get; set; }

    public short? PCPComponentPieceNumber { get; set; }

    public short? PCPChassisPieceNumber { get; set; }

    public short? PCPCustomerOrderLineNumber { get; set; }

    public short? PCPPositionPieceNumber { get; set; }

    public short? PCPSequentialPieceNumber { get; set; }

    public short? PCPPartialSeriesSequentialPieceNumber { get; set; }

    public short? PCPCustomerOrderLineNumber2 { get; set; }

    public int? PCPBarFamilyCode { get; set; }

    public string? PCPBarProductCodeToPrint { get; set; }

    public string? PCPBarReference { get; set; }

    public string? PCPBarProductFamilyName { get; set; }

    public string? PCPBarColorCodeInOut { get; set; }

    public decimal? PCPBarHeight { get; set; }

    public decimal? PCPBarWidth { get; set; }

    public int? PCPBarLength { get; set; }

    public int? PCPScrapMaxLength { get; set; }

    public int? PCPSawCutLength { get; set; }

    public int? PCPFinishingCutLength { get; set; }

    public int? PCPDiePitch { get; set; }

    public int? PCPOptimizationMinLength { get; set; }

    public int? PCPRemainingUntilLength { get; set; }

    public string? PCPMachineCode { get; set; }

    public int? PCPArticleCounter { get; set; }

    public string? PCPProfileNumber { get; set; }

    public string? PCPProfileNumberForMachine { get; set; }

    public string? PCPProfileCodeToPrint { get; set; }

    public string? PCPProfileName { get; set; }

    public decimal? PCPProfileLength { get; set; }

    public decimal? PCPProfileLengthIncludingFOD { get; set; }

    public int? PCPDaylightLengthWithAngleAndCorrection { get; set; }

    public decimal? PCPProfileWidth { get; set; }

    public decimal? PCPProfileHeight { get; set; }

    public string? PCPProfileColorInside { get; set; }

    public string? PCPProfileColorOutside { get; set; }

    public string? PCPProfileColorCodeInOut { get; set; }

    public decimal? PCPCutDimension { get; set; }

    public short? PCPCutInclinationLeft { get; set; }

    public short? PCPCutInclinationRight { get; set; }

    public short? PCPCutPivotLeft { get; set; }

    public short? PCPCutPivotRight { get; set; }

    public short? PCPTrolleyNumber { get; set; }

    public short? PCPTrolleyLevel { get; set; }

    public short? PCPTrolleySlot { get; set; }

    public string? PCPAssemblyCode { get; set; }

    public short? PCPWindowCounter { get; set; }

    public short? PCPElementCounter { get; set; }

    public short? PCPFrameFieldNumber { get; set; }

    public string? PCPConnectionProfileCode { get; set; }

    public string? PCPDrainageCodeUsedForCalculation { get; set; }

    public short? PCPTotalQuantityForPosition { get; set; }

    public short? PCPTotalElementsCount { get; set; }

    public string? PCPAssociatedArticleReferenceRight { get; set; }

    public string? PCPAssociatedArticleReferenceLeft { get; set; }

    public bool PCPIsOptimized { get; set; }

    public bool PCPIsBarSupplied { get; set; }

    public bool PCPIsComment { get; set; }

    public bool PCPIsCut { get; set; }

    public DateTime? PCPCutStartedAt { get; set; }

    public DateTime? PCPCutFinishedAt { get; set; }
}
