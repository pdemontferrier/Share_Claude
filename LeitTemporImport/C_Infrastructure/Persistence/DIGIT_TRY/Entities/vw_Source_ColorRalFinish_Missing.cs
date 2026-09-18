using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

public partial class vw_Source_ColorRalFinish_Missing
{
    public string? Id { get; set; }

    public int? IdInternalRal { get; set; }

    public string? IdInternalFinish { get; set; }

    public int? IdExternalRal { get; set; }

    public string? IdExternalFinish { get; set; }
}
