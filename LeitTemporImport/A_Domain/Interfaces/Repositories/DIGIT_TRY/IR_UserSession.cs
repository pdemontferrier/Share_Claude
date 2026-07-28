using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de repository spécialisé pour l’entité <see cref="UserAppSession"/>.
    /// Définit les opérations de lecture spécifiques nécessaires au pilotage des sessions
    /// applicatives (ouverture/fermeture) dans le cadre de l’application console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases/Handlers de démarrage et d’arrêt du programme afin de retrouver
    /// les sessions existantes d’un utilisateur pour une application donnée.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser les requêtes de lecture spécifiques à <see cref="UserAppSession"/>,
    /// tout en conservant les opérations CRUD génériques via l’héritage de <see cref="IR_Generic{T}"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche UseCases : CommandHandlers, Services, UseCases.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exposer une recherche des sessions par utilisateur et application.</description></item>
    /// <item><description>Permettre l’exploitation des sessions pour l’ouverture et la fermeture du programme.</description></item>
    /// </list>
    /// </summary>
    public interface IR_UserSession : IR_Generic<UserAppSession>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne la liste des sessions <see cref="UserAppSession"/> correspondant à un utilisateur
        /// et une application donnés.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé lors du démarrage/arrêt du programme pour identifier les sessions existantes,
        /// détecter une session encore ouverte, ou reconstituer l’historique.
        /// </para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture ciblée, filtrée et robuste des sessions.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les paramètres d’entrée.</description></item>
        /// <item><description>Interroger les sessions par <c>IdUser</c> et <c>IdApplication</c>.</description></item>
        /// <item><description>Retourner une liste (éventuellement vide).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="appId">Identifiant de l’application.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Liste des sessions correspondant aux critères.</returns>
        Task<List<UserAppSession>> GetByUserIdAppIdAsync(string caller, int userId, int appId, CancellationToken ct = default);
    }
}