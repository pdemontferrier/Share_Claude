using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// Contrat du QueryHandler (IQ) dédié à l’entité <see cref="UserAppAccess"/> dans le
    /// cadre du modèle CQRS.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : consommé en lecture par les ViewModels (chaîne (2)) et les DataProviders
    /// (chaîne (3)), sans accès direct au repository ni au DbContext. Utilisateurs cibles :
    /// ViewModels et DataProviders de lecture.
    /// </para>
    /// <para>
    /// Hérite du socle de lecture <see cref="IQ_Generic{T}"/>
    /// (six lectures de base, non redéclarées ici) et ajoute une lecture
    /// spécialisée par clé fonctionnelle composite.
    /// </para>
    /// <para>
    /// Objectif : fournir un point d’entrée de requête traçable via CallChain, homogène avec
    /// les conventions projet, sans redéclarer les opérations du socle générique.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déterminer si un utilisateur dispose d’un accès à une application.</description></item>
    /// </list>
    /// </remarks>
    public interface IQ_UserAppAccess : IQ_Generic<UserAppAccess>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Indique si l’utilisateur spécifié dispose d’un accès actif à l’application
        /// spécifiée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : utilisé pour le contrôle d’autorisation applicative globale par
        /// (utilisateur, application), à l’exclusion des associations logiquement
        /// supprimées.
        /// </para>
        /// <para>Objectif : permettre une requête CQRS dédiée, traçable et cohérente avec la CallChain.</para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="appId"/> et <paramref name="userId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré sur la clé fonctionnelle composite.</description></item>
        /// <item><description>Retourner <see langword="true"/> si au moins une association active existe.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">Identifiant de l’application. Doit être strictement positif.</param>
        /// <param name="userId">Identifiant de l’utilisateur. Doit être strictement positif.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// <see langword="true"/> si une association active (non supprimée) existe entre
        /// l’utilisateur et l’application ; <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="appId"/> ou <paramref name="userId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l’annulation est signalée via <paramref name="ct"/> avant ou pendant l’exécution.
        /// </exception>
        Task<bool> HandleHasUserAccessAppAsync(string caller, int appId, int userId, CancellationToken ct = default);

        #endregion
    }
}