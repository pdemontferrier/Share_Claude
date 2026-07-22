using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l'entité <see cref="UserAppSession"/> dans le
    /// cadre du modèle CQRS. Hérite du socle de lecture <see cref="IQ_Generic{T}"/>
    /// (six lectures de base, non redéclarées ici) et ajoute une lecture
    /// spécialisée par clé fonctionnelle composite (utilisateur × application).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) et les DataProviders
    /// (chaîne (3)), sans accès direct au repository ni au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir un point d'entrée de requête traçable via CallChain, homogène avec
    /// les conventions projet, sans redéclarer les opérations du socle générique.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels et DataProviders de lecture.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher les sessions d'un utilisateur pour une application donnée.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserAppSession : IQ_Generic<UserAppSession>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne la liste des sessions d'un utilisateur pour une application donnée,
        /// à l'exclusion des sessions logiquement supprimées.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé pour consulter l'état des sessions ouvertes par un utilisateur sur
        /// une application interne identifiée.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la CallChain,
        /// servie par délégation au socle de lecture filtré.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les paramètres <paramref name="userId"/> et <paramref name="appId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré.</description></item>
        /// <item><description>Retourner la liste (éventuellement vide) des sessions correspondantes.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l'application. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppSession"/> de l'utilisateur pour
        /// l'application, hors sessions supprimées logiquement ; liste vide si aucune.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> ou <paramref name="appId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<UserAppSession>> HandleGetByUserIdAppIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne l'identifiant de la session retenue pour un couple (utilisateur,
        /// application), ou <c>0</c> si aucune session n'existe pour ce couple.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Consomme la lecture spécialisée <see cref="HandleGetByUserIdAppIdAsync"/>
        /// (sessions du couple, hors sessions logiquement supprimées) puis réduit le
        /// jeu obtenu à un seul identifiant par une transformation LINQ-to-Objects
        /// côté B_UseCases. Aucune décision métier n'est portée : le tri, la réduction
        /// et la projection sont des opérations de lecture (§4.14.5 du 023).
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Désigner de manière déterministe la session représentative du couple selon
        /// l'ordre total : <c>IsConnected</c> décroissant, puis <c>UpdatedAt</c>
        /// décroissant (valeur nulle ramenée à la borne minimale), puis
        /// <c>CreatedAt</c> décroissant, puis <c>Id</c> décroissant ; retourner son
        /// <c>Id</c>, sinon <c>0</c>.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les paramètres <paramref name="userId"/> et <paramref name="appId"/>.</description></item>
        /// <item><description>Charger les sessions du couple via <see cref="HandleGetByUserIdAppIdAsync"/>.</description></item>
        /// <item><description>Si aucune session, retourner <c>0</c>.</description></item>
        /// <item><description>Trier selon l'ordre total <c>IsConnected</c> &gt; <c>UpdatedAt</c> &gt; <c>CreatedAt</c> &gt; <c>Id</c> (tous décroissants), prendre le premier, retourner son <c>Id</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l'application. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Identifiant de la session retenue pour le couple (utilisateur, application) ;
        /// <c>0</c> si aucune session n'existe pour ce couple.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> ou <paramref name="appId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<int> HandleGetSessionIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default);

        #endregion
    }
}