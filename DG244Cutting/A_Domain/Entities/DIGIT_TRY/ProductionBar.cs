using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Barres de production d’une série, avec défauts, états de cycle de vie et reliquat.
/// </summary>
public partial class ProductionBar
{
    /// <summary>
    /// Identifiant technique unique de la barre de production (PK).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Rattachement à la série de production (FK ProductionSeries.Id).
    /// </summary>
    public int IdProductionSeries { get; set; }

    /// <summary>
    /// Article interne de la barre (FK ArticleInternal.Id).
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Chute d’origine si barre réutilisée (FK CuttingScrapStock.Id).
    /// </summary>
    public int? IdSourceScrap { get; set; }

    /// <summary>
    /// Longueur totale de la barre en millimètres.
    /// </summary>
    public int BarLength { get; set; }

    /// <summary>
    /// Début du 1er défaut depuis le début de la barre (mm).
    /// </summary>
    public int? DefectStart1 { get; set; }

    /// <summary>
    /// Fin du 1er défaut (mm).
    /// </summary>
    public int? DefectEnd1 { get; set; }

    /// <summary>
    /// Début du 2e défaut (mm).
    /// </summary>
    public int? DefectStart2 { get; set; }

    /// <summary>
    /// Fin du 2e défaut (mm).
    /// </summary>
    public int? DefectEnd2 { get; set; }

    /// <summary>
    /// Nombre de découpes réalisées sur la barre (legacy decoupe_nombre).
    /// </summary>
    public int CutPieceCount { get; set; }

    /// <summary>
    /// Reste calculé en mm, préliminaire avant validation.
    /// </summary>
    public int? ResidueLength { get; set; }

    /// <summary>
    /// Qualification automatique du reliquat ; par défaut True.
    /// </summary>
    public bool ResidueIsScrap { get; set; }

    /// <summary>
    /// Longueur finale du reliquat validée par l’opérateur (Page22, mm).
    /// </summary>
    public int? ResidueValidatedLength { get; set; }

    /// <summary>
    /// Statut final validé, peut différer par forçage opérateur.
    /// </summary>
    public bool ResidueFinalIsScrap { get; set; }

    /// <summary>
    /// Code-barre de la chute générée. Format BS- suivi de l’Id sur 9 positions.
    /// </summary>
    public string? ScrapBarcode { get; set; }

    /// <summary>
    /// Emplacement littéral de la chute générée (via vue à créer).
    /// </summary>
    public string? ScrapLocation { get; set; }

    /// <summary>
    /// Valeur du reste sur base de ResidueValidatedLength.
    /// </summary>
    public decimal? ResidueValue { get; set; }

    /// <summary>
    /// Date de mise à jour de la valeur du reste.
    /// </summary>
    public DateTime? ResidueValueUpdatedAt { get; set; }

    /// <summary>
    /// Commentaire de motif de refus de la barre.
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// True = barre neuve, False = chute réutilisée.
    /// </summary>
    public bool IsNewBar { get; set; }

    /// <summary>
    /// Placement provisoire, maintenu True sur la barre de référence.
    /// </summary>
    public bool IsOptimizedTemp { get; set; }

    /// <summary>
    /// Barre acceptée physiquement par l’opérateur.
    /// </summary>
    public bool IsValidated { get; set; }

    /// <summary>
    /// Barre définitivement scellée dans le plan.
    /// </summary>
    public bool IsOptimized { get; set; }

    /// <summary>
    /// Barre utilisée pour des découpes ; True à la validation Page22.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Rupture de stock de barre neuve (legacy appro_rupture).
    /// </summary>
    public bool IsOutOfStock { get; set; }

    /// <summary>
    /// Reliquat confirmé par l’opérateur en Page22.
    /// </summary>
    public bool IsResidueValidated { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de dernière modification.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Suppression logique (1 = supprimé, 0 = actif).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual ProductionSeries IdProductionSeriesNavigation { get; set; } = null!;

    public virtual CuttingScrapStock? IdSourceScrapNavigation { get; set; }
}
