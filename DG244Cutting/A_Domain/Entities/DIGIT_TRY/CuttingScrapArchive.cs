using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Historique complet des chutes issues des opérations de découpe.
/// </summary>
public partial class CuttingScrapArchive
{
    /// <summary>
    /// Identifiant unique de la ligne dans l’archive des chutes.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence vers l’article interne correspondant à la chute enregistrée.
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Type de chute (ex : barre, panneau). Utilisé si plusieurs types de chutes doivent être différenciés.
    /// </summary>
    public int? IdCutType { get; set; }

    /// <summary>
    /// Utilisateur (UserApp) ayant enregistré l’entrée de la chute dans le stock.
    /// </summary>
    public int? IdOperatorEntry { get; set; }

    /// <summary>
    /// Utilisateur (UserApp) ayant consommé ou sorti la chute du stock.
    /// </summary>
    public int? IdOperatorExit { get; set; }

    /// <summary>
    /// Longueur de la chute en millimètres.
    /// </summary>
    public int LengthMm { get; set; }

    /// <summary>
    /// Largeur de la chute (si applicable). Peut rester NULL pour les barres.
    /// </summary>
    public int? WidthMm { get; set; }

    /// <summary>
    /// Valeur estimée de la chute, utilisée pour le calcul d’inventaire ou d’analyse de coûts.
    /// </summary>
    public double Price { get; set; }

    /// <summary>
    /// Indique si la chute a été scannée lors de son entrée dans le stock (0 = non, 1 = oui).
    /// </summary>
    public bool IsScannedOnEntry { get; set; }

    /// <summary>
    /// Indique si la sortie ou consommation de la chute a été réalisée via un scan.
    /// </summary>
    public bool IsScannedOnExit { get; set; }

    /// <summary>
    /// Date et heure d’entrée de la chute dans le stock.
    /// </summary>
    public DateTime? EntryDate { get; set; }

    /// <summary>
    /// Date et heure de sortie ou de consommation de la chute.
    /// </summary>
    public DateTime? ExitDate { get; set; }

    /// <summary>
    /// Indication de réservation de la chute (ex : numéro de lot, commande, projet).
    /// </summary>
    public string? ReservedFor { get; set; }

    /// <summary>
    /// Code-barres associé à la chute pour identification automatique.
    /// </summary>
    public string? Barcode { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement dans le système.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Dernière date de mise à jour de la ligne.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la ligne est supprimée logiquement (soft delete). 0 = actif, 1 = supprimé.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual UserApp? IdOperatorEntryNavigation { get; set; }

    public virtual UserApp? IdOperatorExitNavigation { get; set; }
}
