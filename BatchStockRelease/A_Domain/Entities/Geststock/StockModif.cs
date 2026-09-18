using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class StockModif
{
    public int Id { get; set; }

    public string Action { get; set; } = null!;

    public string? Emplacement { get; set; }

    public int? AncienneQuantite { get; set; }

    public int? NouvelleQuantite { get; set; }

    public string Changement { get; set; } = null!;

    public int IdUser { get; set; }

    public DateTime DateModification { get; set; }

    public int IdArticleInterne { get; set; }
}
