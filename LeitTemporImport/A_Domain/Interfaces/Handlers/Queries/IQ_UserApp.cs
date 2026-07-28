using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l’entité <see cref="UserApp"/> dans le cadre du modèle CQRS.
    /// Définit les requêtes de lecture spécifiques nécessaires aux UseCases.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases afin d’accéder aux informations de l’utilisateur applicatif (UserApp)
    /// sans accéder directement au repository ni au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir des points d’entrée de requêtes traçables via CallChain et homogènes avec les conventions projet 104.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher un utilisateur par login Windows.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserApp : IQ_Generic<UserApp>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour identifier l’utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>Permettre une requête CQRS dédiée, traçable et cohérente avec le modèle de CallChain.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="windowsLogin"/>.</description></item>
        /// <item><description>Interroger le QH UserApp.</description></item>
        /// <item><description>Retourner null si aucun utilisateur ne correspond.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> ou null.</returns>
        Task<UserApp?> HandleGetByWindowsLoginAsync(string caller, string windowsLogin, CancellationToken ct = default);

        #endregion
    }
}