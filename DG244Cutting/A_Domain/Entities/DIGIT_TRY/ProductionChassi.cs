using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Châssis de production rattaché à une commande, issu d’agrégations (group by) depuis Tempor_Import.
/// </summary>
public partial class ProductionChassi
{
    /// <summary>
    /// Identifiant technique du châssis (PK).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FK CustomerOrder. Source : Tempor_Import.Aunummer.
    /// </summary>
    public int IdCustomerOrder { get; set; }

    /// <summary>
    /// Index de sous-série. Source : Tempor_Import.TeilserienIndex.
    /// </summary>
    public int PartialSeriesIndex { get; set; }

    /// <summary>
    /// Position du châssis dans la commande. Source : Tempor_Import.Pos.
    /// </summary>
    public short OrderPosition { get; set; }

    /// <summary>
    /// Identifiant Look3E pour le Chassis. Source : Tempor_Import.Feld_10_513.
    /// </summary>
    public string? LookChassisId { get; set; }

    /// <summary>
    /// Identifiant code-barres chassis. Source : Tempor_Import.Feld_10_059.
    /// </summary>
    public string BarcodeId { get; set; } = null!;

    /// <summary>
    /// Quantité. Source : Tempor_Import.Wert_11.
    /// </summary>
    public short Quantity { get; set; }

    /// <summary>
    /// Position dans la série. Source : Tempor_Import.Feld_10_030.
    /// </summary>
    public short SeriesPosition { get; set; }

    /// <summary>
    /// Position client. Source : Tempor_Import.Feld_10_041.
    /// </summary>
    public string? CustomerPosition { get; set; }

    /// <summary>
    /// Famille produit. Source : Tempor_Import.Feld_10_048.
    /// </summary>
    public string ProductFamily { get; set; } = null!;

    /// <summary>
    /// Hauteur élément. Source : Tempor_Import.Feld_10_032.
    /// </summary>
    public short? ElementHeight { get; set; }

    /// <summary>
    /// Largeur élément. Source : Tempor_Import.Feld_10_031.
    /// </summary>
    public short? ElementWidth { get; set; }

    /// <summary>
    /// Largeur cadre incluant RV. Source : Tempor_Import.Feld_10_077.
    /// </summary>
    public short? FrameWidthIncludingRV { get; set; }

    /// <summary>
    /// Hauteur cadre incluant RV. Source : Tempor_Import.Feld_10_078.
    /// </summary>
    public short? FrameHeightIncludingRV { get; set; }

    /// <summary>
    /// Largeur extérieure incluant RV. Source : Tempor_Import.Feld_10_079.
    /// </summary>
    public short? OuterWidthIncludingRV { get; set; }

    /// <summary>
    /// Hauteur extérieure incluant RV. Source : Tempor_Import.Feld_10_080.
    /// </summary>
    public short? OuterHeightIncludingRV { get; set; }

    /// <summary>
    /// Largeur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_245.
    /// </summary>
    public decimal? WidthWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Hauteur avec correction et coupe à la pointe. Source : Tempor_Import.Feld_10_246.
    /// </summary>
    public short? HeightWithCorrectionAndMiterTip { get; set; }

    /// <summary>
    /// Couleur intérieur/extérieur. Source : Tempor_Import.Feld_10_011.
    /// </summary>
    public string? ColorNameIntExt { get; set; }

    /// <summary>
    /// Texte de fenêtre. Source : Tempor_Import.Feld_10_113.
    /// </summary>
    public string? WindowText { get; set; }

    /// <summary>
    /// Dimensions vantaux G/D. Source : Tempor_Import.Feld_10_012.
    /// </summary>
    public string? SashDimensionsLeftRight { get; set; }

    /// <summary>
    /// Code système fenêtre. Source : Tempor_Import.Feld_10_019.
    /// </summary>
    public string? WindowSystemCode { get; set; }

    /// <summary>
    /// Zone de capacité. Source : Tempor_Import.Feld_10_074.
    /// </summary>
    public string? CapacityZone { get; set; }

    /// <summary>
    /// Type de coulissant. Source : Tempor_Import.Feld_10_233.
    /// </summary>
    public string? SlidingType { get; set; }

    /// <summary>
    /// Type de coulissant détaillé. Source : Tempor_Import.Feld_10_234.
    /// </summary>
    public string? SlidingTypeDetailed { get; set; }

    /// <summary>
    /// Abréviation type d’ouverture. Source : Tempor_Import.Feld_10_034.
    /// </summary>
    public string? OpeningTypeAbbreviation { get; set; }

    /// <summary>
    /// Texte type d’ouverture. Source : Tempor_Import.Feld_10_013.
    /// </summary>
    public string? OpeningTypeText { get; set; }

    /// <summary>
    /// Ventaux prédéfinis. Source : Tempor_Import.Feld_10_282.
    /// </summary>
    public string? SashPreset { get; set; }

    /// <summary>
    /// Date de création (système).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour (système).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Suppression logique (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual CustomerOrder IdCustomerOrderNavigation { get; set; } = null!;

    public virtual ICollection<ProductionFrameSash> ProductionFrameSashes { get; set; } = new List<ProductionFrameSash>();
}
