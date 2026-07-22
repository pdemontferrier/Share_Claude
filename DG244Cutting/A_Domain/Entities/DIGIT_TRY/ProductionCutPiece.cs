using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Pièces à découper issues de Tempor_Import, rattachées à un composant de châssis (cadre ou ouvrant).
/// </summary>
public partial class ProductionCutPiece
{
    /// <summary>
    /// Identifiant technique de la pièce à découper (PK).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du composant châssis (cadre ou ouvrant) parent.
    /// </summary>
    public int IdProductionFrameSash { get; set; }

    /// <summary>
    /// Identifiant de la position spatiale dans le châssis. Source : Tempor_Import.Feld_6.
    /// </summary>
    public short? IdSpatialPosition { get; set; }

    /// <summary>
    /// Identifiant de l’article interne associé à la pièce à découper.
    /// </summary>
    public int? IdArticleInternal { get; set; }

    /// <summary>
    /// Identifiant de la barre de production associée à la découpe.
    /// </summary>
    public int? IdProductionBar { get; set; }

    /// <summary>
    /// Identifiant Look3E pour la Commande Client. Source. Source : Tempor_Import.Feld_10_205.
    /// </summary>
    public string LookCustomerOrderId { get; set; } = null!;

    /// <summary>
    /// Identifiant Look3E pour le Chassis. Source : Tempor_Import.Feld_10_513.
    /// </summary>
    public string? LookChassisId { get; set; }

    /// <summary>
    /// Identifiant Look3E pour la pièce à découper. Source : Tempor_Import.Feld_23.
    /// </summary>
    public string? LookCutPieceId { get; set; }

    /// <summary>
    /// Code-barre de la pièce. Source : Tempor_Import.Feld_10_165.
    /// </summary>
    public string CutBarcode { get; set; } = null!;

    /// <summary>
    /// Indice côté (0 bas,1 gauche,2 haut,3 droite). Source : Tempor_Import.Feld_10_020.
    /// </summary>
    public short? SideIndex { get; set; }

    /// <summary>
    /// Numéro de pièce dans le composant. Source : Tempor_Import.Feld_10_114.
    /// </summary>
    public short? ComponentPieceNumber { get; set; }

    /// <summary>
    /// Numéro de pièce dans le châssis. Source : Tempor_Import.Ref.
    /// </summary>
    public short? ChassisPieceNumber { get; set; }

    /// <summary>
    /// Numéro de ligne de commande client. Source : Tempor_Import.Feld_10_227.
    /// </summary>
    public short? CustomerOrderLineNumber { get; set; }

    /// <summary>
    /// Numéro de pièce dans la position. Source : Tempor_Import.Feld_10_006.
    /// </summary>
    public short? PositionPieceNumber { get; set; }

    /// <summary>
    /// Numéro séquentiel de pièce. Source : Tempor_Import.Feld_10_036.
    /// </summary>
    public short? SequentialPieceNumber { get; set; }

    /// <summary>
    /// Numéro séquentiel dans la série partielle. Source : Tempor_Import.Feld_10_277.
    /// </summary>
    public short? PartialSeriesSequentialPieceNumber { get; set; }

    /// <summary>
    /// Numéro de ligne commande client (variante). Source : Tempor_Import.Feld_10_239.
    /// </summary>
    public short? CustomerOrderLineNumber2 { get; set; }

    /// <summary>
    /// Code famille de barre. Source : Tempor_Import.Wert_2.
    /// </summary>
    public int? BarFamilyCode { get; set; }

    /// <summary>
    /// Code produit à imprimer sur la barre. Source : Tempor_Import.CodeNat.
    /// </summary>
    public string? BarProductCodeToPrint { get; set; }

    /// <summary>
    /// Référence de la barre. Source : Tempor_Import.Feld_9.
    /// </summary>
    public string? BarReference { get; set; }

    /// <summary>
    /// Désignation famille de barre. Source : Tempor_Import.Feld_40.
    /// </summary>
    public string? BarProductFamilyName { get; set; }

    /// <summary>
    /// Code couleur barre intérieur/extérieur. Source : Tempor_Import.Feld_8.
    /// </summary>
    public string? BarColorCodeInOut { get; set; }

    /// <summary>
    /// Hauteur de la barre. Source : Tempor_Import.Wert_41.
    /// </summary>
    public decimal? BarHeight { get; set; }

