using System;
using System.Collections.Generic;

namespace BatchStockRelease.A_Domain.Entities.GestStock;

public partial class ArticleInterne
{
    public int Id { get; set; }

    public string? Reference { get; set; }

    public string? Designation { get; set; }

    public string? Couleur { get; set; }

    public int IdDernierFournisseur { get; set; }

    public int IdFuturFournisseur { get; set; }

    public double? DernierPrix { get; set; }

    public bool? Existant { get; set; }

    public double ConsoHebdo { get; set; }

    public int IdPickingListes { get; set; }

    public bool? Valorised { get; set; }

    public DateTime DateDpa { get; set; }

    public DateOnly DateConsoHebdoAuto { get; set; }

    public int UniteReservation { get; set; }

    public double RapportResaStock { get; set; }

    public int UniteStock { get; set; }

    public sbyte GereEnStock { get; set; }

    public bool? Alertes { get; set; }

    public int QteMiniPickingAuto { get; set; }

    public int? GestionChute { get; set; }

    public string? RefElusoft { get; set; }

    public double? InventaireCoefEmplacement { get; set; }

    public sbyte? InventaireEnCours { get; set; }

    public int? UniteStock2 { get; set; }

    public double? RapportStock1Stock2 { get; set; }

    public DateTime? DateCreation { get; set; }

    public double? Depreciation { get; set; }

    public sbyte? ArticleUbc { get; set; }

    public sbyte? InventaireForce { get; set; }

    public sbyte? InventaireExempt { get; set; }

    public double? Poids { get; set; }

    public int? UnitePoids { get; set; }

    public double RapportPoids { get; set; }

    public int IdComposeType { get; set; }

    public double? PrixMainOeuvre { get; set; }

    public int? IdArticleInterneVitrageType { get; set; }

    /// <summary>
    /// Désignation agrégée
    /// </summary>
    public string? DesignationL1 { get; set; }

    /// <summary>
    /// Catégorie de fournisseur
    /// </summary>
    public string? CategorieL1 { get; set; }

    /// <summary>
    /// Catégorie si Actif, Attente, Suspendu
    /// </summary>
    public string? CategorieL2 { get; set; }

    /// <summary>
    /// CHECK (coef_stoc_secu&gt;= 1)
    /// </summary>
    public float CoefStocSecu { get; set; }

    /// <summary>
    /// CHECK (stock_securite&gt;= 1)
    /// </summary>
    public int StockSecurite { get; set; }

    /// <summary>
    /// CHECK (stock_min&gt;= 0)
    /// </summary>
    public int StockMin { get; set; }

    public bool? CalculStockMin { get; set; }

    public int IdCouleurRal { get; set; }

    public string IdCouleurFinition { get; set; } = null!;

    public string IdCouleurRalFinition { get; set; } = null!;

    public bool ControlCheck { get; set; }

    public double? RapportStockTarif { get; set; }

    public int? UniteTarif { get; set; }

    public double? CoefTauxChute { get; set; }

    public int? IdEmplacementChutes { get; set; }
}
