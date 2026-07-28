using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du repository (IR) dédié à l’entité <see cref="UserApp"/> pour la base DIGIT_TRY.
    /// Définit les opérations de lecture spécifiques liées aux utilisateurs applicatifs.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par la couche Infrastructure (QueryHandlers, Services) afin d’accéder aux données UserApp
    /// via EF Core sans exposer le DbContext aux couches supérieures.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les requêtes spécifiques (ex : recherche par login Windows) et garantir la traçabilité via CallChain.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>QueryHandlers (QH) et Services Infrastructure.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer un utilisateur à partir d’un login Windows.</description></item>
    /// </list>
    /// </summary>
    public interface IR_UserApp : IR_Generic<UserApp>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors d’une identification de l’utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>Permettre une lecture optimisée (AsNoTracking) par champ WindowsLogin.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Interroger UserApp par WindowsLogin.</description></item>
        /// <item><description>Retourner null si aucun utilisateur ne correspond.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> ou null.</returns>
        Task<UserApp?> GetByWindowsLoginAsync(string caller, string windowsLogin, CancellationToken ct = default);

        #endregion
    }
}
