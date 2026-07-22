using System;
using System.Collections.Generic;

namespace DG244Cutting.A_Domain.Entities.DIGIT_TRY;

/// <summary>
/// Referentiel des positions laterales normalisees associant SideIndex et SpacePositionIndex.
/// </summary>
public partial class SidePosition
{
    /// <summary>
    /// Identifiant technique unique de la position laterale normalisee.
    /// </summary>
    public short Id { get; set; }

    /// <summary>
    /// Indice lateral technique utilise pour le parcours des pieces dans le chassis.
    /// </summary>
    public short SideIndex { get; set; }

    /// <summary>
    /// Indice spatial global utilise pour l’ordonnancement circulaire des positions.
    /// </summary>
    public short SpacePositionIndex { get; set; }

    /// <summary>
    /// Designation lisible de la position laterale pour affichage et reporting.
    /// </summary>
    public string Description { get; set; } = null!;

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
}
