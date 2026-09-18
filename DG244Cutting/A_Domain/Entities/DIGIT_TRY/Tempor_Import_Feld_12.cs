using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Éclatement de Tempor_Import.Feld_12 à la granularité 1 usinage par ligne.
/// </summary>
public partial class Tempor_Import_Feld_12
{
    public int Id { get; set; }

    /// <summary>
    /// Clé étrangère vers Tempor_Import.Id : pièce parente dont l’usinage est issu.
    /// </summary>
    public int IdTemporImport { get; set; }

    /// <summary>
    /// Clé de rapprochement métier (LookCutPieceId) reprise de la pièce parente.
    /// </summary>
    public string Feld_23 { get; set; } = null!;

    /// <summary>
    /// Numéro d’ordre de l’usinage dans la pièce (1 à N).
    /// </summary>
    public short WorkIndex { get; set; }

    /// <summary>
    /// Type d’usinage (Z=extrémité, E=perçage, H=trou répétitif, B=ferrure, K, G).
    /// </summary>
    public string? Feld_12_001 { get; set; }

    /// <summary>
    /// Côté (Side) de l’usinage : 0 ou 1.
    /// </summary>
    public short? Feld_12_002 { get; set; }

    /// <summary>
    /// Numéro de macro machine (94001 à 94043).
    /// </summary>
    public int? Feld_12_003 { get; set; }

    /// <summary>
    /// Position longitudinale X1 de l’usinage (mm).
    /// </summary>
    public decimal? Feld_12_004 { get; set; }

    /// <summary>
    /// Longueur ou position X2 ; renseignée pour le type Z uniquement.
    /// </summary>
    public decimal? Feld_12_005 { get; set; }

    /// <summary>
    /// Code macro CAD (préfixe CAD suivi du numéro de macro).
    /// </summary>
    public string? Feld_12_006 { get; set; }

    /// <summary>
    /// Libellé de l’usinage ; vide ou (kein).
    /// </summary>
    public string? Feld_12_007 { get; set; }

    /// <summary>
    /// Flag d’orientation ; -1 indique une pièce retournée.
    /// </summary>
    public short? Feld_12_008_01 { get; set; }

    /// <summary>
    /// Cote A du bloc de paramètres de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_02 { get; set; }

    /// <summary>
    /// Cote B du bloc de paramètres de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_03 { get; set; }

    /// <summary>
    /// Cote C du bloc de paramètres de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_04 { get; set; }

    /// <summary>
    /// Nombre de positions de l’usinage.
    /// </summary>
    public short? Feld_12_008_05 { get; set; }

    /// <summary>
    /// Référence de ferrure ; 0 ou code article AW.
    /// </summary>
    public string? Feld_12_008_06 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_07 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_08 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_09 { get; set; }

    /// <summary>
    /// Cote de hauteur de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_10 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_11 { get; set; }

    /// <summary>
    /// Indicateur du bloc de paramètres ; 0 ou 1.
    /// </summary>
    public short? Feld_12_008_12 { get; set; }

    /// <summary>
    /// Diamètre de l’usinage.
    /// </summary>
    public short? Feld_12_008_13 { get; set; }

    /// <summary>
    /// Entraxe de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_14 { get; set; }

    /// <summary>
    /// Paramètre 15 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_15 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_16 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_17 { get; set; }

    /// <summary>
    /// Référence d’article associée à l’usinage (code AW).
    /// </summary>
    public string? Feld_12_008_18 { get; set; }

    /// <summary>
    /// Paramètre 19 du bloc de l’usinage ; 0 ou 1.
    /// </summary>
    public short? Feld_12_008_19 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_20 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_21 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_22 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_23 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_24 { get; set; }

    /// <summary>
    /// Face de l’usinage (AUSSEN ou XXX).
    /// </summary>
    public string? Feld_12_008_25 { get; set; }

    /// <summary>
    /// Offset de l’usinage (valeur signée).
    /// </summary>
    public decimal? Feld_12_008_26 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_27 { get; set; }

    /// <summary>
    /// Paramètre 28 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_28 { get; set; }

    /// <summary>
    /// Paramètre 29 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_29 { get; set; }

    /// <summary>
    /// Paramètre 30 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_30 { get; set; }

    /// <summary>
    /// Paramètre 31 du bloc de l’usinage ; -1 ou 0.
    /// </summary>
    public short? Feld_12_008_31 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_32 { get; set; }

    /// <summary>
    /// Cote de pose de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_33 { get; set; }

    /// <summary>
    /// Paramètre 34 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_34 { get; set; }

    /// <summary>
    /// Paramètre 35 du bloc de l’usinage.
    /// </summary>
    public decimal? Feld_12_008_35 { get; set; }

    /// <summary>
    /// Vantail concerné par l’usinage ; 0 ou VPd.
    /// </summary>
    public string? Feld_12_008_36 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_37 { get; set; }

    /// <summary>
    /// Gamme de l’usinage (ex. 225 CAD94).
    /// </summary>
    public string? Feld_12_008_38 { get; set; }

    /// <summary>
    /// Ordre de l’usinage.
    /// </summary>
    public short? Feld_12_008_39 { get; set; }

    /// <summary>
    /// Description du sens d’ouverture.
    /// </summary>
    public string? Feld_12_008_40 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_41 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_42 { get; set; }

    /// <summary>
    /// Code de l’usinage ; vide ou (kein).
    /// </summary>
    public string? Feld_12_008_43 { get; set; }

    /// <summary>
    /// Code numérique de l’usinage.
    /// </summary>
    public short? Feld_12_008_44 { get; set; }

    /// <summary>
    /// Position X réelle de l’usinage (mm).
    /// </summary>
    public decimal? Feld_12_008_45 { get; set; }

    /// <summary>
    /// Numéro de pièce de l’usinage.
    /// </summary>
    public short? Feld_12_008_46 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_47 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_48 { get; set; }

    /// <summary>
    /// Formule de calcul de la position (expression).
    /// </summary>
    public string? Feld_12_008_49 { get; set; }

    /// <summary>
    /// Sous-paramètre du bloc Feld_12_008 ; description à déterminer ultérieurement.
    /// </summary>
    public string? Feld_12_008_50 { get; set; }

    /// <summary>
    /// Indique si l’usinage a été propagé vers la cible aval (ProductionMachiningWork).
    /// </summary>
    public bool IsImported { get; set; }

    /// <summary>
    /// Date de création de la ligne dans le système local.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification dans le système local.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indicateur de suppression logique (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual Tempor_Import IdTemporImportNavigation { get; set; } = null!;
}
