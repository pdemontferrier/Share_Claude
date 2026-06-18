namespace DG244Cutting.A_Domain.DTOs.App
{
    /// <summary>
    /// Objet de transport du contexte applicatif et utilisateur courant.
    /// </summary>
    /// <remarks>
    /// <para>Contexte : DTO sans comportement défini dans A_Domain, utilisable par
    /// toutes les couches sans dépendance croisée.</para>
    /// <para>Objectif : transporter un instantané des valeurs de contexte applicatif
    /// et utilisateur agrégées par le service <c>IS_AppContext</c> / <c>SR_AppContext</c>.</para>
    /// <para>Non-responsabilités : aucune logique métier, aucune validation, aucune
    /// référence à EF Core.</para>
    /// </remarks>
    public class DTO_AppContext
    {
        /// <summary>Identifiant de l'application dans l'écosystème ERP.</summary>
        public int AppId { get; set; }

        /// <summary>Date du jour, sans composante horaire.</summary>
        public DateTime AppDate { get; set; }

        /// <summary>Date et heure système courantes.</summary>
        public DateTime AppDateTime { get; set; }

        /// <summary>Identifiant de l'utilisateur applicatif courant.</summary>
        public int AppUserId { get; set; }

        /// <summary>Compte système du poste courant.</summary>
        public string? AppDeviceUser { get; set; }

        /// <summary>Identifiant du poste courant.</summary>
        public string? AppDeviceId { get; set; }

        /// <summary>Adresse IP du poste courant.</summary>
        public string? AppDeviceIP { get; set; }
    }
}