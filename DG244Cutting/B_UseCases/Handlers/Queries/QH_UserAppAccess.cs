using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.B_UseCases.Handlers.Generic;

namespace DG244Cutting.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// QueryHandler (QH) dédié à l’entité <see cref="UserAppAccess"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : consommé en lecture par les ViewModels (chaîne (2)) et par les DataProviders
    /// (chaîne (3)). Aucun appel EF Core n’est porté ici : l’<c>AsNoTracking()</c> et
    /// l’accès au DbContext sont encapsulés dans <c>CR_Generic&lt;UserAppAccess&gt;</c>
    /// (R-4.14.11, R-4.15.12). Utilisateurs cibles : ViewModels (lecture simple) et
    /// DataProviders (lecture composée).
    /// </para>
    /// <para>
    /// Il hérite de
    /// <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute une
    /// lecture spécialisée par clé fonctionnelle composite, sans repository spécialisé :
    /// la vérification d’accès est servie par la méthode héritée
    /// <c>HandleGetFilteredAsync</c>, qui délègue à <c>IR_Generic&lt;UserAppAccess&gt;</c>.
    /// </para>
    /// <para>
    /// Objectif : fournir une lecture CQRS traçable (CallChain) et robuste (classification
    /// d’exceptions homogène), conforme à §4.14.5 et §4.15.4 du 0230.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déterminer si un utilisateur dispose d’un accès à une application.</description></item>
    /// </list>
    /// </remarks>
    public class QH_UserAppAccess : QH_Generic<UserAppAccess>, IQ_UserAppAccess
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement (<c>GetType().Name</c>),
        /// utilisé pour la construction de la CallChain dans les méthodes
        /// publiques portant leur propre bloc de capture.
        /// </summary>
        /// <remarks>
        /// Re-déclaré ici car le champ homonyme de <see cref="QH_Generic{T}"/> est
        /// <c>private</c> (§4.15.4, « Aucune surface protégée pour la dérivation »).
        /// </remarks>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types
        /// applicatifs normalisés (<see cref="Ex_Infrastructure"/> ou
        /// <see cref="Ex_Unclassified"/>).
        /// </summary>
        /// <remarks>
        /// Réinjecté au dérivé : la dépendance <c>_classifier</c> de
        /// <c>QH_Generic&lt;T&gt;</c> est <c>private</c> (aucune surface protected, §4.15.4)
        /// et n’est donc pas accessible depuis la classe dérivée pour son propre catch.
        /// </remarks>
        private readonly IS_ExClassifier _classifier;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="QH_UserAppAccess"/> avec ses
        /// dépendances opérationnelles.
        /// </summary>
        /// <remarks>
        /// <para>Contexte : instancié via DI dans la couche B_UseCases.</para>
        /// <para>
        /// Objectif : transmettre le repository générique et le classifier au socle
        /// <see cref="QH_Generic{T}"/> et initialiser l’identité de composant
        /// utilisée par la CallChain.
        /// </para>
        /// </remarks>
        /// <param name="repository">
        /// Repository générique <see cref="IR_Generic{T}"/> de l’entité
        /// <see cref="UserAppAccess"/> (résolu par le DI vers <c>CR_Generic&lt;UserAppAccess&gt;</c>).
        /// </param>
        /// <param name="classifier">Service de classification des exceptions.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est null.
        /// </exception>
        public QH_UserAppAccess(
            IR_Generic<UserAppAccess> repository,
            IS_ExClassifier classifier)
            : base(repository, classifier)
        {
            _callee = GetType().Name;

            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion

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
        /// <para>
        /// Objectif : fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="appId"/> et <paramref name="userId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Réduire le résultat à un booléen d’existence (LINQ-to-Objects).</description></item>
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
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="appId"/> ou <paramref name="userId"/>
        /// est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l’annulation est signalée via <paramref name="ct"/> avant ou pendant l’exécution.
        /// </exception>
        public async Task<bool> HandleHasUserAccessAppAsync(
            string caller,
            int appId,
            int userId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleHasUserAccessAppAsync)}";

            try
            {
                // Préconditions structurelles validées DANS le bloc try (patron standard
                // §4.7 ; §4.15.4 aligné). Les Ex_Business typées remontent intactes au
                // composant appelant : interceptées par catch (Ex_Business) { throw; }
                // avant le catch terminal, sans requalification. Le placement dans le try
                // garantit en outre qu'une exception imprévue levée pendant l'évaluation
                // de la validation est requalifiée par le classifier via catch (Exception ex).
                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application fourni pour {nameof(UserAppAccess)} est invalide : {appId}. Doit être strictement positif.");

                if (userId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'utilisateur fourni pour {nameof(UserAppAccess)} est invalide : {userId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture filtrée déléguée au socle hérité de QH_Generic<UserAppAccess>,
                // qui consomme IR_Generic<UserAppAccess>.GetFilteredAsync : la clause WHERE
                // est générée et exécutée côté base de données par EF Core, encapsulé
                // dans CR_Generic<T> (R-4.14.11, R-4.15.12). Aucune API EF Core ici.
                // Le filtre IsDeleted est porté par le prédicat applicatif (3e asymétrie
                // assumée, §4.15.4 : aucun filtrage automatique du socle).
                List<UserAppAccess> matches = await HandleGetFilteredAsync(
                    callChain,
                    ua => ua.IdUser == userId && ua.IdApplication == appId && !ua.IsDeleted,
                    ct);

                // Réduction LINQ-to-Objects en B_UseCases (autorisée par §4.14.6 :
                // pas de Repository spécialisé requis pour une réduction post-chargement).
                return matches.Count != 0;
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