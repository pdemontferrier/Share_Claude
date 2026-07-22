using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.B_UseCases.Handlers.Generic;

namespace DG244Cutting.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// QueryHandler (QH) dédié à l'entité <see cref="UserAppSession"/>. Il hérite de
    /// <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute une
    /// lecture spécialisée par clé fonctionnelle composite, sans repository spécialisé :
    /// la recherche par utilisateur et application est servie par la méthode héritée
    /// <c>HandleGetFilteredAsync</c>, qui délègue à <c>IR_Generic&lt;UserAppSession&gt;</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) et par les DataProviders
    /// (chaîne (3)). Aucun appel EF Core n'est porté ici : l'<c>AsNoTracking()</c> et
    /// l'accès au DbContext sont encapsulés dans <c>CR_Generic&lt;UserAppSession&gt;</c>
    /// (R-4.14.11, R-4.15.12).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une lecture CQRS traçable (CallChain) et robuste (classification
    /// d'exceptions homogène), conforme à §4.14.5 et §4.15.4 du 023.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels (lecture simple) et DataProviders (lecture composée).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher les sessions d'un utilisateur pour une application donnée.</description></item>
    /// </list>
    /// </summary>
    public class QH_UserAppSession : QH_Generic<UserAppSession>, IQ_UserAppSession
    {
        #region === Propriétés privées ===

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
        /// <para>Construit le QueryHandler UserAppSession.</para>
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
        /// <see cref="UserAppSession"/> (résolu par le DI vers <c>CR_Generic&lt;UserAppSession&gt;</c>).
        /// </param>
        /// <param name="classifier">Service de classification des exceptions.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est null.
        /// </exception>
        public QH_UserAppSession(
            IR_Generic<UserAppSession> repository,
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
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="userId"/> et <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Retourner la liste résultante (filtre IsDeleted porté par le prédicat).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l'application. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppSession"/> correspondantes ; liste vide si aucune.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> ou <paramref name="appId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        public async Task<List<UserAppSession>> HandleGetByUserIdAppIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByUserIdAppIdAsync)}";

            try
            {
                // Préconditions structurelles validées DANS le bloc try (patron standard
                // §4.7 ; §4.15.4 aligné). L'Ex_Business typée remonte intacte au
                // composant appelant : elle est interceptée par catch (Ex_Business)
                // { throw; } avant le catch terminal, sans requalification, et ce
                // indépendamment de sa position. Le placement dans le try garantit
                // en outre qu'une exception imprévue levée pendant l'évaluation de
                // la validation est requalifiée par le classifier via
                // catch (Exception ex). Ordre normatif : validation -> ct.
                if (userId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni pour la recherche de sessions est invalide : {userId}. Doit être strictement positif.");

                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant application fourni pour la recherche de sessions est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture filtrée déléguée au socle hérité de QH_Generic<UserAppSession>,
                // qui consomme IR_Generic<UserAppSession>.GetFilteredAsync : la clause WHERE
                // est générée et exécutée côté base de données par EF Core, encapsulé
                // dans CR_Generic<T> (R-4.14.11, R-4.15.12). Aucune API EF Core ici.
                // Le filtre IsDeleted est porté par le prédicat applicatif (aucun
                // filtrage automatique du socle — troisième asymétrie §4.15.4).
                return await HandleGetFilteredAsync(
                    callChain,
                    userAppSession =>
                        userAppSession.IdUser == userId &&
                        userAppSession.IdApplication == appId &&
                        !userAppSession.IsDeleted,
                    ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Retourne l'identifiant de la session retenue pour un couple (utilisateur,
        /// application), ou <c>0</c> si aucune session n'existe pour ce couple.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Lecture spécialisée par clé fonctionnelle composite servie par délégation :
        /// elle réutilise la lecture spécialisée <see cref="HandleGetByUserIdAppIdAsync"/>
        /// (qui délègue elle-même à <c>HandleGetFilteredAsync</c> hérité du socle),
        /// puis applique une transformation LINQ-to-Objects côté B_UseCases. Aucun
        /// repository spécialisé, aucun appel EF Core (R-4.14.11, R-4.15.12).
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Désigner de manière déterministe la session représentative du couple selon
        /// l'ordre total : <c>IsConnected</c> décroissant, puis <c>UpdatedAt</c>
        /// décroissant (valeur nulle ramenée à <see cref="DateTime.MinValue"/>), puis
        /// <c>CreatedAt</c> décroissant, puis <c>Id</c> décroissant ; retourner son
        /// <c>Id</c>, sinon <c>0</c>.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="userId"/> et <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Charger les sessions du couple via <see cref="HandleGetByUserIdAppIdAsync"/> (délégation public→public, redoublement assumé du segment _callee).</description></item>
        /// <item><description>Si la liste est vide, retourner <c>0</c>.</description></item>
        /// <item><description>Trier (LINQ-to-Objects) selon l'ordre total décroissant <c>IsConnected</c> &gt; <c>UpdatedAt</c> &gt; <c>CreatedAt</c> &gt; <c>Id</c>, prendre le premier, projeter son <c>Id</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l'application. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Identifiant de la session retenue pour le couple ; <c>0</c> si aucune session n'existe.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> ou <paramref name="appId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        public async Task<int> HandleGetSessionIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetSessionIdAsync)}";

            try
            {
                // Préconditions structurelles validées DANS le bloc try (patron standard
                // §4.7 ; §4.15.4 aligné ; R-4.7.25). L'Ex_Business typée remonte intacte
                // (catch (Ex_Business) { throw; } avant le catch terminal, sans
                // requalification). La revalidation ici, alors que la méthode déléguée
                // HandleGetByUserIdAppIdAsync revalide également, est le pendant en
                // précondition du redoublement intra-classe public→public assumé
                // (§4.15.4, Invariant 11 du 0232-QH). Ordre normatif : validation -> ct.
                if (userId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni pour la sélection de session est invalide : {userId}. Doit être strictement positif.");

                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant application fourni pour la sélection de session est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation à la lecture spécialisée existante (clé fonctionnelle
                // utilisateur × application × !IsDeleted), elle-même déléguée au socle
                // hérité HandleGetFilteredAsync. La CallChain enrichie est propagée :
                // le segment {_callee} est redoublé (… > QH_UserAppSession >
                // HandleGetSessionIdAsync > QH_UserAppSession > HandleGetByUserIdAppIdAsync
                // > QH_UserAppSession > HandleGetFilteredAsync). Redoublement
                // intra-classe public→public normatif et assumé (§4.15.4 ; Invariant 11).
                List<UserAppSession> sessions =
                    await HandleGetByUserIdAppIdAsync(callChain, userId, appId, ct);

                if (sessions.Count == 0)
                    return 0;

                // Transformation LINQ-to-Objects côté B_UseCases (autorisée par §4.14.6
                // et §4.14.5 : tri + réduction au premier enregistrement + projection
                // scalaire ; pas de Repository spécialisé requis). Aucune décision
                // métier : tri/réduction/projection sont des opérations de lecture
                // (§4.14.5, « Responsabilités exclues »).
                UserAppSession selected = sessions
                    .OrderByDescending(session => session.IsConnected)
                    .ThenByDescending(session => session.UpdatedAt ?? DateTime.MinValue)
                    .ThenByDescending(session => session.CreatedAt)
                    .ThenByDescending(session => session.Id)
                    .First();

                return selected.Id;
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