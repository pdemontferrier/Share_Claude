using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page97 — Page d’administration des sessions utilisateurs.
    ///
    /// <para><b>Contexte :</b> Cette page permet aux administrateurs
    /// de superviser les connexions en cours et de gérer la disponibilité
    /// de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Offrir trois fonctions principales :</para>
    /// <list type="bullet">
    ///   <item><description><b>Verrouillage de l’application :</b> rend temporairement l’application inaccessible.</description></item>
    ///   <item><description><b>Déconnexion ciblée :</b> force la déconnexion d’un utilisateur sélectionné.</description></item>
    ///   <item><description><b>Déconnexion globale :</b> force la déconnexion de tous les utilisateurs connectés.</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page97.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Utilise les services <see cref="IS_UserSession_Admin"/>, <see cref="IQ_UserSessionDetails"/>, <see cref="IS_StoreProcedure"/> et <see cref="IS_Notification"/>.  
    /// - Les exceptions sont classifiées via <see cref="Ex_Classifier"/> et enregistrées avec <see cref="IS_LogAndNotify"/>.  
    /// - Le bouton principal <see cref="AppAccess"/> affiche dynamiquement l’état d’accessibilité de l’application.</para>
    /// </summary>
    public class VM_Page97 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IQ_UserSessionDetails _usdQuery;
        private readonly IS_StoreProcedure _database;
        private readonly IS_UserSession_Admin _userSessionAdmin;
        private readonly IS_Notification _notification;
        private readonly IS_ApplicationAvailability _applicationAvailability;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;

        #endregion

        #region === Commandes ===

        /// <summary>Commande pour basculer l’accessibilité de l’application.</summary>
        public ICommand UpdateAppAccessCommand => new UT_RelayCommandArg0Async(UpdateAppAccessAsync);

        /// <summary>Commande pour déconnecter un utilisateur sélectionné.</summary>
        public ICommand DisconnectUserCommand => new UT_RelayCommandArg0Async(DisconnectUserAsync);

        /// <summary>Commande pour déconnecter tous les utilisateurs.</summary>
        public ICommand DisconnectAllUsersCommand => new UT_RelayCommandArg0Async(DisconnectAllUsersAsync);

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page97 avec les services nécessaires.
        /// </summary>
        public VM_Page97(
            IQ_UserSessionDetails usdQuery,
            IS_UserSession_Admin userSessionAdmin,
            IS_ApplicationAvailability applicationAvailability,
            IS_Notification notification,
            IS_StoreProcedure database,
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify,
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser)
            : base(settingsUseCase, navigation, dictionary, logAndNotify)
        {
            _usdQuery = usdQuery;
            _userSessionAdmin = userSessionAdmin;
            _applicationAvailability = applicationAvailability;
            _notification = notification;
            _database = database;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private string _appAccess = string.Empty;
        /// <summary>
        /// Libellé du bouton indiquant l’état d’accessibilité de l’application.
        /// </summary>
        public string AppAccess
        {
            get => _appAccess;
            set => SetProperty(ref _appAccess, value);
        }

        private ObservableCollection<DTO_UserSessionDetails> _userSession = new();
        /// <summary>
        /// Liste des sessions utilisateurs actuellement actives dans le système.
        /// </summary>
        public ObservableCollection<DTO_UserSessionDetails> UserSession
        {
            get => _userSession;
            set => SetProperty(ref _userSession, value);
        }

        private DTO_UserSessionDetails _selectedUserSession = new();
        /// <summary>
        /// Session utilisateur sélectionnée par l’administrateur.
        /// </summary>
        public DTO_UserSessionDetails SelectedUserSession
        {
            get => _selectedUserSession;
            set
            {
                if (SetProperty(ref _selectedUserSession, value) && value != null)
                {
                    _settingsUser.SetSelectedSessionId(value.Id);
                    _settingsUser.SetSelectedSessionFullName(value.FullnameUser ?? string.Empty);
                }
            }
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge la liste des sessions actives et met à jour le statut d’accessibilité de l’application.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));
            int appid = _settingsApp.GetAppId();

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    var userSessionList = await _usdQuery.HandleAsync(appid);
                    UserSession = new ObservableCollection<DTO_UserSessionDetails>(userSessionList);

                    bool appAccessible = await _applicationAvailability.IsAppAccessibleAsync(appid, callChain);
                    AppAccess = appAccessible ? _dictionary.GetText("P97_05") : _dictionary.GetText("P97_06");
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Bascule l’état d’accessibilité de l’application (verrouillage / déverrouillage).
        /// </summary>
        private async Task UpdateAppAccessAsync()
        {
            string callChain = BuildFirstCallChain(nameof(UpdateAppAccessAsync));
            int appid = _settingsApp.GetAppId();

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    bool appAccessible = await _applicationAvailability.IsAppAccessibleAsync(appid, callChain);
                    await _database.SprApplicationAccessUpdateAsync(_settingsApp.GetAppId());

                    // Inverser l’état affiché après la mise à jour
                    AppAccess = appAccessible ? _dictionary.GetText("P97_06") : _dictionary.GetText("P97_05");
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _notification.Error("No_EC_11", $"VM_P97_ER_01 - {ex.Message}");
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        /// <summary>
        /// Force la déconnexion d’un utilisateur sélectionné dans la liste.
        /// </summary>
        private async Task DisconnectUserAsync()
        {
            string callChain = BuildFirstCallChain(nameof(DisconnectUserAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    if (SelectedUserSession.IdUser > 0)
                        await _userSessionAdmin.IssueCloseSessionCommandAsync(SelectedUserSession.IdUser);
                    else
                        _notification.Warning("No_Wa_09");
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _notification.Error("No_EC_11", $"VM_P97_ER_02 - {ex.Message}");
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        /// <summary>
        /// Force la déconnexion de tous les utilisateurs connectés.
        /// </summary>
        private async Task DisconnectAllUsersAsync()
        {
            string callChain = BuildFirstCallChain(nameof(DisconnectAllUsersAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    if (UserSession.Any())
                    {
                        foreach (var userSession in UserSession)
                            await _userSessionAdmin.IssueCloseSessionCommandAsync(userSession.IdUser);
                    }
                    else
                    {
                        _notification.Warning("No_Wa_09");
                    }
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    _notification.Error("No_EC_12", $"VM_P97_ER_03 - {ex.Message}");
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        #endregion
    }
}