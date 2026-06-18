using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ProductionCutPiece_Missing
{
    public int IdProductionFrameSash { get; set; }

    public short? IdSpatialPosition { get; set; }

    public int IdArticleInternal { get; set; }

    public string? LookCustomerOrderId { get; set; }

    public string? LookChassisId { get; set; }

    public string? LookCutPieceId { get; set; }

    public string? CutBarcode { get; set; }

    public short? SideIndex { get; set; }

    public short? ComponentPieceNumber { get; set; }

    public short? ChassisPieceNumber { get; set; }

    public short? CustomerOrderLineNumber { get; set; }

    public short? PositionPieceNumber { get; set; }

    public short? SequentialPieceNumber { get; set; }

    public short? PartialSeriesSequentialPieceNumber { get; set; }

    public short? CustomerOrderLineNumber2 { get; set; }

    public int? BarFamilyCode { get; set; }

    public string? BarProductCodeToPrint { get; set; }

    public string? BarReference { get; set; }

    public string? BarProductFamilyName { get; set; }

    public string? BarColorCodeInOut { get; set; }

    public decimal? BarHeight { get; set; }

    public decimal? BarWidth { get; set; }

    public int? BarLength { get; set; }

    public int? ScrapMaxLength { get; set; }

    public int? SawCutLength { get; set; }

    public int? FinishingCutLength { get; set; }

    public int? DiePitch { get; set; }

    public int? OptimizationMinLength { get; set; }

    public int? RemainingUntilLength { get; set; }

    public string? MachineCode { get; set; }

    public int? ArticleCounter { get; set; }

    public string? ProfileNumber { get; set; }

    public string? ProfileNumberForMachine { get; set; }

    public string? ProfileCodeToPrint { get; set; }

    public string? ProfileName { get; set; }

    public decimal? ProfileLength { get; set; }

    public decimal? ProfileLengthIncludingFOD { get; set; }

    public int? DaylightLengthWithAngleAndCorrection { get; set; }

    public decimal? ProfileWidth { get; set; }

    public decimal? ProfileHeight { get; set; }

    public string? ProfileColorInside { get; set; }

    public string? ProfileColorOutside { get; set; }

    public string? ProfileColorCodeInOut { get; set; }

    public decimal? CutDimension { get; set; }

    public short? CutInclinationLeft { get; set; }

    public short? CutInclinationRight { get; set; }

    public short? CutPivotLeft { get; set; }

    public short? CutPivotRight { get; set; }

    public short? TrolleyNumber { get; set; }

    public short? TrolleyLevel { get; set; }

    public short? TrolleySlot { get; set; }

    public string? AssemblyCode { get; set; }

    public short? WindowCounter { get; set; }

    public short? ElementCounter { get; set; }

    public short? FrameFieldNumber { get; set; }

    public string? ConnectionProfileCode { get; set; }

    public string? DrainageCodeUsedForCalculation { get; set; }

    public short? TotalQuantityForPosition { get; set; }

    public short? TotalElementsCount { get; set; }

    public string? AssociatedArticleReferenceRight { get; set; }

    public string? AssociatedArticleReferenceLeft { get; set; }

    public long? rn_CutBarcode { get; set; }

    public long? rn_CutPieceLookId { get; set; }
}
