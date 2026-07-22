using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class ArticleInterneConsommation
{
    public int Id { get; set; }

    public int IdArticleInterne { get; set; }

    public int Quantite { get; set; }

    public DateTime DateConso { get; set; }

    public int IdUtilisateur { get; set; }

    public string Motif { get; set; } = null!;

    public int IdMotif { get; set; }

    public string? NumConsoManuelle { get; set; }

    public string? NumCommande { get; set; }

    public string? Conteneur { get; set; }

    public string? Adresse { get; set; }
}
