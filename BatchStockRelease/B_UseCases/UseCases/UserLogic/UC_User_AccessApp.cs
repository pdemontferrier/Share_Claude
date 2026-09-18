using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;

namespace BatchStockRelease.B_UseCases.UseCases.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// UseCase orchestrateur du processus d’accès d’un utilisateur à une application.
    /// Il combine la vérification des droits, la mise à jour du contexte utilisateur,
    /// l’initialisation des droits sur les pages et l’ouverture de la session utilisateur.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce UseCase est exécuté après l’authentification réussie de l’utilisateur.  
    /// Il assure que toutes les informations utilisateur et applicatives nécessaires
    /// sont correctement synchronisées avant de permettre l’accès complet à l’application.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que l’utilisateur autorisé dispose de tous ses droits et d’un contexte complet
    /// avant d’accéder aux pages de l’application.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Vérifier les droits d’accès de l’utilisateur à l’application.</description></item>
    /// <item><description>Mettre à jour le contexte utilisateur (identité, nom complet, droits).</description></item>
    /// <item><description>Initialiser les droits d’accès aux pages selon la configuration en base.</description></item>
    /// <item><description>Ouvrir une session utilisateur pour l’application courante.</description></item>
    /// <item><description>Notifier et tracer toute erreur métier ou infrastructurelle.</description></item>
    /// </list>
    ///
    /// <b>Exceptions :</b>
    /// <list type="bullet">
    /// <item><description><see cref="Ex_Business"/> : en cas de violation d’une règle métier (accès refusé, droits insuffisants).</description></item>
    /// <item><description><see cref="Ex_Infrastructure"/> : en cas de problème d’accès aux ressources (base de données, handlers).</description></item>
    /// <item><description><see cref="Exception"/> : toute autre erreur non prévue, reclassée par <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// </summary>
    public class UC_User_AccessApp : IU_User_AccessApp
    {
        #region === Propriétés privées ===
        private readonly string _callee;
        #endregion

        #region === Dépendances privées ===
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Dictionary _dictionary;

        private readonly IS_UserApplication_CheckAccess _userCheckAccess;
        private readonly IS_User_UpdateContext _userUpdateContext;
        private readonly IS_User_InitializePageAccessRights _userInitializePageAccessRights;
        private readonly IS_UserSession_Open _userSessionOpen;
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise une nouvelle instance du UseCase <see cref="UC_User_AccessApp"/>.
        /// </summary>
        /// <param name="settingsApp">Service de configuration de l’application courante.</param>
        /// <param name="settingsUser">Service de configuration de l’utilisateur courant.</param>
        /// <param name="logAndNotify">Service centralisé de journalisation et de notification.</param>
        /// <param name="dictionary">Service d’accès au dictionnaire multilingue.</param>
        /// <param name="userCheckAccess">Service métier de vérification des droits d’accès utilisateur.</param>
        /// <param name="userUpdateContext">Service métier de mise à jour du contexte utilisateur.</param>
        /// <param name="userInitializePageAccessRights">Service métier d’initialisation des droits sur les pages.</param>
        /// <param name="userSessionOpen">Service métier d’ouverture de session utilisateur.</param>
        public UC_User_AccessApp(
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_LogAndNotify logAndNotify,
            IS_Dictionary dictionary,
            IS_UserApplication_CheckAccess userCheckAccess,
            IS_User_UpdateContext userUpdateContext,
            IS_User_InitializePageAccessRights userInitializePageAccessRights,
            IS_UserSession_Open userSessionOpen)
        {
            _callee = GetType().Name;

            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _logAndNotify = logAndNotify;
            _dictionary = dictionary;

            _userCheckAccess = userCheckAccess;
            _userUpdateContext = userUpdateContext;
            _userInitializePageAccessRights = userInitializePageAccessRights;
            _userSessionOpen = userSessionOpen;
        }
        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Exécute le processus complet d’accès de l’utilisateur à l’application.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Orchestrer les vérifications d’accès, la mise à jour du contexte,
        /// l’initialisation des droits et l’ouverture de session utilisateur.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Vérifier les droits d’accès à l’application.</description></item>
        /// <item><description>Mettre à jour le contexte utilisateur.</description></item>
        /// <item><description>Initialiser les droits sur les pages applicatives.</description></item>
        /// <item><description>Ouvrir la session utilisateur.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns><c>true</c> si l’accès est autorisé et les initialisations réussies, sinon <c>false</c>.</returns>
        public async Task<bool> ExecuteAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            int appId = _settingsApp.GetAppId();
            int userId = _settingsUser.GetAppUserId();
            int appAction = _settingsApp.GetAppAction();

            try
            {
                // Étape 1️ : Vérification des droits d’accès utilisateur
                bool accessGranted = await _userCheckAccess.ExecuteAsync(userId, appId, appAction, callChain);

                if (!accessGranted)
                    return false;

                // Étape 2️ : Mise à jour du contexte utilisateur
                await _userUpdateContext.ExecuteAsync(userId, accessGranted, callChain);

                // Étape 3️ : Initialisation des droits d’accès aux pages
                await _userInitializePageAccessRights.ExecuteAsync(userId, appId, callChain);

                // Étape 4️ : Ouverture de la session utilisateur
                await _userSessionOpen.ExecuteAsync(userId, appId, callChain);

                return true;
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
                return false;
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
                return false;
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
                return false;
            }
        }
        #endregion

        #region === Méthodes privées ===
        // A compléter
        #endregion
    }
}