    /// <summary>
    /// Largeur de la barre. Source : Tempor_Import.Wert_42.
    /// </summary>
    public decimal? BarWidth { get; set; }

    /// <summary>
    /// Longueur brute de la barre. Source : Tempor_Import.Wert_21.
    /// </summary>
    public int? BarLength { get; set; }

    /// <summary>
    /// Longueur maximale de chute autorisée. Source : Tempor_Import.Wert_39.
    /// </summary>
    public int? ScrapMaxLength { get; set; }

    /// <summary>
    /// Longueur de coupe scie. Source : Tempor_Import.Wert_34.
    /// </summary>
    public int? SawCutLength { get; set; }

    /// <summary>
    /// Longueur de coupe finition. Source : Tempor_Import.Wert_33.
    /// </summary>
    public int? FinishingCutLength { get; set; }

    /// <summary>
    /// Pas de filière. Source : Tempor_Import.Wert_37.
    /// </summary>
    public int? DiePitch { get; set; }

    /// <summary>
    /// Longueur minimale pour optimisation. Source : Tempor_Import.Wert_38.
    /// </summary>
    public int? OptimizationMinLength { get; set; }

    /// <summary>
    /// Longueur restante avant seuil. Source : Tempor_Import.Wert_36.
    /// </summary>
    public int? RemainingUntilLength { get; set; }

    /// <summary>
    /// Code machine de découpe. Source : Tempor_Import.Feld_10_021.
    /// </summary>
    public string? MachineCode { get; set; }

    /// <summary>
    /// Compteur article Leitxx. Source : Tempor_Import.Feld_10_302.
    /// </summary>
    public int? ArticleCounter { get; set; }

    /// <summary>
    /// Numéro de profil. Source : Tempor_Import.Feld_10_066.
    /// </summary>
    public string? ProfileNumber { get; set; }

    /// <summary>
    /// Numéro de profil machine. Source : Tempor_Import.Feld_10_330.
    /// </summary>
    public string? ProfileNumberForMachine { get; set; }

    /// <summary>
    /// Code profil à imprimer. Source : Tempor_Import.Feld_10_027.
    /// </summary>
    public string? ProfileCodeToPrint { get; set; }

    /// <summary>
    /// Nom du profil. Source : Tempor_Import.Feld_10_100.
    /// </summary>
    public string? ProfileName { get; set; }

    /// <summary>
    /// Longueur du profil. Source : Tempor_Import.Feld_10_010.
    /// </summary>
    public decimal? ProfileLength { get; set; }

    /// <summary>
    /// Longueur du profil avec FOD. Source : Tempor_Import.Feld_10_565.
    /// </summary>
    public decimal? ProfileLengthIncludingFOD { get; set; }

    /// <summary>
    /// Longueur jour avec angle et correction. Source : Tempor_Import.Feld_10_267.
    /// </summary>
    public int? DaylightLengthWithAngleAndCorrection { get; set; }

    /// <summary>
    /// Largeur du profil. Source : Tempor_Import.Feld_10_051.
    /// </summary>
    public decimal? ProfileWidth { get; set; }

    /// <summary>
    /// Hauteur du profil. Source : Tempor_Import.Feld_10_075.
    /// </summary>
    public decimal? ProfileHeight { get; set; }

    /// <summary>
    /// Couleur intérieure du profil. Source : Tempor_Import.Feld_10_088.
    /// </summary>
    public string? ProfileColorInside { get; set; }

    /// <summary>
    /// Couleur extérieure du profil. Source : Tempor_Import.Feld_10_089.
    /// </summary>
    public string? ProfileColorOutside { get; set; }

    /// <summary>
    /// Code couleur profil intérieur/extérieur. Source : Tempor_Import.Feld_10_026.
    /// </summary>
    public string? ProfileColorCodeInOut { get; set; }

    /// <summary>
    /// Dimension de coupe. Source : Tempor_Import.Wert_6
    /// </summary>
    public decimal? CutDimension { get; set; }

    /// <summary>
    /// Inclinaison de coupe gauche. Source : Tempor_Import.Feld_10_104.
    /// </summary>
    public short? CutInclinationLeft { get; set; }

    /// <summary>
    /// Inclinaison de coupe droite. Source : Tempor_Import.Feld_10_105.
    /// </summary>
    public short? CutInclinationRight { get; set; }

