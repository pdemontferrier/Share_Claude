using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;

namespace BatchStockRelease.B_UseCases.UseCases.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// UseCase responsable de l'authentification complète de l'utilisateur
    /// lors du chargement initial de l'application. Il orchestre :
    /// <list type="number">
    /// <item>l'identification automatique via le compte Windows,</item>
    /// <item>la vérification des droits d'accès à l'application,</item>
    /// <item>et la restitution d'un statut d'autorisation global.</item>
    /// </list>
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce UseCase est appelé depuis <c>MainWindow</c> au démarrage de l'application
    /// pour initialiser la session utilisateur et diriger la navigation vers la page
    /// appropriée (Page00 ou Page10).
    /// </para>
    /// </summary>
    public class UC_User_Authentification : IU_User_Authentification
    {
        #region === Propriétés privées ===
        /// <summary>
        /// Nom du UseCase pour la traçabilité.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances injectées ===

        private readonly IS_Settings_User _settingsUser;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Notification _notification;
        private readonly IS_Navigation _navigation;
        private readonly IS_User_CheckAppDeviceUser _user_CheckAppDeviceUser;
        private readonly IU_User_AccessApp _userAccessApp;

        #endregion

        #region === Constructeur ===

        public UC_User_Authentification(
            IS_Settings_User settingsUser,
            IS_LogAndNotify logAndNotify,
            IS_Notification notification,
            IS_Navigation navigation,
            IS_User_CheckAppDeviceUser user_CheckAppDeviceUser,
            IU_User_AccessApp userAccessApp)
        {
            _callee = GetType().Name;

            _settingsUser = settingsUser;
            _logAndNotify = logAndNotify;
            _notification = notification;
            _navigation = navigation;
            _user_CheckAppDeviceUser = user_CheckAppDeviceUser;
            _userAccessApp = userAccessApp;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Exécute le processus complet d'authentification utilisateur.
        /// </summary>
        /// <param name="caller">Chaîne d'appel d'origine pour la traçabilité.</param>
        public async Task ExecuteAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : Identification automatique via le compte Windows
                await IdentifyUserFromDeviceAsync(callChain);

                // Étape 2 : Aucun utilisateur identifié → redirection Page00
                if (_settingsUser.GetAppUserId() == 0)
                {
                    _navigation.NavigateToNewPage("Page00");
                    return;
                }

                // Étape 3 : Vérification des droits d’accès utilisateur
                bool accessGranted = await _userAccessApp.ExecuteAsync(callChain);

                // Étape 4 : Navigation selon le résultat
                if (accessGranted)
                {
                    _navigation.NavigateToNewPage("Page10");
                }
                else
                {
                    _notification.Warning("No_Wa_04");
                    _navigation.NavigateToNewPage("Page00");
                }
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
                _navigation.NavigateToNewPage("Page00");
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
                _navigation.NavigateToNewPage("Page00");
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
                _navigation.NavigateToNewPage("Page00");
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Identifie l’utilisateur via le compte Windows si aucun utilisateur n’est déjà enregistré.
        /// </summary>
        private async Task IdentifyUserFromDeviceAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(IdentifyUserFromDeviceAsync)}";

            if (_settingsUser.GetAppUserId() != 0)
                return; // L’utilisateur est déjà connu

            string deviceUser = _settingsUser.GetAppDeviceUser();

            if (string.IsNullOrWhiteSpace(deviceUser))
                return; // Aucun identifiant Windows disponible

            try
            {
                int userId = await _user_CheckAppDeviceUser.ExecuteAsync(deviceUser, callChain);

                if (userId > 0)
                    _settingsUser.SetAppUserId(userId);
            }
            catch (Exception ex)
            {
                // Classification centralisée des exceptions
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion
    }
}