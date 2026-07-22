using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class Stock
{
    public int Id { get; set; }

    public int IdArticleInterne { get; set; }

    public string NumCassette { get; set; } = null!;

    public double Quantite { get; set; }

    public DateOnly DateInventaire { get; set; }

    public DateTime DateAccessible { get; set; }

    public bool? Accessible { get; set; }
}
