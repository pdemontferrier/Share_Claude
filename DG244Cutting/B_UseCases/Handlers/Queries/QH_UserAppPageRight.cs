using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.B_UseCases.Handlers.Generic;

namespace DG244Cutting.B_UseCases.Handlers.Queries
{
    /// <summary>
    /// QueryHandler (QH) dédié à l’entité <see cref="UserAppPageRight"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : consommé en lecture par les ViewModels (chaîne (2)) et par les DataProviders
    /// (chaîne (3)). Aucun appel EF Core n’est porté ici : l’<c>AsNoTracking()</c> et
    /// l’accès au DbContext sont encapsulés dans <c>CR_Generic&lt;UserAppPageRight&gt;</c>
    /// (R-4.14.11, R-4.15.12). Utilisateurs cibles : ViewModels (lecture simple) et
    /// DataProviders (lecture composée).
    /// </para>
    /// <para>
    /// Il hérite de
    /// <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute une
    /// lecture spécialisée par clé fonctionnelle composite, sans repository
    /// spécialisé : la recherche par utilisateur et application est servie par la
    /// méthode héritée <c>HandleGetFilteredAsync</c>, qui délègue à
    /// <c>IR_Generic&lt;UserAppPageRight&gt;</c>.
    /// </para>
    /// <para>
    /// Objectif : fournir une lecture CQRS traçable (CallChain) et robuste (classification
    /// d’exceptions homogène), conforme à §4.14.5 et §4.15.4 du 0230.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Rechercher les droits d’accès d’un utilisateur sur une application.</description></item>
    /// </list>
    /// </remarks>
    public class QH_UserAppPageRight : QH_Generic<UserAppPageRight>, IQ_UserAppPageRight
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
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
        /// Initialise une nouvelle instance de <see cref="QH_UserAppPageRight"/> avec ses
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
        /// <see cref="UserAppPageRight"/> (résolu par le DI vers
        /// <c>CR_Generic&lt;UserAppPageRight&gt;</c>).
        /// </param>
        /// <param name="classifier">Service de classification des exceptions.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est null.
        /// </exception>
        public QH_UserAppPageRight(
            IR_Generic<UserAppPageRight> repository,
            IS_ExClassifier classifier)
            : base(repository, classifier)
        {
            _callee = GetType().Name;

            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        }

        #endregion


        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne la liste des droits d’accès (<see cref="UserAppPageRight"/>) d’un
        /// utilisateur pour une application donnée, exclusion faite des lignes
        /// logiquement supprimées, triées par code fonctionnel de page.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : utilisé pour résoudre les autorisations page par page d’un utilisateur sur
        /// une application cible (device user).
        /// </para>
        /// <para>
        /// Objectif : fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le filtrage est délégué au socle générique hérité.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles sur <paramref name="userId"/> et <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le filtrage à <c>HandleGetFilteredAsync</c> (socle hérité).</description></item>
        /// <item><description>Trier le résultat par <c>PageCode</c> (LINQ-to-Objects).</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant de l’utilisateur concerné. Doit être strictement positif.</param>
        /// <param name="appId">Identifiant de l’application cible. Doit être strictement positif.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// Liste des entités <see cref="UserAppPageRight"/> correspondantes, triée par
        /// <c>PageCode</c> ; liste vide si aucune ligne ne correspond.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="userId"/> ou
        /// <paramref name="appId"/> est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l’annulation est signalée via <paramref name="ct"/> avant ou pendant l’exécution.
        /// </exception>
        public async Task<List<UserAppPageRight>> HandleGetByUserIdAppIdAsync(
            string caller,
            int userId,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByUserIdAppIdAsync)}";

            try
            {
                // Préconditions structurelles validées DANS le bloc try (patron
                // standard §4.7 ; §4.15.4 aligné), dans l'ordre validation -> ct.
                // L'Ex_Business typée remonte intacte via catch (Ex_Business) { throw; }
                // avant le catch terminal, sans requalification.
                if (userId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni pour {nameof(UserAppPageRight)} est invalide : {userId}. Doit être strictement positif.");

                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant application fourni pour {nameof(UserAppPageRight)} est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture filtrée déléguée au socle hérité de QH_Generic<UserAppPageRight>,
                // qui consomme IR_Generic<UserAppPageRight>.GetFilteredAsync : la clause
                // WHERE est générée et exécutée côté base de données par EF Core, encapsulé
                // dans CR_Generic<T> (R-4.14.11, R-4.15.12). Aucune API EF Core ici.
                // Le filtre IsDeleted est porté par le prédicat applicatif (aucun
                // filtrage automatique du socle, 3e asymétrie §4.15.4).
                List<UserAppPageRight> matches = await HandleGetFilteredAsync(
                    callChain,
                    record =>
                        record.IdUser == userId &&
                        record.IdApplication == appId &&
                        !record.IsDeleted,
                    ct);

                // Transformation LINQ-to-Objects en B_UseCases (tri autorisé par §4.14.6 :
                // pas de Repository spécialisé requis pour un ordonnancement post-chargement).
                return matches
                    .OrderBy(record => record.PageCode)
                    .ToList();
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