using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Usinages à plat (1 ligne par usinage) pour la génération des blocs machine ECA/ECW.
/// </summary>
public partial class ProductionMachiningWork
{
    public int Id { get; set; }

    /// <summary>
    /// Clé étrangère vers ProductionCutPiece.Id ; renseignée après coup par jointure sur LookCutPieceId.
    /// </summary>
    public int? IdProductionCutPiece { get; set; }

    /// <summary>
    /// Clé étrangère vers CuttingMachine.Id ; renseignée par affectation profil vers machine.
    /// </summary>
    public int? IdCuttingMachine { get; set; }

    /// <summary>
    /// Clé de rapprochement métier vers ProductionCutPiece (issue de Feld_23). Source : Tempor_Import_Feld_12.Feld_23.
    /// </summary>
    public string LookCutPieceId { get; set; } = null!;

    /// <summary>
    /// Numéro d’ordre 1..n attribué après tri (SideIndex, PositionX) aux seuls usinages retenus. Source : Calcul. Destination : WNo.
    /// </summary>
    public short? MachiningSequence { get; set; }

    /// <summary>
    /// Code macro CAD (CAD suivi du numéro de macro). Source : Tempor_Import_Feld_12.Feld_12_006.
    /// </summary>
    public string? MacroCadCode { get; set; }

    /// <summary>
    /// Numéro de macro machine (94001 à 94043). Source : Tempor_Import_Feld_12.Feld_12_003. Destination : WMacro.
    /// </summary>
    public int? MacroNumber { get; set; }

    /// <summary>
    /// Libellé de l’usinage (vide ou (kein)). Source : Tempor_Import_Feld_12.Feld_12_007.
    /// </summary>
    public string? MachiningLabel { get; set; }

    /// <summary>
    /// Commentaire lié à l’usinage. Source : Tempor_Import_Feld_12.Feld_12_008_40. Destination : WComment.
    /// </summary>
    public string? MachiningComment { get; set; }

    /// <summary>
    /// Type d’usinage (Z, E, H, B...). Source : Tempor_Import_Feld_12.Feld_12_001. Destination : WType.
    /// </summary>
    public string? MachiningType { get; set; }

    /// <summary>
    /// Flag d’orientation (-1 = pièce retournée). Source : Tempor_Import_Feld_12.Feld_12_008_01.
    /// </summary>
    public short? OrientationFlag { get; set; }

    /// <summary>
    /// Côté recalculé ; ramené à 0 sauf pièce biface (Z côté 1) où le côté est conservé. Source : Tempor_Import_Feld_12.Feld_12_002. Destination : WSide.
    /// </summary>
    public short? SideIndex { get; set; }

    /// <summary>
    /// Position longitudinale recalculée selon le flag d’orientation, arrondie au 1/10 mm. Source : Tempor_Import_Feld_12.Feld_12_004. Destination : WX1.
    /// </summary>
    public decimal? PositionX { get; set; }

    /// <summary>
    /// Longueur de découpe de la pièce ; sert au recalcul et au test de limites. Source : Tempor_Import.Wert_6.
    /// </summary>
    public decimal? CutLength { get; set; }

    /// <summary>
    /// Vrai si 0 est inférieur ou égal à PositionX inférieur ou égal à CutLength après recalcul. Source : Calcul.
    /// </summary>
    public bool IsWithinLimits { get; set; }

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

    public virtual CuttingMachine? IdCuttingMachineNavigation { get; set; }

    public virtual ProductionCutPiece? IdProductionCutPieceNavigation { get; set; }
}
