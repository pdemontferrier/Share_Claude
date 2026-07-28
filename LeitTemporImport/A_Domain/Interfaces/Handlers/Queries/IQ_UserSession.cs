using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de QueryHandler dédié à <see cref="UserAppSession"/>.
    /// Définit les requêtes nécessaires à la récupération des sessions applicatives
    /// afin de piloter l’ouverture et la fermeture du programme console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Services pour identifier les sessions existantes d’un utilisateur
    /// pour une application donnée, et déterminer la session “courante” à manipuler.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser les lectures autour des sessions applicatives, avec traçabilité via la callChain
    /// (paramètre <paramref name="caller"/>) et gestion asynchrone.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application (App / Business).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer les sessions d’un utilisateur pour une application.</description></item>
    /// <item><description>Déterminer un identifiant de session pertinent (0 si aucune session).</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserSession : IQ_Generic<UserAppSession>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne les sessions <see cref="UserAppSession"/> d’un utilisateur pour une application donnée.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors du démarrage/arrêt pour identifier les sessions existantes (dont la session courante).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture filtrée et robuste des sessions correspondant aux critères.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les paramètres d’entrée (côté implémentation).</description></item>
        /// <item><description>Charger les sessions correspondant à <c>IdUser</c> et <c>IdApplication</c>.</description></item>
        /// <item><description>Retourner la liste (éventuellement vide).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Liste des sessions correspondant aux critères.</returns>
        Task<List<UserAppSession>> HandleGetByUserIdAppIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’identifiant d’une session “courante” pour un utilisateur et une application.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé par les UseCases pour sélectionner une session à mettre à jour (ex : fermeture du programme).
        /// Si aucune session n’existe, la méthode retourne 0.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Fournir une sélection déterministe d’une session existante ou 0 en absence de session.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Charger les sessions existantes.</description></item>
        /// <item><description>Sélectionner la session la plus pertinente (règle définie dans l’implémentation).</description></item>
        /// <item><description>Retourner l’Id sélectionné, ou 0.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Id de la session, ou 0 si aucune session n’existe.</returns>
        Task<int> HandleGetSessionIdAsync( string caller, int userId, int appId, CancellationToken ct = default);
    }
}