using System;
using System.Collections.Generic;

namespace LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

/// <summary>
/// Stores RAL color references including designations and various color code formats (Hex, RGB, CMYK).
/// </summary>
public partial class ColorRal
{
    public int Id { get; set; }

    /// <summary>
    /// English color name as per the RAL standard.
    /// </summary>
    public string? DesignationEn { get; set; }

    /// <summary>
    /// French color name as per the RAL standard.
    /// </summary>
    public string? DesignationFr { get; set; }

    public string? HexCode { get; set; }

    public string? RgbCode { get; set; }

    public string? CmykCode { get; set; }

    public string? BaseReference { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<ColorRalFinish> ColorRalFinishIdExternalRalNavigations { get; set; } = new List<ColorRalFinish>();

    public virtual ICollection<ColorRalFinish> ColorRalFinishIdInternalRalNavigations { get; set; } = new List<ColorRalFinish>();
}
