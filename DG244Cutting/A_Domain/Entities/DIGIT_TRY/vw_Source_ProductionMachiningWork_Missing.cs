using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_Source_ProductionMachiningWork_Missing
{
    /// <summary>
    /// Clé étrangère vers ProductionCutPiece.Id, résolue via LookCutPieceId. Source : ProductionCutPiece.Id (via Feld_23).
    /// </summary>
    public int IdProductionCutPiece { get; set; }

    /// <summary>
    /// Machine de découpe, résolue via ProductionCutPiece puis ArticleInternal puis ArticleReference. Source : ArticleReference.IdCuttingMachine.
    /// </summary>
    public int IdCuttingMachine { get; set; }

    /// <summary>
    /// Clé de rapprochement métier vers la pièce de découpe. Source : Tempor_Import_Feld_12.Feld_23.
    /// </summary>
    public string LookCutPieceId { get; set; } = null!;

    /// <summary>
    /// Numéro d’ordre des seuls usinages retenus, par pièce, trié (SideIndex, PositionX) ; NULL si écarté. Source : Calculé.
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
    /// Flag d’orientation (-1 = pièce retournée) ; intrant du recalcul de position. Source : Tempor_Import_Feld_12.Feld_12_008_01.
    /// </summary>
    public short? OrientationFlag { get; set; }

    /// <summary>
    /// Côté : 0 si pièce retournée ; sinon côté source si pièce biface ; sinon 0. Source : Calculé. Destination : WSide.
    /// </summary>
    public short? SideIndex { get; set; }

    /// <summary>
    /// Position recalculée (CutLength - X1 si pièce retournée, sinon X1), arrondie au 1/10 mm. Source : Calculé. Destination : WX1.
    /// </summary>
    public decimal? PositionX { get; set; }

    /// <summary>
    /// Longueur de découpe de la pièce ; intrant des calculs et du test de limites. Source : Tempor_Import.Wert_6.
    /// </summary>
    public decimal? CutLength { get; set; }

    /// <summary>
    /// Vrai si 0 est inférieur ou égal à PositionX inférieur ou égal à CutLength (position recalculée). Source : Calculé.
    /// </summary>
    public bool? IsWithinLimits { get; set; }
}
