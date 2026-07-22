using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class PickingEmplacement
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
}
