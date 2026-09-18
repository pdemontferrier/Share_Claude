using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class CommandeClientModification
{
    public int Id { get; set; }

    public string NumProjet { get; set; } = null!;

    public string TypeModif { get; set; } = null!;

    public string NouvelleValeur { get; set; } = null!;

    public DateTime DateModification { get; set; }

    public int UserId { get; set; }
}
