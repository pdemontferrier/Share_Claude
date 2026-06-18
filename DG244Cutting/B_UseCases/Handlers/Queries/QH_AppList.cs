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
    /// QueryHandler (QH) dédié à l’entité <see cref="AppList"/>. Il hérite de
    /// <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute une
    /// lecture spécialisée par clé fonctionnelle, sans repository spécialisé : la
    /// vérification d’accessibilité d’une application est servie par la méthode
    /// héritée <c>HandleAnyByPredicateAsync</c>, qui délègue à
    /// <c>IR_Generic&lt;AppList&gt;.AnyByPredicateAsync</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les composants amont autorisés par §4.14.9 du 0230,
    /// via le contrat <see cref="IQ_AppList"/>. Aucun appel EF Core n’est porté
    /// ici : le test d’existence est traduit en <c>SELECT TOP 1 1 WHERE ...</c>
    /// côté SGBD, sans matérialisation d’entité, l’accès au DbContext étant
    /// encapsulé dans <c>CR_Generic&lt;AppList&gt;</c> (R-4.14.11, R-4.15.12).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une lecture CQRS traçable (CallChain) et robuste (classification
    /// d’exceptions homogène), conforme à §4.14.5 et §4.15.4 du 0230.
    /// </para>
    /// </summary>
    public class QH_AppList : QH_Generic<AppList>, IQ_AppList
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom du composant courant, résolu dynamiquement (<c>GetType().Name</c>),
        /// utilisé pour la construction de la CallChain dans les méthodes
        /// publiques portant leur propre bloc de capture. Re-déclaré ici car le
        /// champ homonyme de <see cref="QH_Generic{T}"/> est <c>private</c>
        /// (§4.15.4, « Aucune surface protégée pour la dérivation »).
        /// </summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types
        /// applicatifs normalisés (<see cref="Ex_Infrastructure"/> ou
        /// <see cref="Ex_Unclassified"/>). Ré-injecté ici car le champ homonyme
        /// de <see cref="QH_Generic{T}"/> est <c>private</c> (§4.15.4) et
        /// indispensable au catch terminal des méthodes spécialisées.
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="QH_AppList"/> avec ses
        /// dépendances opérationnelles. La première instruction est
        /// <c>base(repository, classifier)</c>, conformément au patron principal
        /// d’extension par dérivation (R-3.14.7, §4.15.4).
        /// </summary>
        /// <param name="repository">
        /// Repository générique EF Core pour les lectures d’entités
        /// <see cref="AppList"/>. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types
        /// applicatifs normalisés. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est <see langword="null"/>
        /// (la nullité de <paramref name="repository"/> est, elle, vérifiée par
        /// le constructeur de la classe de base).
        /// </exception>
        public QH_AppList(
            IR_Generic<AppList> repository,
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
        /// Retourne <see langword="true"/> si l’application identifiée par
        /// <paramref name="appId"/> est actuellement accessible et non supprimée
        /// logiquement, <see langword="false"/> sinon.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé pour évaluer l’accessibilité d’une application avant toute
        /// opération qui la concerne (ouverture de session, accès à un écran,
        /// etc.). Aucune distinction n’est exposée entre « n’existe pas » et
        /// « existe mais non accessible ».
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : le test d’existence est délégué au socle générique
        /// hérité, qui le traduit en <c>SELECT TOP 1 1 WHERE ...</c> côté SGBD
        /// sans matérialiser d’entité.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="appId"/> (dans le try).</description></item>
        /// <item><description>Déléguer la vérification d’existence à <c>HandleAnyByPredicateAsync</c> (socle hérité) avec le prédicat composite <c>Id == appId &amp;&amp; Accessible &amp;&amp; !IsDeleted</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="appId">Identifiant d’application à interroger. Doit être strictement positif.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>
        /// <see langword="true"/> si au moins un <see cref="AppList"/> satisfait
        /// <c>Id == appId &amp;&amp; Accessible &amp;&amp; !IsDeleted</c> ;
        /// <see langword="false"/> sinon.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code <c>BU_ER_02</c>) si <paramref name="appId"/> est inférieur
        /// ou égal à zéro.
        /// </exception>
        public async Task<bool> HandleAppAccessibilityAsync(
            string caller,
            int appId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAppAccessibilityAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard
                // §4.7 ; §4.15.4 aligné). L'Ex_Business typée remonte intacte au
                // composant appelant : elle est interceptée par catch (Ex_Business)
                // { throw; } avant le catch terminal, sans requalification.
                if (appId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant d'application fourni pour AppList est invalide : {appId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Test d'existence délégué au socle hérité de QH_Generic<AppList>,
                // qui consomme IR_Generic<AppList>.AnyByPredicateAsync : la clause
                // WHERE est générée et exécutée côté base de données par EF Core
                // sous la forme d'un SELECT TOP 1 1 WHERE ..., sans matérialisation
                // d'entité ni AsNoTracking() porté ici (l'accès EF Core est
                // encapsulé dans CR_Generic<T> ; R-4.14.11, R-4.15.12). Le filtre
                // IsDeleted est porté par le prédicat applicatif (troisième
                // asymétrie §4.15.4 : pas de filtrage par défaut sur IsDeleted
                // côté lecture).
                return await HandleAnyByPredicateAsync(
                    callChain,
                    app => app.Id == appId && app.Accessible && !app.IsDeleted,
                    ct);
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