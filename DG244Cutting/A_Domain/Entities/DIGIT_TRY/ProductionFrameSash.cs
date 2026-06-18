using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Cadres et ouvrants de production rattachés à un châssis, issus de Tempor_Import (données Leitxx).
/// </summary>
public partial class ProductionFrameSash
{
    /// <summary>
    /// Identifiant technique du cadre/ouvrant (PK).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant technique du châssis parent (FK ProductionChassis).
    /// </summary>
    public int IdProductionChassis { get; set; }

    /// <summary>
    /// Numéro du composant dans le chassis. Source : Tempor_Import.Wert_14.
    /// </summary>
    public short ComponentNumber { get; set; }

    /// <summary>
    /// Largeur cadre/ouvrant. Source : Tempor_Import.Feld_10_038.
    /// </summary>
    public decimal? FrameSashWidth { get; set; }

    /// <summary>
    /// Hauteur cadre/ouvrant. Source : Tempor_Import.Feld_10_039.
    /// </summary>
    public decimal? FrameSashHeight { get; set; }

    /// <summary>
    /// Type d’ouverture (texte). Source : Tempor_Import.Feld_10_043.
    /// </summary>
    public string? OpeningTypeText { get; set; }

    /// <summary>
    /// Indicateur de sens d’ouverture. Source : Tempor_Import.Feld_10_045.
    /// </summary>
    public string? OpeningDirectionIndicator { get; set; }

    /// <summary>
    /// Code type d’ouverture spécifique. Source : Tempor_Import.Feld_10_125.
    /// </summary>
    public string? SpecialOpeningTypeCode { get; set; }

    /// <summary>
    /// Partie de cadre adjacente à l’ouvrant. Source : Tempor_Import.Feld_10_224.
    /// </summary>
    public string? AdjacentFramePartToSash { get; set; }

    /// <summary>
    /// Largeur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_229.
    /// </summary>
    public decimal? FrameSashWidthTenths { get; set; }

    /// <summary>
    /// Hauteur cadre/ouvrant en dixièmes. Source : Tempor_Import.Feld_10_230.
    /// </summary>
    public decimal? FrameSashHeightTenths { get; set; }

    /// <summary>
    /// Contre-profil cadre/seuil. Source : Tempor_Import.Feld_10_221.
    /// </summary>
    public string? FrameThresholdCounterProfile { get; set; }

    /// <summary>
    /// Code de renfort. Source : Tempor_Import.Feld_10_015.
    /// </summary>
    public string? ReinforcementCode { get; set; }

    /// <summary>
    /// Longueur du renfort. Source : Tempor_Import.Feld_10_016.
    /// </summary>
    public short? ReinforcementLength { get; set; }

    /// <summary>
    /// Longueur de renfort 1 sans trame. Source : Tempor_Import.Feld_10_583.
    /// </summary>
    public short? ReinforcementLength1NoGrid { get; set; }

    /// <summary>
    /// Longueur de renfort 2 sans trame. Source : Tempor_Import.Feld_10_584.
    /// </summary>
    public short? ReinforcementLength2NoGrid { get; set; }

    /// <summary>
    /// Couleur d’affichage intérieure. Source : Tempor_Import.Feld_10_585.
    /// </summary>
    public string? DisplayColorInside { get; set; }

    /// <summary>
    /// Couleur d’affichage extérieure. Source : Tempor_Import.Feld_10_586.
    /// </summary>
    public string? DisplayColorOutside { get; set; }

    /// <summary>
    /// Joint. Source : Tempor_Import.Feld_10_061.
    /// </summary>
    public string? Seal { get; set; }

    /// <summary>
    /// Couleur du joint. Source : Tempor_Import.Feld_10_062.
    /// </summary>
    public string? SealColor { get; set; }

    /// <summary>
    /// Système de joint. Source : Tempor_Import.Feld_10_067.
    /// </summary>
    public string? SealSystem { get; set; }

