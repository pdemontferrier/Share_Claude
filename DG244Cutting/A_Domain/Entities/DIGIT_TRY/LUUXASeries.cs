using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class LUUXASeries
{
    public string SERIALNOSTR { get; set; } = null!;

    public long RECID { get; set; }

    public int RECVERSION { get; set; }

    public string EEEA_SERIALDESCRIPTION { get; set; } = null!;

    public DateTime EEEA_SERIALPLANDATE { get; set; }

    public DateTime ATWIN_PRODUCTIONENDDATE { get; set; }

    public DateTime CREATEDDATETIME { get; set; }
}
