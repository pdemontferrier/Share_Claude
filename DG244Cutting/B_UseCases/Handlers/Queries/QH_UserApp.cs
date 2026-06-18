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
    /// QueryHandler (QH) dédié à l'entité <see cref="UserApp"/>. Il hérite de
    /// <see cref="QH_Generic{T}"/> (socle de lecture obligatoire) et y ajoute deux
    /// lectures spécialisées par clés fonctionnelles, sans repository spécialisé :
    /// la recherche par login Windows est servie par la méthode héritée
    /// <c>HandleGetFirstOrDefaultAsNoTrackingAsync</c> (variante avec prédicat) ; la
    /// projection FullName par identifiant primaire est servie par la méthode héritée
    /// <c>HandleGetByIdAsNoTrackingAsync</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Consommé en lecture par les ViewModels (chaîne (2)) et par les DataProviders
    /// (chaîne (3)). Aucun appel EF Core n'est porté ici : l'<c>AsNoTracking()</c> et
    /// l'accès au DbContext sont encapsulés dans <c>CR_Generic&lt;UserApp&gt;</c>
    /// (R-4.14.11, R-4.15.12).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une lecture CQRS traçable (CallChain) et robuste (classification
    /// d'exceptions homogène), conforme à §4.14.5 et §4.15.4 du 0230.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>ViewModels (lecture simple) et DataProviders (lecture composée).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Rechercher un utilisateur par login Windows.</description></item>
    /// <item><description>Projeter l'identité d'un utilisateur (FullName) à partir de son identifiant primaire.</description></item>
    /// </list>
    /// </summary>
    public class QH_UserApp : QH_Generic<UserApp>, IQ_UserApp
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
        /// <para>Construit le QueryHandler UserApp.</para>
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
        /// <see cref="UserApp"/> (résolu par le DI vers <c>CR_Generic&lt;UserApp&gt;</c>).
        /// </param>
        /// <param name="classifier">Service de classification des exceptions.</param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="classifier"/> est null.
        /// </exception>
        public QH_UserApp(
            IR_Generic<UserApp> repository,
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
        /// <para>Retourne un utilisateur applicatif à partir de son login Windows.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour identifier l'utilisateur courant sur un poste (device user).</para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une lecture CQRS dédiée, traçable et robuste, sans repository
        /// spécialisé : la recherche est déléguée au socle générique hérité, par
        /// <c>HandleGetFirstOrDefaultAsNoTrackingAsync</c> avec prédicat, dont la
        /// traduction côté SQL combine clause <c>WHERE</c> et limitation à un seul
        /// enregistrement, sans matérialisation de la table complète.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="windowsLogin"/> (dans le try).</description></item>
        /// <item><description>Déléguer la lecture à <c>HandleGetFirstOrDefaultAsNoTrackingAsync</c> (socle hérité, variante avec prédicat, sans tracking).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="windowsLogin">Login Windows à rechercher.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>Entité <see cref="UserApp"/> correspondante, ou null.</returns>
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_01) si <paramref name="windowsLogin"/> est null, vide
        /// ou composé uniquement d'espaces.
        /// </exception>
        public async Task<UserApp?> HandleGetByWindowsLoginAsync(
            string caller,
            string windowsLogin,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByWindowsLoginAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard
                // §4.7 ; §4.15.4 aligné). L'Ex_Business typée remonte intacte au
                // composant appelant : elle est interceptée par catch (Ex_Business)
                // { throw; } avant le catch terminal, sans requalification, et ce
                // indépendamment de sa position. Le placement dans le try garantit
                // en outre qu'une exception imprévue levée pendant l'évaluation de
                // la validation est requalifiée par le classifier via
                // catch (Exception ex).
                if (string.IsNullOrWhiteSpace(windowsLogin))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        "Le login Windows fourni pour la recherche est nul ou vide.");

                ct.ThrowIfCancellationRequested();

                // Délégation au socle hérité de QH_Generic<UserApp> (sous-cas (i) du
                // critère de lecture spécialisée de §4.14.5) : la lecture par clé
                // fonctionnelle est servie par HandleGetFirstOrDefaultAsNoTrackingAsync
                // (variante avec prédicat), dont la traduction EF Core combine clause
                // WHERE et limitation à un enregistrement côté serveur de base de
                // données, sans matérialisation de la table complète ni transformation
                // LINQ-to-Objects côté B_UseCases. Aucun Repository spécialisé, aucun
                // appel EF Core porté par le QH (R-4.14.11, R-4.15.12). AsNoTracking
                // par principe pour une lecture pure (QH-16, R-4.10.10 atténué,
                // §4.15.4 « Lectures avec et sans tracking »). Le filtre IsDeleted est
                // porté côté prédicat applicatif (QH-17, troisième asymétrie assumée
                // du socle de lecture). Le redoublement du segment _callee dans la
                // CallChain propagée au socle hérité ("... > QH_UserApp >
                // HandleGetByWindowsLoginAsync > QH_UserApp >
                // HandleGetFirstOrDefaultAsNoTrackingAsync") est normatif et assumé
                // en délégation intra-classe public→public (§4.15.4, qualification
                // anti-faux-positif).
                return await HandleGetFirstOrDefaultAsNoTrackingAsync(
                    callChain,
                    user => user.WindowsLogin == windowsLogin && !user.IsDeleted,
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
        /// Retourne le nom complet (FullName) d'un utilisateur applicatif à partir de
        /// son identifiant primaire, sous la forme <c>"Prénom Nom"</c>.
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Lecture spécialisée à clé fonctionnelle simple (<see cref="UserApp.Id"/>) avec
        /// projection vers une chaîne unique. La lecture par clé primaire est déléguée
        /// au socle hérité <c>HandleGetByIdAsNoTrackingAsync</c> (sous-cas (i) du critère
        /// de lecture spécialisée de §4.14.5) ; aucun repository spécialisé n'est requis.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une projection traçable et robuste de l'identité d'un utilisateur, avec
        /// garantie de non-nullité et de non-vacuité du résultat. Une valeur de repli
        /// défensive est retournée en cas d'état dégradé non attendu sur
        /// <see cref="UserApp.FirstName"/> ou <see cref="UserApp.LastName"/>, les deux
        /// propriétés étant non-nullables côté schéma.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle sur <paramref name="userId"/> (dans le try).</description></item>
        /// <item><description>Déléguer le chargement à <c>HandleGetByIdAsNoTrackingAsync</c> (socle hérité, sans tracking).</description></item>
        /// <item><description>Rejeter le cas d'utilisateur inexistant ou logiquement supprimé.</description></item>
        /// <item><description>Projeter l'entité vers la concaténation prénom + nom (garde-fou défensif).</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant primaire de l'utilisateur. Doit être strictement positif.</param>
        /// <param name="ct">Token d'annulation.</param>
        /// <returns>
        /// Le nom complet de l'utilisateur au format <c>"Prénom Nom"</c>. Chaîne garantie
        /// non nulle et non vide ; en cas d'état dégradé non attendu où
        /// <see cref="UserApp.FirstName"/> ou <see cref="UserApp.LastName"/> serait blanc,
        /// la valeur de repli <c>"Utilisateur non identifié : {userId}"</c> est retournée.
        /// </returns>
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_02) si <paramref name="userId"/> est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="Ex_Business">
        /// Levée (code BU_ER_03) si aucun utilisateur applicatif utilisable ne correspond
        /// à <paramref name="userId"/> (identifiant inexistant ou entité logiquement supprimée).
        /// </exception>
        public async Task<string> HandleGetFullNameByIdAsync(
            string caller,
            int userId,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFullNameByIdAsync)}";

            try
            {
                // Précondition structurelle validée DANS le bloc try (patron standard
                // §4.7 ; R-4.7.25). Une Ex_Business typée remonte intacte au composant
                // appelant via catch (Ex_Business) { throw; } sans requalification.
                if (userId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni est invalide : {userId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Délégation au socle hérité de QH_Generic<UserApp> (sous-cas (i) du
                // critère de lecture spécialisée de §4.14.5) : la lecture par clé
                // primaire est servie par HandleGetByIdAsNoTrackingAsync, sans
                // Repository spécialisé, sans appel EF Core porté par le QH
                // (R-4.14.11, R-4.15.12). AsNoTracking par principe pour une lecture
                // purement projective : l'entité chargée n'est candidate à aucune
                // mutation par le scénario - elle alimente uniquement la concaténation
                // FullName en sortie (QH-16, R-4.10.10 atténué, §4.15.4 « Lectures
                // avec et sans tracking »). Le redoublement du segment _callee dans
                // la CallChain propagée au socle hérité ("... > QH_UserApp >
                // HandleGetFullNameByIdAsync > QH_UserApp >
                // HandleGetByIdAsNoTrackingAsync") est normatif et assumé en
                // délégation intra-classe public→public (§4.15.4, qualification
                // anti-faux-positif).
                UserApp? user = await HandleGetByIdAsNoTrackingAsync(callChain, userId, ct);

                // Précondition structurelle complémentaire — lecture extensive de
                // BU_ER_03 : l'identifiant est syntaxiquement valide (BU_ER_02
                // satisfait) mais ne correspond à aucune entité utilisable —
                // inexistante ou logiquement supprimée. Aucune décision métier ici :
                // il s'agit de la qualification structurelle de l'utilisabilité de
                // l'entité chargée, équivalente à celle portée par CH_Generic<T>
                // sur l'axe écriture. Le filtre IsDeleted est ainsi porté côté
                // prédicat applicatif post-chargement, conforme à QH-17 / §4.15.4
                // (troisième asymétrie assumée du socle de lecture).
                if (user is null || user.IsDeleted)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"Aucun utilisateur applicatif utilisable pour l'identifiant fourni : {userId}. Id inexistant ou supprimé.");

                // Garde-fou défensif — non fonctionnel attendu en régime nominal :
                // FirstName et LastName sont non-nullables côté schéma EF Core. En
                // cas d'état dégradé technique, on garantit néanmoins la non-nullité
                // et la non-vacuité du résultat contractualisé, plutôt que de
                // retourner une chaîne malformée (" ", "X ", " Y"). Ce garde-fou
                // n'est jamais censé se déclencher en exploitation.
                if (string.IsNullOrWhiteSpace(user.FirstName) || string.IsNullOrWhiteSpace(user.LastName))
                    return $"Utilisateur non identifié : {userId}";

                // Cas nominal — projection FullName effectuée en B_UseCases
                // (LINQ-to-Objects, autorisée pour une réduction post-chargement
                // §4.14.6 ; ne mobilise aucune API EF Core).
                return $"{user.FirstName} {user.LastName}";
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