    /// <summary>
    /// Joint ouvrant/cadre intérieur. Source : Tempor_Import.Feld_10_150.
    /// </summary>
    public string? InnerSealSashFrame { get; set; }

    /// <summary>
    /// Texte de variante de joint. Source : Tempor_Import.Feld_10_564.
    /// </summary>
    public string? SealVariantText { get; set; }

    /// <summary>
    /// Code de variante de joint. Source : Tempor_Import.Feld_10_563.
    /// </summary>
    public string? SealVariantCode { get; set; }

    /// <summary>
    /// Joint intérieur issu du système de parcloses. Source : Tempor_Import.Feld_10_294.
    /// </summary>
    public string? BeadSystemInnerSeal { get; set; }

    /// <summary>
    /// Couleur du joint issue des données de position. Source : Tempor_Import.Feld_10_489.
    /// </summary>
    public string? PositionDataSealColor { get; set; }

    /// <summary>
    /// Texte pour le joint de vitrage. Source : Tempor_Import.Feld_10_017.
    /// </summary>
    public string? GlazingSealText { get; set; }

    /// <summary>
    /// Affectation du vitrage. Source : Tempor_Import.Feld_10_148.
    /// </summary>
    public string? GlazingAssignment { get; set; }

    /// <summary>
    /// Code vitrage. Source : Tempor_Import.Feld_10_560.
    /// </summary>
    public string? GlazingCode { get; set; }

    /// <summary>
    /// Dimensions du vitrage. Source : Tempor_Import.Feld_10_134.
    /// </summary>
    public string? GlazingDimensions { get; set; }

    /// <summary>
    /// Texte vitrage. Source : Tempor_Import.Feld_10_018.
    /// </summary>
    public string? GlazingText { get; set; }

    /// <summary>
    /// Parcloses par ouvrant/cadre. Source : Tempor_Import.Feld_10_137.
    /// </summary>
    public string? GlazingBeadsPerSashFrame { get; set; }

    /// <summary>
    /// Hauteur des parcloses. Source : Tempor_Import.Feld_10_056.
    /// </summary>
    public decimal? BeadsHeight { get; set; }

    /// <summary>
    /// Largeur des parcloses. Source : Tempor_Import.Feld_10_055.
    /// </summary>
    public decimal? BeadsWidth { get; set; }

    /// <summary>
    /// Texte pour le système de ferrures. Source : Tempor_Import.Feld_10_014.
    /// </summary>
    public string? HardwareSystemText { get; set; }

    /// <summary>
    /// Code du système de ferrures. Source : Tempor_Import.Feld_10_023.
    /// </summary>
    public string? HardwareSystemCode { get; set; }

    /// <summary>
    /// Position de la poignée. Source : Tempor_Import.Feld_10_161.
    /// </summary>
    public string? HandlePosition { get; set; }

    /// <summary>
    /// Indicateur de ferrure d’ouvrant sinon global. Source : Tempor_Import.Feld_10_187.
    /// </summary>
    public string? SashHardwareIndicator { get; set; }

    /// <summary>
    /// Code de mécanisme (boîtier/entraînement). Source : Tempor_Import.Feld_10_215.
    /// </summary>
    public string? MechanismCode { get; set; }

    /// <summary>
    /// Largeur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_231.
    /// </summary>
    public decimal? HardwareRabbetWidthTenths { get; set; }

    /// <summary>
    /// Hauteur feuillure de ferrure en dixièmes. Source : Tempor_Import.Feld_10_232.
    /// </summary>
    public decimal? HardwareRabbetHeightTenths { get; set; }

    /// <summary>
    /// Ferrage type crémone 1. Source : Tempor_Import.Feld_10_262.
    /// </summary>
    public string? CremoneType1 { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ProductionChassi IdProductionChassisNavigation { get; set; } = null!;

    public virtual ICollection<ProductionCutPiece> ProductionCutPieces { get; set; } = new List<ProductionCutPiece>();
}
