using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table ENUM des couleurs de production utilisées pour qualifier létat dune série en fonction du jour de fin de production.
/// </summary>
public partial class ProductionColorLabelType
{
    /// <summary>
    /// Identifiant ENUM : 1=Bleu, 2=Orange, 3=Jaune, 4=Rouge, 5=Rose.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Libellé textuel de la couleur de production.
    /// </summary>
    public string Label { get; set; } = null!;

    public virtual ICollection<ProductionSeries> ProductionSeries { get; set; } = new List<ProductionSeries>();
}