    /// <summary>
    /// Pivot de coupe gauche. Source : Tempor_Import.Feld_10_111.
    /// </summary>
    public short? CutPivotLeft { get; set; }

    /// <summary>
    /// Pivot de coupe droite. Source : Tempor_Import.Feld_10_112.
    /// </summary>
    public short? CutPivotRight { get; set; }

    /// <summary>
    /// Numéro de chariot. Source : Tempor_Import.Wagen.
    /// </summary>
    public short? TrolleyNumber { get; set; }

    /// <summary>
    /// Niveau du chariot. Source : Tempor_Import.Etage.
    /// </summary>
    public short? TrolleyLevel { get; set; }

    /// <summary>
    /// Emplacement du chariot. Source : Tempor_Import.Fach.
    /// </summary>
    public short? TrolleySlot { get; set; }

    /// <summary>
    /// Code de montage. Source : Tempor_Import.Feld_10_009.
    /// </summary>
    public string? AssemblyCode { get; set; }

    /// <summary>
    /// Compteur de fenêtre. Source : Tempor_Import.Feld_10_052.
    /// </summary>
    public short? WindowCounter { get; set; }

    /// <summary>
    /// Compteur d’élément. Source : Tempor_Import.Feld_10_057.
    /// </summary>
    public short? ElementCounter { get; set; }

    /// <summary>
    /// Numéro de champ du cadre. Source : Tempor_Import.Feld_10_058.
    /// </summary>
    public short? FrameFieldNumber { get; set; }

    /// <summary>
    /// Code profil de liaison. Source : Tempor_Import.Feld_10_022.
    /// </summary>
    public string? ConnectionProfileCode { get; set; }

    /// <summary>
    /// Code d’évacuation pour calcul. Source : Tempor_Import.Feld_10_181.
    /// </summary>
    public string? DrainageCodeUsedForCalculation { get; set; }

    /// <summary>
    /// Quantité totale pour la position. Source : Tempor_Import.Feld_10_133.
    /// </summary>
    public short? TotalQuantityForPosition { get; set; }

    /// <summary>
    /// Nombre total d’éléments. Source : Tempor_Import.Feld_10_144.
    /// </summary>
    public short? TotalElementsCount { get; set; }

    /// <summary>
    /// Référence article associée à droite. Source : Tempor_Import.Feld_10_346.
    /// </summary>
    public string? AssociatedArticleReferenceRight { get; set; }

    /// <summary>
    /// Référence article associée à gauche. Source : Tempor_Import.Feld_10_347.
    /// </summary>
    public string? AssociatedArticleReferenceLeft { get; set; }

    /// <summary>
    /// Position de la decoupe au sein de la barre (ordre de coupe).
    /// </summary>
    public int? CutPositionInBar { get; set; }

    /// <summary>
    /// Indicateur temporaire d&apos;optimisation de la decoupe (usage transitoire, en parallele de IsOptimized).
    /// </summary>
    public bool IsOptimizedTemp { get; set; }

    /// <summary>
    /// Indique si la decoupe a ete selectionnee par le processus d&apos;optimisation.
    /// </summary>
    public bool IsOptimized { get; set; }

    /// <summary>
    /// Indique si la barre necessaire a la decoupe a ete approvisionnee.
    /// </summary>
    public bool IsBarSupplied { get; set; }

    /// <summary>
    /// Indique si la barre necessaire à la découpe est en rupture de stock.
    /// </summary>
    public bool IsBarOutOfStock { get; set; }

    /// <summary>
    /// Indique si la decoupe a ete realisee.
    /// </summary>
    public bool IsCut { get; set; }

    /// <summary>
    /// Date et heure de debut de la decoupe.
    /// </summary>
    public DateTime? CutStartedAt { get; set; }

    /// <summary>
    /// Date et heure de validation de la decoupe.
    /// </summary>
    public DateTime? CutFinishedAt { get; set; }

    /// <summary>
    /// Date de creation de la ligne dans le systeme local.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de derniere modification dans le systeme local.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal? IdArticleInternalNavigation { get; set; }

    public virtual ProductionFrameSash IdProductionFrameSashNavigation { get; set; } = null!;

    public virtual SpatialPosition? IdSpatialPositionNavigation { get; set; }
}
