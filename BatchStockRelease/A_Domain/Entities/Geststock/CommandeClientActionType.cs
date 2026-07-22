using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CommandeClientActionType
{
    public int Id { get; set; }

    public string Action { get; set; } = null!;

    public string? Controle { get; set; }

    public int AdvAff { get; set; }

    public int IsModif { get; set; }

    public int IdCcStatut { get; set; }
}
