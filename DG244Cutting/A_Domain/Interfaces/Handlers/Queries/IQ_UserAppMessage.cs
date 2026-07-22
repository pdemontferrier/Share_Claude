using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l'entité <see cref="UserAppMessage"/>
    /// dans le cadre du modèle CQRS. Hérite du socle de lecture
    /// <see cref="IQ_Generic{T}"/> (six lectures de base, non redéclarées ici) et
    /// ajoute trois lectures spécialisées par clé fonctionnelle
    /// <c>appId</c> (référence <c>AppList.Id</c>).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) — en particulier le
    /// VM_MainWindow pour la notification de présence de messages non lus, et les
    /// ViewModels d'affichage des listes reçues/envoyées — et le cas échéant par
    /// un UseCase orchestrateur (3e modalité §4.14.2 amendée : lecture
    /// complémentaire à l'orchestration sans assemblage composite). Aucun
    /// DataProvider n'est prévu : aucune de ces lectures ne donne lieu à un
    /// assemblage composite multi-sources.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir trois points d'entrée de requête traçables via CallChain,
    /// homogènes avec les conventions projet, sans redéclarer les opérations du
    /// socle générique.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels de présentation et UseCases orchestrateurs (consommation directe encadrée).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Obtenir la liste des messages reçus par une application donnée.</description></item>
    /// <item><description>Obtenir la liste des messages envoyés par une application donnée.</description></item>
    /// <item><description>Vérifier l'existence d'au moins un message non lu pour une application donnée.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_UserAppMessage : IQ_Generic<UserAppMessage>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne la liste des messages reçus par une application donnée,
        /// ordonnée par date d'envoi décroissante.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Alimente l'écran de consultation des messages entrants pour
        /// l'application identifiée par <paramref name="appId"/>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la
        /// CallChain, excluant les messages logiquement supprimés.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="appId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré sur le destinataire et la non-suppression logique.</description></item>
        /// <item><description>Ordonner le résultat par date d'envoi décroissante.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">
        /// Identifiant de l'application destinataire (référence <c>AppList.Id</c>).
        /// Doit être strictement positif.
        /// </param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppMessage"/> reçues par
        /// l'application, triées par <c>SentAt</c> décroissant ; liste vide si aucun message.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync(
            string caller,
            int appId,
            CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne la liste des messages envoyés par une application donnée,
        /// ordonnée par date d'envoi décroissante.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Alimente l'écran de consultation des messages sortants pour
        /// l'application identifiée par <paramref name="appId"/>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la
        /// CallChain, excluant les messages logiquement supprimés.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="appId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré sur l'émetteur et la non-suppression logique.</description></item>
        /// <item><description>Ordonner le résultat par date d'envoi décroissante.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">
        /// Identifiant de l'application émettrice (référence <c>AppList.Id</c>).
        /// Doit être strictement positif.
        /// </param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppMessage"/> envoyées par
        /// l'application, triées par <c>SentAt</c> décroissant ; liste vide si aucun message.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        Task<List<UserAppMessage>> HandleGetMessagesSentAsync(
            string caller,
            int appId,
            CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Indique s'il existe au moins un message non lu pour une application
        /// donnée (test d'existence).
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Alimente la notification de présence de messages non lus dans
        /// VM_MainWindow ou tout consommateur équivalent.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre une requête CQRS dédiée, traçable et cohérente avec la
        /// CallChain, retournant un simple booléen.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre <paramref name="appId"/>.</description></item>
        /// <item><description>Interroger le socle de lecture filtré sur destinataire, non lu et non suppression logique.</description></item>
        /// <item><description>Réduire le résultat à un test d'existence.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">
        /// Identifiant de l'application destinataire (référence <c>AppList.Id</c>).
        /// Doit être strictement positif.
        /// </param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// <see langword="true"/> si au moins un message non lu existe pour
        /// l'application ; <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        Task<bool> HandleGetAnyMessageNotReadAsync(
            string caller,
            int appId,
            CancellationToken ct = default);

        #endregion
    }
}