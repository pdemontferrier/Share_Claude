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
    /// UseCase orchestrateur de l'application des droits de pages de l'utilisateur courant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : ce UseCase appartient à la couche applicative (B_UseCases) et réside en
    /// <c>B_UseCases/UseCases/User</c>. Il est résolu par injection de dépendances et
    /// constitue le point unique d'orchestration de la constitution des droits de pages
    /// dans le contexte utilisateur partagé. Il consomme les Query Handlers pour le
    /// chargement du référentiel des pages applicatives et des droits spécifiques, écrit
    /// le contexte utilisateur partagé via <see cref="ISE_User"/> (accès Settings légitime
    /// au niveau UseCase, §4.14.2), et porte le traitement terminal des erreurs.
    /// </para>
    /// <para>
    /// Objectif : garantir que le contexte utilisateur partagé contient un état cohérent
    /// et à jour des droits de pages avant toute interaction avec l'interface. Le scénario
    /// est en lecture seule côté base de données ; son seul effet est l'écriture du
    /// contexte utilisateur partagé, qui ne constitue pas une mutation persistée et
    /// n'ouvre donc pas de transaction (§4.10).
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Valider les préconditions structurelles issues du contexte applicatif.</description></item>
    /// <item><description>Initialiser les droits de pages par défaut au moindre privilège sur l'ensemble des pages applicatives connues, à l'exception des pages système exemptées du contrôle.</description></item>
    /// <item><description>Charger les droits spécifiques de l'utilisateur via le Query Handler.</description></item>
    /// <item><description>Appliquer les droits chargés dans le contexte utilisateur partagé.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/>.</description></item>
    /// <item><description>Exposer un retour signalable booléen (<see langword="true"/> = succès, <see langword="false"/> = échec applicatif capté) destiné à un éventuel UseCase orchestrant amont, conformément à la clause de chaîne d'appel UseCase → UseCase de §4.14.2.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne décide pas du moment d'appel ni de l'identité de l'utilisateur : ceux-ci sont fournis par le contexte applicatif.</description></item>
    /// <item><description>N'ouvre aucune transaction : le scénario ne persiste aucune mutation.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : la lecture passe par les Query Handlers.</description></item>
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
        /// Constitue l'état des droits de pages de l'utilisateur courant dans le contexte
        /// utilisateur partagé, en initialisant les droits par défaut puis en appliquant
        /// les droits spécifiques chargés.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : exécuté après authentification ou lors d'une réinitialisation du
        /// contexte utilisateur. L'identité de l'utilisateur et de l'application est lue
        /// depuis le contexte applicatif courant. Initialise les droits par défaut au
        /// moindre privilège sur l'ensemble des pages applicatives connues, à l'exception
        /// des pages système exemptées du contrôle, charge les droits spécifiques via le
        /// Query Handler, puis applique ces droits dans le contexte utilisateur partagé.
        /// Les exceptions applicatives typées (<see cref="Ex_Business"/>,
        /// <see cref="Ex_Infrastructure"/>, <see cref="Ex_Unclassified"/>) sont captées
        /// terminalement et traitées par <see cref="IU_LogAndNotify"/>, conformément à
        /// §4.7.4, puis signalées à l'appelant par un retour <see langword="false"/> ;
        /// le scénario nominal retourne <see langword="true"/>.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles issues du contexte applicatif.</description></item>
        /// <item><description>Initialiser les droits de pages par défaut au moindre privilège sur l'ensemble des pages applicatives connues, à l'exception des pages système.</description></item>
        /// <item><description>Charger les droits spécifiques et les appliquer au contexte utilisateur partagé.</description></item>
        /// <item><description>Signaler l'issue du scénario par un retour booléen.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si l'application des droits de pages a abouti ;
        /// <see langword="false"/> si une exception applicative typée a été captée et
        /// traitée terminalement.
        /// </returns>
        /// <exception cref="OperationCanceledException">
        /// Propagée à l'appelant lorsque l'annulation coopérative est demandée, conformément à §4.6.
        /// Les exceptions applicatives typées (<see cref="Ex_Business"/>, <see cref="Ex_Infrastructure"/>,
        /// <see cref="Ex_Unclassified"/>) ne sont jamais propagées : elles sont captées et traitées
        /// terminalement par <see cref="IU_LogAndNotify"/>, conformément à §4.7.4.
        /// Les exceptions applicatives typées ne sont jamais propagées : elles sont captées et
        /// signalées par le retour <see langword="false"/>.
        /// </exception>
        public async Task<bool> ExecuteAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                DTO_AppContext appCtx = _appContext.GetAppContext();

                if (appCtx.AppUserId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant utilisateur fourni pour l'application des droits de pages est invalide : {appCtx.AppUserId}. Doit être strictement positif.");

                if (appCtx.AppId <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant application fourni pour l'application des droits de pages est invalide : {appCtx.AppId}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                await InitializeDefaultPageRightsAsync(callChain, ct);

                List<UserAppPageRight> accessiblePages =
                    await _qhUserAppPageRight.HandleGetByUserIdAppIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                if (accessiblePages.Count > 0)
                    ApplyPageRights(callChain, accessiblePages);

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