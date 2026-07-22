using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ProductionChassi
{
    public int IdCustomerOrder { get; set; }

    public int? PartialSeriesIndex { get; set; }

    public short? OrderPosition { get; set; }

    public string? BarcodeId { get; set; }

    public int? Quantity { get; set; }

    public short? SeriesPosition { get; set; }

    public string? CustomerPosition { get; set; }

    public string? ProductFamily { get; set; }

    public short? ElementHeight { get; set; }

    public short? ElementWidth { get; set; }

    public short? FrameWidthIncludingRV { get; set; }

    public short? FrameHeightIncludingRV { get; set; }

    public short? OuterWidthIncludingRV { get; set; }

    public short? OuterHeightIncludingRV { get; set; }

    public decimal? WidthWithCorrectionAndMiterTip { get; set; }

    public int? HeightWithCorrectionAndMiterTip { get; set; }

    public string? ColorNameIntExt { get; set; }

    public string? WindowText { get; set; }

    public string? SashDimensionsLeftRight { get; set; }

    public string? WindowSystemCode { get; set; }

    public string? CapacityZone { get; set; }

    public string? SlidingType { get; set; }

    public string? SlidingTypeDetailed { get; set; }

    public string? OpeningTypeAbbreviation { get; set; }

    public string? OpeningTypeText { get; set; }

    public string? SashPreset { get; set; }
}
