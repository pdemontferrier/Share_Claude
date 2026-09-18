using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BatchCutting_DG.A_Domain.AppEntities;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page90 : VM_Page_Generic
    {
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;
        private readonly IQ_User _userQuery;
        private readonly IQ_UserAppPageDroit _upaQuery;
        public ObservableCollection<UserAppPageDroit> PagesUserRightsList { get; private set; }
            = new ObservableCollection<UserAppPageDroit>();

        public ObservableCollection<KeyValuePair<string, PageRights>> AllPagesUserRightsList { get; private set; }
            = new ObservableCollection<KeyValuePair<string, PageRights>>();

        private User _currentUser = new User();
        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }

        private string _deviceID = string.Empty;
        public string DeviceID
        {
            get => _deviceID;
            set
            {
                _deviceID = value;
                OnPropertyChanged();
            }
        }

        private string _deviceIP = string.Empty;
        public string DeviceIP
        {
            get => _deviceIP;
            set
            {
                _deviceIP = value;
                OnPropertyChanged();
            }
        }

        private string _deviceUser = string.Empty;
        public string DeviceUser
        {
            get => _deviceUser;
            set
            {
                _deviceUser = value;
                OnPropertyChanged();
            }
        }

        public VM_Page90(IS_Settings settings, IS_Notification notification, IQ_User user, IQ_UserAppPageDroit upaQuery)
        {
            _settings = settings;
            _notification = notification;
            _userQuery = user;
            _upaQuery = upaQuery;
            LoadData();
        }

        private async void LoadData()
        {
            await LoadUserDataAsync();
            await LoadUserAccessRightsAsync();
            LoadAllPagesUserRightsList();
        }

        public async Task LoadUserDataAsync()
        {
            var user = await _userQuery.HandleGetByIdAsync(_settings.GetAppUserID());
            if (user != null)
            {
                CurrentUser = user;
            }
            else
            {
                // Gérez le cas où l'utilisateur n'est pas trouvé
                CurrentUser = new User
                {
                    Id = 0,
                    Nom = "Non trouvé",
                    Prenom = "Non trouvé",
                    Login = "Non trouvé",
                    Initial = "N/A",
                    TelPro = "N/A",
                    TelFixePro = "N/A",
                    MailPro = "N/A"
                };
            }
            DeviceUser = _settings.GetCRDeviceUser();
            DeviceID = _settings.GetCRDeviceID();
            DeviceIP = _settings.GetCRDeviceIP();
        }

        private async Task LoadUserAccessRightsAsync()
        {
            try
            {
                // Obtenir l'ID utilisateur et l'ID application
                var userId = _settings.GetAppUserID();
                var appId = _settings.GetAppID();

                // Appeler le query handler pour obtenir les droits d'accès
                var userRights = await _upaQuery.HandleGetByUserIdAppIdAsync(userId, appId);

                // Mettre à jour la liste observable
                PagesUserRightsList.Clear();
                foreach (var droit in userRights)
                {
                    PagesUserRightsList.Add(droit);
                }
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_15", ex.Message);
            }
        }

        private void LoadAllPagesUserRightsList()
        {
            try
            {
                // Appeler la méthode pour obtenir les droits utilisateur depuis le dictionnaire
                var userRights = _settings.GetAllPagesUserRights();

                // Vider la liste existante
                AllPagesUserRightsList.Clear();

                // Ajouter chaque élément de userRights dans PagesUserRightsList
                foreach (var right in userRights)
                {
                    AllPagesUserRightsList.Add(right);
                }
            }
            catch (Exception ex)
            {
                _notification.Error("No_EC_15", ex.Message);
            }
        }
    }
}
