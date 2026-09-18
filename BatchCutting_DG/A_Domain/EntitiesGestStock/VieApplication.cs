using System;
using System.Collections.Generic;

namespace BatchCutting_DG.A_Domain.EntitiesGestStock;

public partial class VieApplication
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;

    public string Fonction { get; set; } = null!;

    public int Ordre { get; set; }

    public bool Accessible { get; set; }

    public string? Image { get; set; }

    public sbyte? TjsVisible { get; set; }

    public sbyte? PleinEcran { get; set; }

    public int? IdAppliParent { get; set; }
}
