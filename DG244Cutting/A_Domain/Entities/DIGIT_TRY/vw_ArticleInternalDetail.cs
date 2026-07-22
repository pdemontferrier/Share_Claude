using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

public partial class vw_ArticleInternalDetail
{
    /// <summary>
    /// Identifiant unique de l’article interne.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Référence principale de l’article (famille produit).
    /// </summary>
    public string Reference { get; set; } = null!;

    /// <summary>
    /// Identifiant du couple RAL + finition utilisé par l’article interne.
    /// </summary>
    public string? IdColorRalFinish { get; set; }

    /// <summary>
    /// Désignation commerciale de la référence article.
    /// </summary>
    public string Designation { get; set; } = null!;

    /// <summary>
    /// Type d’identification : pièce, barre, boîte, etc.
    /// </summary>
    public string? IdentificationType { get; set; }

    /// <summary>
    /// Code de l’unité de stockage (ex : ML, PC, KG…).
    /// </summary>
    public string? StorageUnitCode { get; set; }

    /// <summary>
    /// Désignation complète de l’unité de stockage.
    /// </summary>
    public string? StorageUnitDesignation { get; set; }

    public double? StandardBarLengthMm { get; set; }

    /// <summary>
    /// Catégorie article niveau 1.
    /// </summary>
    public string? CategoryLevel1 { get; set; }

    /// <summary>
    /// Catégorie article niveau 2.
    /// </summary>
    public string? CategoryLevel2 { get; set; }

    /// <summary>
    /// Catégorie article niveau 3.
    /// </summary>
    public string? CategoryLevel3 { get; set; }

    /// <summary>
    /// Identifiant de la machine de coupe utilisée pour traiter cet article (ex : DG244).
    /// </summary>
    public string? CuttingMachineCode { get; set; }

    /// <summary>
    /// Ordre de tri conseillé pour les listes d’articles.
    /// </summary>
    public short SortOrder { get; set; }

    /// <summary>
    /// Indique si cet article doit être géré dans la logique des chutes.
    /// </summary>
    public bool ManageScraps { get; set; }

    /// <summary>
    /// Longueur minimale considérée comme une chute réutilisable.
    /// </summary>
    public decimal? MinScrapLength { get; set; }

    /// <summary>
    /// Identifiant du casier horizontal où les chutes sont rangées.
    /// </summary>
    public int IdScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Désignation de l’emplacement horizontal où ranger les chutes.
    /// </summary>
    public string? ScrapLocationHorizontal { get; set; }

    /// <summary>
    /// Identifiant du casier vertical où les chutes sont rangées.
    /// </summary>
    public int IdScrapLocationVertical { get; set; }

    /// <summary>
    /// Désignation de l’emplacement vertical où ranger les chutes.
    /// </summary>
    public string? ScrapLocationVertical { get; set; }
}
