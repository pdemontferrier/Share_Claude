using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Table définissant les types de bacs (bins) utilisés pour stocker ou contenir des articles.
/// </summary>
public partial class StockBinType
{
    /// <summary>
    /// Identifiant unique du type de bac.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Désignation du type de bac (ex: Bac plastique, Tiroir, Panier filaire).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Description optionnelle du type de bac.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière modification de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<StockBin> StockBins { get; set; } = new List<StockBin>();
}
