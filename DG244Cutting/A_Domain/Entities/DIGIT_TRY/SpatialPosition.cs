using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Referentiel de correspondance des positions spatiales issues de Tempor_Import.Feld_6.
/// </summary>
public partial class SpatialPosition
{
    /// <summary>
    /// Identifiant technique unique de la position spatiale.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Code source issu de Tempor_Import.Feld_6.
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Description fonctionnelle lisible de la position ou du type de piece.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Position spatiale normalisee (Haut, Bas, Gauche, Droite, Horizontal, Vertical, Croisillon, NA).
    /// </summary>
    public string Position { get; set; } = null!;

    /// <summary>
    /// Date de creation de l&apos;enregistrement.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de derniere mise a jour de l&apos;enregistrement.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Indique si la position spatiale est supprimee logiquement.
    /// </summary>
    public bool IsDeleted { get; set; }

    public virtual ICollection<ProductionCutPiece> ProductionCutPieces { get; set; } = new List<ProductionCutPiece>();
}
