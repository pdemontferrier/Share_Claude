using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ProductionCutPiece_Control_Coherence
{
    public int TemporImportId { get; set; }

    public string? LookCutPieceId { get; set; }

    public string? CutBarcode { get; set; }

    public int? SourceIdSerialNumber { get; set; }

    public int? SourceIdOrder { get; set; }

    public string? SourceBarcodeId { get; set; }

    public string? SourceReference { get; set; }

    public string? SourceIdColorRalFinish { get; set; }

    public string? SourceSpatialPositionCode { get; set; }

    public int? OrderPosition { get; set; }

    public short? ComponentNumber { get; set; }

    public long? rn { get; set; }

    public int? ProductionChassisId { get; set; }

    public string? ProductionChassisBarcodeId { get; set; }

    public int? ProductionFrameSashId { get; set; }

    public int? IdProductionChassis { get; set; }

    public short? ProductionFrameSashComponentNumber { get; set; }

    public int? ArticleReferenceId { get; set; }

    public string? ArticleReferenceCode { get; set; }

    public string? ColorRalFinishId { get; set; }

    public int? ArticleInternalId { get; set; }

    public short? SpatialPositionId { get; set; }

    public string? SpatialPositionCode { get; set; }

    public string ChassisStatus { get; set; } = null!;

    public string FrameSashStatus { get; set; } = null!;

    public string ArticleReferenceStatus { get; set; } = null!;

    public string ColorStatus { get; set; } = null!;

    public string ArticleInternalStatus { get; set; } = null!;

    public string SpatialPositionStatus { get; set; } = null!;
}
