using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class VieStockQuantiteEmplacement
{
    public int Id { get; set; }

    public int IdArticleInterne { get; set; }

    public string? Reference { get; set; }

    public string? Couleur { get; set; }

    public double Quantite { get; set; }

    public int ZonePriorite { get; set; }

    public string? ZoneDesignation { get; set; }

    public int AdressePriorite { get; set; }

    public string? AdresseDesignation { get; set; }

    public string? ConteneurDesignation { get; set; }

    public string? TypeConteneur { get; set; }

    public string? TypeContenant { get; set; }

    public int Categorie1 { get; set; }

    public int Categorie2 { get; set; }

    public int Categorie3 { get; set; }

    public string Categorie4 { get; set; } = null!;
}
