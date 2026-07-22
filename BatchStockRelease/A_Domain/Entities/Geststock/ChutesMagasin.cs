using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class ChutesMagasin
{
    public int Id { get; set; }

    public int? IdArticleInterne { get; set; }

    public int? IdType { get; set; }

    public int? Longueur { get; set; }

    public int? Largeur { get; set; }

    public sbyte? Scan { get; set; }

    public int? IdOperateur { get; set; }

    public DateTime? Enregistrement { get; set; }

    public string? Reserve { get; set; }

    public int? Emplacement { get; set; }

    public string? CodeBarre { get; set; }

    public double? Prix { get; set; }

    public bool AttenteIntegration { get; set; }

    public DateTime DateIntegration { get; set; }
}
