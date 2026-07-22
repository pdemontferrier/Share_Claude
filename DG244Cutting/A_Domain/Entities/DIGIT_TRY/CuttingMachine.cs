using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table storing configuration data for each cutting machine in the aluminium workshop.
/// </summary>
public partial class CuttingMachine
{
    public int Id { get; set; }

    /// <summary>
    /// Unique code identifying the cutting machine (e.g., DG244_01).
    /// </summary>
    public string MachineCode { get; set; } = null!;

    public string? Designation { get; set; }

    public string? ConsoleName { get; set; }

    public string? ConsoleIpAddress { get; set; }

    public string? ConsoleComPort { get; set; }

    public string? PcName { get; set; }

    public string? PcIpAddress { get; set; }

    public string? PcComPort { get; set; }

    public string? PrinterName { get; set; }

    public string? PrinterIpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ArticleReference> ArticleReferences { get; set; } = new List<ArticleReference>();
}
