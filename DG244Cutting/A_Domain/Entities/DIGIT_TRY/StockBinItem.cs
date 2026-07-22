using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Représente les quantités d’articles neufs stockées dans un emplacement physique (StockBin).
/// </summary>
public partial class StockBinItem
{
    /// <summary>
    /// Identifiant unique du StockBinItem.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence vers l’article interne stocké (clé étrangère vers ArticleInternal).
    /// </summary>
    public int IdArticleInternal { get; set; }

    /// <summary>
    /// Référence vers le bac de stockage physique (clé étrangère vers StockBin).
    /// </summary>
    public int IdStockBin { get; set; }

    /// <summary>
    /// Quantité disponible dans le bac pour cet article interne.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Date du dernier inventaire effectué pour ce bac et cet article.
    /// </summary>
    public DateOnly InventoryDate { get; set; }

    /// <summary>
    /// Date à partir de laquelle la quantité est disponible pour l’utilisation ou la préparation.
    /// </summary>
    public DateTime AccessibleDate { get; set; }

    /// <summary>
    /// Indique si le stock est actuellement accessible (1 = oui, 0 = non).
    /// </summary>
    public bool IsAccessible { get; set; }

    /// <summary>
    /// Date et heure de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de la dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est logiquement supprimé (soft delete).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ArticleInternal IdArticleInternalNavigation { get; set; } = null!;

    public virtual StockBin IdStockBinNavigation { get; set; } = null!;
}
