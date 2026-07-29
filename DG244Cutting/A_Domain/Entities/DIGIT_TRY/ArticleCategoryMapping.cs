using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Referentiel des categories de piece derivees du couple Feld_6 / Feld_40 de Tempor_Import.
/// </summary>
public partial class ArticleCategoryMapping
{
    /// <summary>
    /// Identifiant technique unique de la categorie de piece.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Code categorie principale, issu de Tempor_Import.Feld_6.
    /// </summary>
    public string Source_Feld_6 { get; set; } = null!;

    /// <summary>
    /// Code d’affinage de la categorie, issu de Tempor_Import.Feld_40 (peut valoir (kein)).
    /// </summary>
    public string Source_Feld_40 { get; set; } = null!;

    /// <summary>
    /// Designation metier normalisee de la categorie de piece.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Ordre d’affichage de la categorie (valeur par defaut 1).
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Date de creation de l’enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de derniere mise a jour de l’enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la categorie est supprimee logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }
}
