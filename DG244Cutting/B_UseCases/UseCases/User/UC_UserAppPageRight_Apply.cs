using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
using DG244Cutting.A_Domain.Interfaces.Settings.Presentation;
using DG244Cutting.A_Domain.Interfaces.Settings.User;
using DG244Cutting.A_Domain.Interfaces.UseCases.App;
using DG244Cutting.A_Domain.Interfaces.UseCases.User;

namespace DG244Cutting.B_UseCases.UseCases.User
{
    /// <summary>
    /// UseCase orchestrateur de l'initialisation systématique des droits de pages au
    /// moindre privilège et de l'application conditionnelle des droits de l'utilisateur
    /// courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>, conformément à la cellule UC_ ↔ B_UseCases/UseCases/[Domaine]
    /// du tableau famille × couche de §2.8 du 0230 et à la note [b] du même tableau
    /// restreignant le sous-dossier [Domaine] à <c>{App, Business, User}</c>. Il est résolu
    /// par injection de dépendances et constitue le point unique d'orchestration de la
    /// constitution des droits de pages dans le contexte utilisateur partagé. Il consomme
    /// les Query Handlers pour le chargement du référentiel des pages applicatives et des
    /// droits spécifiques, écrit le contexte utilisateur partagé via <see cref="ISE_User"/>
    /// (accès Settings légitime au niveau UseCase au titre de la neuvième obligation
    /// contractuelle de §4.14.2 amendée), et porte le traitement terminal des erreurs.
    /// </para>
    /// <para>
    /// Objectif : garantir que le contexte utilisateur partagé contient un état cohérent
    /// des droits de pages avant toute interaction avec l'interface, selon une logique
    /// fonctionnelle en deux temps - initialisation systématique au moindre privilège
    /// indépendamment de la présence d'un utilisateur identifié dans le contexte applicatif,
    /// puis application conditionnelle des droits spécifiques de l'utilisateur courant
    /// lorsqu'un identifiant utilisateur strictement positif est présent dans le contexte
    /// applicatif. Le scénario est en lecture seule côté base de données ; son seul effet
    /// est l'écriture du contexte utilisateur partagé, qui ne constitue pas une mutation
    /// persistée et n'ouvre donc pas de transaction (§4.10).
    /// </para>
    /// <para>
    /// Chaîne d'appel : le UseCase est conçu pour être consommé en sous-séquence par un
    /// UseCase orchestrant amont au sens de la clause de chaîne d'appel UseCase → UseCase
    /// de §4.14.2 amendée indexée par R-4.14.21 (orchestrateur amont prévu :
    /// <c>UC_Application_OnStart</c> dans le cadre de la séquence de démarrage applicatif
    /// posée en §3.10 du 0230). La signature signalable <c>Task&lt;bool&gt;</c> de la
    /// méthode publique permet à cet orchestrant amont de constater l'issue du
    /// sous-scénario sans propagation d'exception applicative typée.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider la précondition structurelle d'identifiant application strictement positif issue du contexte applicatif.</description></item>
    /// <item><description>Initialiser systématiquement les droits de pages par défaut au moindre privilège sur l'ensemble des pages applicatives connues, à l'exception des pages système exemptées du contrôle, indépendamment de la présence d'un utilisateur identifié.</description></item>
    /// <item><description>Charger conditionnellement les droits spécifiques de l'utilisateur via le Query Handler lorsqu'un identifiant utilisateur strictement positif est présent dans le contexte applicatif.</description></item>
    /// <item><description>Appliquer conditionnellement les droits chargés dans le contexte utilisateur partagé.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/> conformément à §4.7.4 du 0230.</description></item>
    /// <item><description>Exposer un retour signalable booléen (<see langword="true"/> = succès, <see langword="false"/> = échec applicatif capté) destiné au UseCase orchestrant amont, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2 amendée indexée par R-4.14.21.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne décide pas du moment d'appel : celui-ci est fixé par l'orchestrateur amont (séquence de démarrage applicatif, pipeline d'authentification ou réinitialisation du contexte).</description></item>
    /// <item><description>Ne décide pas de la présence ou non d'un utilisateur identifié : la conditionnalité d'application des droits utilisateur est portée par la lecture du contexte applicatif courant.</description></item>
    /// <item><description>N'ouvre aucune transaction : le scénario ne persiste aucune mutation (invariant 6 + R-4.10.1).</description></item>
    /// <item><description>N'appelle jamais directement un Repository : la lecture passe par les Query Handlers (I-4.14.4 amendée, I-4.14.6, I-4.14.9).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppPageRight_Apply"/>
    public class UC_UserAppPageRight_Apply : IU_UserAppPageRight_Apply
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserAppPageRight _qhUserAppPageRight;
        private readonly IQ_Generic<UserAppPage> _qhUserAppPage;
        private readonly IS_AppContext _appContext;
        private readonly ISE_User _seUser;
        private readonly ISE_Navigation _seNavigation;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppPageRight_Apply"/> avec ses dépendances.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cette implémentation consomme deux dépendances de référentiel substituant les
        /// constantes locales obsolètes d'une précédente implémentation : <c>IQ_Generic</c>
        /// paramétré par <see cref="UserAppPage"/>, qui énumère dynamiquement les pages
        /// applicatives éligibles à des droits depuis le référentiel <c>UserAppPage</c> ;
        /// et <see cref="ISE_Navigation"/>, dont les propriétés
        /// <see cref="ISE_Navigation.LoginPageName"/> et <see cref="ISE_Navigation.FallbackPageName"/>
        /// identifient les pages système exemptées du contrôle d'accès.
        /// </para>
        /// </remarks>
        /// <param name="qhUserAppPageRight">Query Handler de lecture des droits de pages utilisateur.</param>
        /// <param name="qhUserAppPage">Query Handler générique paramétré par <see cref="UserAppPage"/>, consommé pour énumérer les pages applicatives éligibles à des droits depuis le référentiel.</param>
        /// <param name="appContext">Service fournissant le contexte applicatif courant.</param>
        /// <param name="seUser">Setting utilisateur courant, cible de l'application des droits de pages.</param>
        /// <param name="seNavigation">Setting de navigation exposant les noms logiques des pages système exemptées du contrôle des droits (page de connexion et page de repli).</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppPageRight_Apply(
            IQ_UserAppPageRight qhUserAppPageRight,
            IQ_Generic<UserAppPage> qhUserAppPage,
            IS_AppContext appContext,
            ISE_User seUser,
            ISE_Navigation seNavigation,
            IU_LogAndNotify logAndNotify)
        {
            _qhUserAppPageRight = qhUserAppPageRight ?? throw new ArgumentNullException(nameof(qhUserAppPageRight));
            _qhUserAppPage = qhUserAppPage ?? throw new ArgumentNullException(nameof(qhUserAppPage));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
            _seNavigation = seNavigation ?? throw new ArgumentNullException(nameof(seNavigation));
            _logAndNotify = logAndNotify ?? throw new ArgumentNullException(nameof(logAndNotify));
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Initialise systématiquement les droits de pages au moindre privilège dans le
        /// contexte utilisateur partagé, puis applique conditionnellement les droits
        /// spécifiques de l'utilisateur courant lorsqu'un utilisateur est identifié dans
        /// le contexte applicatif.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : invocable au démarrage applicatif avant toute authentification, ainsi
        /// qu'après authentification ou lors d'une réinitialisation du contexte utilisateur.
        /// L'identité de l'application et, le cas échéant, de l'utilisateur sont lues depuis
        /// le contexte applicatif courant via <see cref="IS_AppContext.GetAppContext"/>.
        /// </para>
        /// <para>
        /// Sémantique fonctionnelle en deux temps : (1) initialisation systématique au
        /// moindre privilège - après validation de la précondition structurelle
        /// d'identifiant application strictement positif, le UseCase invoque
        /// <see cref="InitializeDefaultPageRightsAsync"/> de manière inconditionnelle pour
        /// initialiser les droits par défaut sur l'ensemble des pages applicatives connues,
        /// à l'exception des pages système exemptées du contrôle, indépendamment de la
        /// présence d'un utilisateur identifié ; (2) application conditionnelle des droits
        /// utilisateur - lorsque le contexte applicatif fournit un identifiant utilisateur
        /// strictement positif (<c>AppUserId &gt; 0</c>), le UseCase charge les droits
        /// spécifiques via <see cref="IQ_UserAppPageRight.HandleGetByUserIdAppIdAsync"/>
        /// et invoque <see cref="ApplyPageRights"/> lorsque le jeu retourné est non vide.
        /// En l'absence d'un identifiant utilisateur strictement positif, le contexte
        /// utilisateur partagé est laissé en état initialisé au moindre privilège.
        /// </para>
        /// <para>
        /// Sémantique du retour : le scénario nominal retourne <see langword="true"/> et
        /// couvre les deux variantes - avec utilisateur identifié, l'initialisation par
        /// défaut a été effectuée puis les droits utilisateur ont été chargés et appliqués
        /// (le maintien des droits par défaut en cas de jeu vide retourné par le Query
        /// Handler est une variante admise de cette branche) ; sans utilisateur identifié,
        /// seule l'initialisation par défaut a été effectuée. Le retour
        /// <see langword="false"/> signale qu'une exception applicative typée a été captée
        /// terminalement et traitée par <see cref="IU_LogAndNotify"/> conformément à §4.7.4
        /// du 0230, et couvre quatre cas - précondition structurelle <c>AppId &lt;= 0</c>
        /// (<see cref="Ex_Business"/> code <see cref="Ex_Business.ErrorCodes.BU_ER_02"/>),
        /// absence de page applicative non supprimée dans le référentiel <c>UserAppPage</c>
        /// empêchant l'initialisation par défaut (<see cref="Ex_Business"/> code
        /// <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> levée par
        /// <see cref="InitializeDefaultPageRightsAsync"/> et captée terminalement par la
        /// présente méthode), défaillance d'infrastructure remontée par les composants aval
        /// (<see cref="Ex_Infrastructure"/>), défaillance applicative non classifiée
        /// (<see cref="Ex_Unclassified"/>). L'annulation coopérative
        /// (<see cref="OperationCanceledException"/>) est propagée à l'appelant sans
        /// signalisation booléenne, conformément au mécanisme normatif de §4.6 du 0230.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider la précondition structurelle d'identifiant application strictement positif.</description></item>
        /// <item><description>Initialiser systématiquement les droits de pages par défaut au moindre privilège.</description></item>
        /// <item><description>Charger et appliquer conditionnellement les droits spécifiques de l'utilisateur courant.</description></item>
        /// <item><description>Signaler l'issue du scénario par un retour booléen destiné à un orchestrant amont.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si le scénario a abouti à son objectif - avec utilisateur
        /// identifié, initialisation par défaut effectuée puis chargement et application
        /// des droits utilisateur ; sans utilisateur identifié, seule l'initialisation par
        /// défaut effectuée. <see langword="false"/> si une exception applicative typée
        /// (<see cref="Ex_Business"/> code <c>BU_ER_02</c> ou <c>BU_ER_04</c>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) a été captée
        /// et traitée terminalement par <see cref="IU_LogAndNotify"/>.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément
        /// à §4.6 du 0230. Les exceptions applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) ne sont jamais
        /// propagées : elles sont captées et signalées par le retour <see langword="false"/>.
        /// </exception>
        public async Task<bool> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";
            try
            {
                DTO_AppContext appCtx = _appContext.GetAppContext();

                if (appCtx.AppId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant application fourni pour l'application des droits de pages est invalide : {appCtx.AppId}. Doit être strictement positif.");
                
                ct.ThrowIfCancellationRequested();

                await InitializeDefaultPageRightsAsync(callChain, ct);

                if (appCtx.AppUserId > 0)
                {
                    List<UserAppPageRight> accessiblePages =
                        await _qhUserAppPageRight.HandleGetByUserIdAppIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                    if (accessiblePages.Count > 0)
                        ApplyPageRights(callChain, accessiblePages);
                }

                return true;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
                return false;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
                return false;
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
                return false;
            }
            catch (OperationCanceledException)
            {
                // Annulation coopérative : remontée sans traitement.
                // L'annulation n'est pas une erreur ; aucun appel à IU_LogAndNotify.
                // Cf. section 4.6 pour la mécanique d'annulation coopérative.
                throw;
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Initialise les droits de pages par défaut dans le contexte utilisateur partagé,
        /// selon le principe du moindre privilège.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée en première étape de <see cref="ExecuteAsync"/>, après les
        /// préconditions structurelles. Charge l'ensemble des pages applicatives non
        /// supprimées logiquement depuis le référentiel <c>UserAppPage</c> via
        /// <see cref="IQ_Generic{T}.HandleGetFilteredAsNoTrackingAsync"/>, puis positionne
        /// tous les droits à <see langword="false"/>, à l'exception des deux pages système
        /// identifiées par <see cref="ISE_Navigation.LoginPageName"/> et
        /// <see cref="ISE_Navigation.FallbackPageName"/>, exemptées du contrôle d'accès
        /// par construction et toujours accessibles.
        /// </para>
        /// <para>
        /// La levée d'<see cref="Ex_Business"/> avec le code
        /// <see cref="Ex_Business.ErrorCodes.BU_ER_04"/> en cas de retour vide du Query
        /// Handler est strictement postérieure à la réussite technique de l'appel à
        /// <see cref="IQ_Generic{T}"/> : elle exprime une précondition métier (le
        /// référentiel <c>UserAppPage</c> ne peut être vide pour que le scénario soit
        /// applicable), distincte d'une défaillance technique d'accès aux données.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de la méthode publique appelante.</param>
        /// <param name="ct">Jeton d'annulation coopérative propagé depuis la méthode publique appelante.</param>
        /// <exception cref="Ex_Business">
        /// Levée lorsque le référentiel <c>UserAppPage</c> ne contient aucune page applicative
        /// non supprimée, état applicatif anormal empêchant l'initialisation des droits par défaut.
        /// Captée terminalement par le catch typé <c>Ex_Business</c> de <see cref="ExecuteAsync"/>.
        /// </exception>
        private async Task InitializeDefaultPageRightsAsync(string caller, CancellationToken ct)
        {
            string callChain = $"{caller} > {nameof(InitializeDefaultPageRightsAsync)}";

            List<UserAppPage> pages =
                await _qhUserAppPage.HandleGetFilteredAsNoTrackingAsync(callChain, p => !p.IsDeleted, ct);

            if (pages.Count == 0)
                throw new Ex_Business(
                    callChain,
                    Ex_Business.ErrorCodes.BU_ER_04,
                    "Le référentiel des pages applicatives (UserAppPage) ne contient aucune page non supprimée ; impossible d'initialiser les droits de pages par défaut.");

            string loginPageName = _seNavigation.LoginPageName;
            string fallbackPageName = _seNavigation.FallbackPageName;

            _seUser.ClearPageAccessRights();

            foreach (UserAppPage page in pages)
            {
                bool canAccess = page.PageCode == loginPageName || page.PageCode == fallbackPageName;

                _seUser.SetPageAccessRight(
                    page.PageCode,
                    new PageRights
                    {
                        CanAccess = canAccess
                    });
            }
        }

        /// <summary>
        /// Applique un jeu de droits de pages chargé dans le contexte utilisateur partagé.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée en dernière étape de <see cref="ExecuteAsync"/>, après
        /// chargement des droits depuis la base de données. Met à jour uniquement les
        /// pages pour lesquelles des droits spécifiques ont été chargés.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de la méthode publique appelante.</param>
        /// <param name="pageAccessRights">Collection de droits par page, issue de la base de données.</param>
        private void ApplyPageRights(string caller, IEnumerable<UserAppPageRight> pageAccessRights)
        {
            string callChain = $"{caller} > {nameof(ApplyPageRights)}";

            foreach (UserAppPageRight pageAccess in pageAccessRights)
            {
                _seUser.SetPageAccessRight(
                    pageAccess.PageCode,
                    new PageRights
                    {
                        CanAccess = pageAccess.CanAccess,
                        CanCreate = pageAccess.CanCreate,
                        CanRead = pageAccess.CanRead,
                        CanUpdate = pageAccess.CanUpdate,
                        CanDelete = pageAccess.CanDelete,
                        CanControl = pageAccess.CanControl,
                        CanValidate = pageAccess.CanValidate,
                        CanSupervise = pageAccess.CanSupervise,
                        CanMonitor = pageAccess.CanMonitor,
                        CanAdmin = pageAccess.CanAdmin
                    });
            }
        }

        #endregion
    }
}