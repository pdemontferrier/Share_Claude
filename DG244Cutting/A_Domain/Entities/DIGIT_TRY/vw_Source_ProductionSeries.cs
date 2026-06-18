using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ProductionSeries
{
    public int? IdSerialNumber { get; set; }

    public long IdRec { get; set; }

    public int RecVersion { get; set; }

    public string? Description { get; set; }

    public DateTime? ProductionStartDate { get; set; }

    public DateTime? ProductionEndDate { get; set; }

    public byte? ProductionEndDay { get; set; }

    public DateTime? SerieCreatedAt { get; set; }
}
