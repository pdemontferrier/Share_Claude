using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.B_UseCases.Handlers.Generic;

namespace DG244Cutting.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// QueryHandler (QH) dédié à l'entité <see cref="UserAppMessage"/>. Il hérite
    /// de <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute
    /// trois lectures spécialisées par clé fonctionnelle <c>appId</c>, sans
    /// repository spécialisé : les filtrages sont servis par la méthode héritée
    /// <c>HandleGetFilteredAsync</c>, qui délègue à <c>IR_Generic&lt;UserAppMessage&gt;</c>.
    /// Les transformations finales (tri par <c>SentAt</c> décroissant pour les
    /// listes, test d'existence pour le booléen) sont opérées en LINQ-to-Objects
    /// côté B_UseCases.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) — notamment
    /// VM_MainWindow pour la notification de messages non lus et les ViewModels
    /// d'affichage des listes reçues/envoyées — et le cas échéant par un
    /// UseCase orchestrateur (3e modalité §4.14.2 amendée). Aucun appel EF Core
    /// n'est porté ici : l'accès au DbContext est encapsulé dans
    /// <c>CR_Generic&lt;UserAppMessage&gt;</c> (R-4.14.11, R-4.15.12).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir trois lectures CQRS traçables (CallChain) et robustes
    /// (classification d'exceptions homogène), conformes à §4.14.5 et §4.15.4 du 0230.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels de présentation et UseCases orchestrateurs.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lister les messages reçus par une application, triés par date décroissante.</description></item>
    /// <item><description>Lister les messages envoyés par une application, triés par date décroissante.</description></item>
    /// <item><description>Tester l'existence d'au moins un message non lu pour une application.</description></item>
    /// </list>
    /// </summary>
    public class QH_UserAppMessage : QH_Generic<UserAppMessage>, IQ_UserAppMessage
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        // _classifier est réinjecté au dérivé : la dépendance _classifier de
        // QH_Generic<T> est private (aucune surface protected, §4.15.4) et n'est
        // donc pas accessible depuis la classe dérivée pour son propre catch.
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le QueryHandler UserAppMessage.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche B_UseCases.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Transmettre le repository générique et le classifier au socle
        /// <see cref="QH_Generic{T}"/> et initialiser l'identité de composant
        /// utilisée par la CallChain.
        /// </para>
        /// </summary>
        /// <param name="repository">
        /// Repository générique <see cref="IR_Generic{T}"/> de l'entité
        /// <see cref="UserAppMessage"/> (résolu par le DI vers
        /// <c>CR_Generic&lt;UserAppMessage&gt;</c>).
        /// </param>
        /// <param name="classifier">Service de classification des exceptions.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est null.
        /// </exception>
        public QH_UserAppMessage(
            IR_Generic<UserAppMessage> repository,
            IS_ExClassifier classifier)
            : base(repository, classifier)
        {
            _callee = GetType().Name;

            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion


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
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité, le
        /// tri est opéré en LINQ-to-Objects.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Trier le résultat par <c>SentAt</c> décroissant.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">Identifiant de l'application destinataire (référence <c>AppList.Id</c>).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppMessage"/> reçues par
        /// l'application, triées par <c>SentAt</c> décroissant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        public async Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync(
            string caller,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetMessagesReceivedAsync)}";

            try
            {
                // Précondition structurelle dans le bloc try (R-4.7.25, §4.15.4) :
                // l'Ex_Business typée remonte intacte au composant appelant via
                // catch (Ex_Business) { throw; } avant le catch terminal.
                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application fourni pour {nameof(UserAppMessage)} est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture filtrée déléguée au socle hérité de QH_Generic<UserAppMessage>,
                // qui consomme IR_Generic<UserAppMessage>.GetFilteredAsync : la clause
                // WHERE est générée et exécutée côté base de données par EF Core,
                // encapsulé dans CR_Generic<T> (R-4.14.11, R-4.15.12). Aucune API EF
                // Core ici. Filtre IsDeleted porté par le prédicat applicatif (3e
                // asymétrie assumée §4.15.4 ; item QH-17).
                List<UserAppMessage> matches = await HandleGetFilteredAsync(
                    callChain,
                    m => m.IdApplicationRecipient == appId && !m.IsDeleted,
                    ct);

                // Transformation LINQ-to-Objects en B_UseCases (autorisée §4.14.6 :
                // pas de Repository spécialisé requis pour un tri post-chargement).
                return matches.OrderByDescending(m => m.SentAt).ToList();
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

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
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité, le
        /// tri est opéré en LINQ-to-Objects.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Trier le résultat par <c>SentAt</c> décroissant.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">Identifiant de l'application émettrice (référence <c>AppList.Id</c>).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppMessage"/> envoyées par
        /// l'application, triées par <c>SentAt</c> décroissant.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        public async Task<List<UserAppMessage>> HandleGetMessagesSentAsync(
            string caller,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetMessagesSentAsync)}";

            try
            {
                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application fourni pour {nameof(UserAppMessage)} est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                List<UserAppMessage> matches = await HandleGetFilteredAsync(
                    callChain,
                    m => m.IdApplicationSender == appId && !m.IsDeleted,
                    ct);

                return matches.OrderByDescending(m => m.SentAt).ToList();
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

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
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité, la
        /// réduction est opérée en LINQ-to-Objects via <c>Any()</c>.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Réduire le résultat à un test d'existence.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">Identifiant de l'application destinataire (référence <c>AppList.Id</c>).</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// <see langword="true"/> si au moins un message non lu existe pour
        /// l'application ; <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        public async Task<bool> HandleGetAnyMessageNotReadAsync(
            string caller,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAnyMessageNotReadAsync)}";

            try
            {
                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application fourni pour {nameof(UserAppMessage)} est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                List<UserAppMessage> matches = await HandleGetFilteredAsync(
                    callChain,
                    m => m.IdApplicationRecipient == appId && !m.IsRead && !m.IsDeleted,
                    ct);

                return matches.Any();
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}