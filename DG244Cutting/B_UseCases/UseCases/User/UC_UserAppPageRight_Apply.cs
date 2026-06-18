using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.DTOs.App;
using DG244Cutting.A_Domain.Entities.App;
using DG244Cutting.A_Domain.Entities.DIGIT_TRY;
using DG244Cutting.A_Domain.Interfaces.Handlers.Queries;
using DG244Cutting.A_Domain.Interfaces.Services.App;
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
    /// dans le contexte utilisateur partagé. Il consomme le Query Handler pour le
    /// chargement des droits, écrit le contexte utilisateur partagé via <see cref="ISE_User"/>
    /// (accès Settings légitime au niveau UseCase, §4.14.2), et porte le traitement
    /// terminal des erreurs.
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
    /// <item><description>Initialiser les droits de pages par défaut au moindre privilège.</description></item>
    /// <item><description>Charger les droits spécifiques de l'utilisateur via le Query Handler.</description></item>
    /// <item><description>Appliquer les droits chargés dans le contexte utilisateur partagé.</description></item>
    /// <item><description>Déléguer le traitement terminal des erreurs à <see cref="IU_LogAndNotify"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne décide pas du moment d'appel ni de l'identité de l'utilisateur : ceux-ci sont fournis par le contexte applicatif.</description></item>
    /// <item><description>N'ouvre aucune transaction : le scénario ne persiste aucune mutation.</description></item>
    /// <item><description>N'appelle jamais directement un Repository : la lecture passe par le Query Handler.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="IU_UserAppPageRight_Apply"/>
    public class UC_UserAppPageRight_Apply : IU_UserAppPageRight_Apply
    {
        #region === Propriétés privées ===

        private const int FirstPageIndex = 0;
        private const int LastPageIndex = 99;
        private const string SystemPageEntry = "Page00";
        private const string SystemPageExit = "Page99";

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IQ_UserAppPageRight _qhUserAppPageRight;
        private readonly IS_AppContext _appContext;
        private readonly ISE_User _seUser;
        private readonly IU_LogAndNotify _logAndNotify;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="UC_UserAppPageRight_Apply"/> avec ses dépendances.
        /// </summary>
        /// <param name="qhUserAppPageRight">Query Handler de lecture des droits de pages utilisateur.</param>
        /// <param name="appContext">Service fournissant le contexte applicatif courant.</param>
        /// <param name="seUser">Setting utilisateur courant, cible de l'application des droits de pages.</param>
        /// <param name="logAndNotify">Pipeline terminal de journalisation et de notification des erreurs.</param>
        /// <exception cref="ArgumentNullException">Levée si l'un des paramètres est <see langword="null"/>.</exception>
        public UC_UserAppPageRight_Apply(
            IQ_UserAppPageRight qhUserAppPageRight,
            IS_AppContext appContext,
            ISE_User seUser,
            IU_LogAndNotify logAndNotify)
        {
            _qhUserAppPageRight = qhUserAppPageRight ?? throw new ArgumentNullException(nameof(qhUserAppPageRight));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _seUser = seUser ?? throw new ArgumentNullException(nameof(seUser));
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
        /// moindre privilège, charge les droits spécifiques via le Query Handler, puis
        /// applique ces droits dans le contexte utilisateur partagé. Toute exception typée
        /// est traitée par le pipeline terminal.
        /// </para>
        /// <para>Responsabilités :</para>
        /// <list type="bullet">
        /// <item><description>Valider les préconditions structurelles issues du contexte applicatif.</description></item>
        /// <item><description>Initialiser les droits de pages par défaut au moindre privilège.</description></item>
        /// <item><description>Charger les droits spécifiques et les appliquer au contexte utilisateur partagé.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de l'appelant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant utilisateur ou l'identifiant application lus du contexte
        /// applicatif courant ne sont pas strictement positifs.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée si le chargement des droits en base échoue.</exception>
        public async Task ExecuteAsync(string caller, CancellationToken ct = default)
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

                InitializeDefaultPageRights(callChain);

                List<UserAppPageRight> accessiblePages =
                    await _qhUserAppPageRight.HandleGetByUserIdAppIdAsync(callChain, appCtx.AppUserId, appCtx.AppId, ct);

                if (accessiblePages.Count > 0)
                    ApplyPageRights(callChain, accessiblePages);
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_01", ex, ct: ct);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_02", ex, ct: ct);
            }
            catch (Ex_Unclassified ex)
            {
                await _logAndNotify.ExecuteAsync(callChain, "No_EC_03", ex, ct: ct);
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
        /// Contexte : appelée en première étape de <see cref="ExecuteAsync"/>, avant tout
        /// chargement depuis la base de données. Positionne tous les droits à
        /// <see langword="false"/>, à l'exception des pages système d'entrée et de sortie
        /// toujours accessibles.
        /// </para>
        /// </remarks>
        /// <param name="caller">Chaîne d'appel reçue de la méthode publique appelante.</param>
        private void InitializeDefaultPageRights(string caller)
        {
            string callChain = $"{caller} > {nameof(InitializeDefaultPageRights)}";

            _seUser.ClearPageAccessRights();

            for (int index = FirstPageIndex; index <= LastPageIndex; index++)
            {
                string pageCode = $"Page{index:00}";
                bool canAccess = pageCode is SystemPageEntry or SystemPageExit;

                _seUser.SetPageAccessRight(
                    pageCode,
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