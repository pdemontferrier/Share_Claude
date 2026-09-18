using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class ChutesArchive
{
    public int Id { get; set; }

    public int? IdArticleInterne { get; set; }

    public int? IdType { get; set; }

    public int? Longueur { get; set; }

    public int? Largeur { get; set; }

    public sbyte? ScanEntree { get; set; }

    public sbyte? ScanSortie { get; set; }

    public int? IdOperateurEntree { get; set; }

    public int? IdOperateurSortie { get; set; }

    public DateTime? DateEntree { get; set; }

    public DateTime? DateSortie { get; set; }

    public string? Reserve { get; set; }

    public string? CodeBarre { get; set; }

    public double? Prix { get; set; }
}
