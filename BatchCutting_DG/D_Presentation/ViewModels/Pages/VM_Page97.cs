using System.Collections.ObjectModel;
using System.Windows.Input;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page97 : VM_Page_Generic
    {
        private readonly IS_Dictionary _dictionary;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;
        private readonly IS_DataBase _database;
        private readonly IS_UserSessionsAdmin _userSessionAdmin;
        private readonly IQ_UserSessionDetails _usdQuery;

        private string _appAccess;
        public string AppAccess
        {
            get => _appAccess;
            set
            {
                _appAccess = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<DTO_UserSessionDetails> _userSession;
        public ObservableCollection<DTO_UserSessionDetails> UserSession
        {
            get => _userSession;
            set
            {
                _userSession = value;
                OnPropertyChanged();
            }
        }

        private DTO_UserSessionDetails _selectedUserSession;
        public DTO_UserSessionDetails SelectedUserSession
        {
            get => _selectedUserSession;
            set
            {
                _selectedUserSession = value;
                OnPropertyChanged();

                // Mettre à jour UserSettings
                if (value != null)
                {
                    _settings.SetSelectedSessionId(value.Id);
                    _settings.SetSelectedSessionFullName(value.FullnameUser ?? string.Empty);
                }
            }
        }

        public VM_Page97(IS_Dictionary dictionary, IS_Settings settings,
                               IS_Notification notification, IS_UserSessionsAdmin userSessionAdmin,
                               IS_DataBase database, IQ_UserSessionDetails usdQuery)
        {
            _dictionary = dictionary;
            _settings = settings;
            _notification = notification;
            _userSessionAdmin = userSessionAdmin;
            _database = database;
            _usdQuery = usdQuery;
            _userSession = new ObservableCollection<DTO_UserSessionDetails>();
            _selectedUserSession = new DTO_UserSessionDetails();
            _appAccess = string.Empty;
            LoadData();
        }

        private async void LoadData()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var userSessionList = await _usdQuery.HandleAsync(_settings.GetAppID());
            UserSession = new ObservableCollection<DTO_UserSessionDetails>(userSessionList);

            // Tester l'accessibilité de l'application
            bool appAccessible = await _userSessionAdmin.CheckAppAccessibleAsync();

            // Mettre à jour le libéllé du bouton d'accessibilité de l'application
            AppAccess = appAccessible ? _dictionary.GetText("P97_05") : _dictionary.GetText("P97_06");
        }


        public ICommand UpdateAppAccessCommand => new UT_RelayCommandArg0Async(UpdateAppAccessAsync);

        private async Task UpdateAppAccessAsync()
        {
            try
            {
                // Tester l'accessibilité de l'application
                bool appAccessible = await _userSessionAdmin.CheckAppAccessibleAsync();

                // Faire les mise à jour
                await _database.SprApplicationAccessUpdateAsync(_settings.GetAppID());
                AppAccess = appAccessible ? _dictionary.GetText("P97_06") : _dictionary.GetText("P97_05");
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_11", ex.Message);
            }
        }

        public ICommand DisconnectUserCommand => new UT_RelayCommandArg0Async(DisconnectUserAsync);

        private async Task DisconnectUserAsync()
        {
            try
            {
                if (SelectedUserSession.IdUser > 0)
                {
                    await _userSessionAdmin.IssueCloseSessionCommandAsync(SelectedUserSession.IdUser);
                }
                else
                {
                    _notification.Warning("No_Wa_09");
                }

            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_11", ex.Message);
            }
        }

        public ICommand DisconnectAllUsersCommand => new UT_RelayCommandArg0Async(DisconnectAllUsersAsync);

        private async Task DisconnectAllUsersAsync()
        {
            try
            {
                if (UserSession.Any())
                {
                    foreach (var userSession in UserSession)
                    {
                        await _userSessionAdmin.IssueCloseSessionCommandAsync(userSession.IdUser);
                    }
                }
                else
                {
                    _notification.Warning("No_Wa_09");
                }
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_12", ex.Message);
            }
        }
    }
}