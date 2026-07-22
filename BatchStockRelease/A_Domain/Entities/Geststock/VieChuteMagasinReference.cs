using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class VieChuteMagasinReference
{
    public int? IdArticleInterne { get; set; }

    public int IdChuteMagasin { get; set; }

    public string? CodeBarre { get; set; }

    public int? Emplacement { get; set; }

    public string? EmplacementDesignation { get; set; }

    public decimal? LongueurBarre { get; set; }

    public decimal? LongueurChuteMini { get; set; }

    public int Categorie1 { get; set; }

    public int Categorie2 { get; set; }

    public int Categorie3 { get; set; }

    public string Categorie4 { get; set; } = null!;

    public int OrdreTri { get; set; }
}
