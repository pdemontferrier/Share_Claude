using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l’entité <see cref="UserAppPageRight"/> dans
    /// le cadre du modèle CQRS. Hérite du socle de lecture <see cref="IQ_Generic{T}"/>
    /// (six lectures de base, non redéclarées ici) et ajoute une lecture spécialisée
    /// par clé fonctionnelle composite.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) et les DataProviders
    /// (chaîne (3)), sans accès direct au repository ni au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir un point d’entrée de requête traçable via CallChain, homogène avec les
    /// conventions projet, sans redéclarer les opérations du socle générique.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels et DataProviders de lecture.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher les droits d’accès d’un utilisateur sur une application donnée.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserAppPageRight : IQ_Generic<UserAppPageRight>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne la liste des droits d’accès (<see cref="UserAppPageRight"/>) d’un
        /// utilisateur pour une application donnée, exclusion faite des lignes
        /// logiquement supprimées, triées par code fonctionnel de page.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé pour résoudre les autorisations page par page d’un utilisateur sur
        /// une application cible.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la CallChain,
        /// sans repository spécialisé : le filtrage est délégué au socle de lecture.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="userId"/> et <paramref name="appId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré.</description></item>
        /// <item><description>Trier le résultat par <c>PageCode</c> (LINQ-to-Objects).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l’utilisateur concerné. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l’application cible. Doit être strictement positif.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppPageRight"/> correspondantes, triée par
        /// <c>PageCode</c> ; liste vide si aucune ligne ne correspond.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="userId"/> ou
        /// <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l’annulation est signalée via <paramref name="ct"/> avant ou pendant l’exécution.
        /// </exception>
        Task<List<UserAppPageRight>> HandleGetByUserIdAppIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default);

        #endregion
    }
}