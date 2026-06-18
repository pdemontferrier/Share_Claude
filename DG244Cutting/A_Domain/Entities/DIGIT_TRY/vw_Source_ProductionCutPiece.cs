using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ProductionCutPiece
{
    public int IdProductionFrameSash { get; set; }

    public short? ChassisPieceNumber { get; set; }

    public string? Look3EId { get; set; }

    public string? CommandId { get; set; }

    public string? PositionId { get; set; }

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

    public short? BarLength { get; set; }

    public short? ScrapMaxLength { get; set; }

    public short? SawCutLength { get; set; }

    public short? FinishingCutLength { get; set; }

    public short? DiePitch { get; set; }

    public short? OptimizationMinLength { get; set; }

    public short? RemainingUntilLength { get; set; }

    public string? MachineCode { get; set; }

    public int? ArticleCounter { get; set; }

    public string? ProfileNumber { get; set; }

    public string? ProfileNumberForMachine { get; set; }

    public string? ProfileCodeToPrint { get; set; }

    public string? ProfileName { get; set; }

    public decimal? ProfileLength { get; set; }

    public decimal? ProfileLengthIncludingFOD { get; set; }

    public short? DaylightLengthWithAngleAndCorrection { get; set; }

    public decimal? ProfileWidth { get; set; }

    public decimal? ProfileHeight { get; set; }

    public string? ProfileColorInside { get; set; }

    public string? ProfileColorOutside { get; set; }

    public string? ProfileColorCodeInOut { get; set; }

    public string? CutBarcode { get; set; }

    public decimal? CutDimension { get; set; }

    public short? CutInclinationLeft { get; set; }

    public short? CutInclinationRight { get; set; }

    public short? CutPivotLeft { get; set; }

    public short? CutPivotRight { get; set; }

    public short? TrolleyNumber { get; set; }

    public bool? TrolleyLevel { get; set; }

    public short? TrolleySlot { get; set; }

    public string? AssemblyCode { get; set; }

    public short? SpatialPositionInChassis { get; set; }

    public short? SideIndex { get; set; }

    public short? SpacePositionIndex { get; set; }

    public bool? WindowCounter { get; set; }

    public bool? ElementCounter { get; set; }

    public bool? FrameFieldNumber { get; set; }

    public string? ConnectionProfileCode { get; set; }

    public string? DrainageCodeUsedForCalculation { get; set; }

    public bool? TotalQuantityForPosition { get; set; }

    public bool? TotalElementsCount { get; set; }

    public string? AssociatedArticleReferenceRight { get; set; }

    public string? AssociatedArticleReferenceLeft { get; set; }

    public int IdArticleInternal { get; set; }
}
