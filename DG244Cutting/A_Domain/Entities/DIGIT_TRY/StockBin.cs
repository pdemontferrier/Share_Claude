using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Contenant physique (bac, boîte, plateau…) utilisé dans les zones de stockage.
/// </summary>
public partial class StockBin
{
    /// <summary>
    /// Identifiant unique du bac (clé primaire).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l’adresse de zone dans laquelle se trouve le bac.
    /// </summary>
    public int IdStockZoneAddress { get; set; }

    /// <summary>
    /// Type de support présent dans l’adresse (rack, sol, structure spéciale…).
    /// </summary>
    public int? IdStockSupportType { get; set; }

    /// <summary>
    /// Type de bac (boîte, plateau, panier, cassette…).
    /// </summary>
    public int IdStockBinType { get; set; }

    /// <summary>
    /// Désignation unique du bac (ex. : C45, A12).
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Indique si le bac est déplaçable (TRUE = mobile).
    /// </summary>
    public bool IsMovable { get; set; }

    /// <summary>
    /// Capacité maximale théorique du bac (en nombre d’unités).
    /// </summary>
    public int? MaxItems { get; set; }

    /// <summary>
    /// Nombre actuel de contenants présents dans le bac.
    /// </summary>
    public int? CurrentItems { get; set; }

    /// <summary>
    /// Date de création de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si l’enregistrement est supprimé logiquement (1 = supprimé).
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual StockBinType IdStockBinTypeNavigation { get; set; } = null!;

    public virtual StockSupportType? IdStockSupportTypeNavigation { get; set; }

    public virtual StockZoneAddress IdStockZoneAddressNavigation { get; set; } = null!;

    public virtual ICollection<StockBinItem> StockBinItems { get; set; } = new List<StockBinItem>();
}
