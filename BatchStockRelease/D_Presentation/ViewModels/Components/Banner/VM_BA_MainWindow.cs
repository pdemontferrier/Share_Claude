using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using BatchStockRelease.A_Domain.Common.Enums;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;
using BatchStockRelease.D_Presentation.Utilities;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.Banner
{
    /// <summary>
    /// ViewModel de la bannière principale de l’application.
    /// Gère la navigation entre les sections utilisateur, langue, messagerie et informations.
    /// </summary>
    public class VM_BA_MainWindow : INotifyPropertyChanged
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe appellée pour la traçabilité des logs et opérations.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances ===

        private readonly IU_User_Authentification _userAuthentification;
        private readonly IU_User_CloseApplication _userCloseApplication;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Navigation _navigation;
        private readonly IS_Shutdown _application;
        private readonly IS_Flags _flag;
        private readonly IS_Icons _icons;
        private readonly IS_Messages _messages;

        #endregion

        #region === Propriétés bindables ===

        private string _userFullName = string.Empty;
        public string UserFullName
        {
            get => _userFullName;
            set { _userFullName = value; OnPropertyChanged(); }
        }

        private bool _isBannerVisible;
        public bool IsBannerVisible
        {
            get => _isBannerVisible;
            set { _isBannerVisible = value; OnPropertyChanged(); }
        }

        private bool _hasUnreadMessages;
        public bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            set { _hasUnreadMessages = value; OnPropertyChanged(); }
        }

        private ImageSource? _languageIcon;
        public ImageSource? LanguageIcon
        {
            get => _languageIcon;
            set
            {
                if (_languageIcon != value)
                {
                    _languageIcon = value;
                    OnPropertyChanged();
                }
            }
        }

        // --- Icônes dynamiques ---
        public ImageSource MessageIcon => ImageConverter.FromUri(_icons.GetEmailIcon_Source());
        public ImageSource MessageUnreadIcon => ImageConverter.FromUri(_icons.GetEmailNotReadIcon_Source());
        public ImageSource AppCloseIcon => ImageConverter.FromUri(_icons.GetAppCloseBlue_Source());

        #endregion

        #region === Commandes ===

        public ICommand NavigateLanguageCommand { get; }
        public ICommand NavigateUserCommand { get; }
        public ICommand NavigateMessagesCommand { get; }
        public ICommand NavigateAppInfoCommand { get; }
        public ICommand CloseAppCommand { get; }

        #endregion

        #region === Constructeur ===

        public VM_BA_MainWindow(
            IU_User_Authentification userAuthentification,
            IU_User_CloseApplication userCloseApplication,
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_Navigation navigation,
            IS_Shutdown application,
            IS_Flags flag,
            IS_Icons icons,
            IS_Messages messages)
        {
            _callee = GetType().Name;

            _userAuthentification = userAuthentification;
            _userCloseApplication = userCloseApplication;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _navigation = navigation;
            _application = application;
            _flag = flag;
            _icons = icons;
            _messages = messages;

            // Initialiser les commandes de navigation
            NavigateLanguageCommand = new UT_RelayCommandArg0(() => _navigation.NavigateToNewPage("Page91"));
            NavigateUserCommand = new UT_RelayCommandArg0(() => _navigation.NavigateToNewPage("Page90"));
            NavigateMessagesCommand = new UT_RelayCommandArg0(() => _navigation.NavigateToNewPage("Page96"));
            NavigateAppInfoCommand = new UT_RelayCommandArg0(() => _navigation.NavigateToNewPage("Page98"));
            CloseAppCommand = new UT_RelayCommandArg0(() => _application.Shutdown(_callee));

            // Abonnements aux changements
            _settingsApp.PropertyChanged += OnSettingsAppPropertyChanged;
            _settingsUser.PropertyChanged += OnSettingsUserPropertyChanged;
            _flag.PropertyChanged += OnLanguageIconPropertyChanged;

        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Initialise le ViewModel : authentification automatique et affichage de la bannière.
        /// </summary>
        public async Task InitializeAsync(string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(InitializeAsync)}";

            RefreshLanguageIcon();
            RefreshUserFullName();

            if (_settingsUser.GetAppUserId() > 0)
                IsBannerVisible = true;
            else
                IsBannerVisible = false;

            HasUnreadMessages = _settingsApp.GetHasUnreadMessages();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gère la demande de fermeture de l’application.
        /// </summary>
        public async Task<En_CloseResult> RequestCloseAsync(CancelEventArgs e, string caller)
        {
            return await _userCloseApplication.ExecuteAsync(e, caller);
        }

        #endregion

        #region === Gestion dynamique des événements ===

        private void RefreshUserFullName()
        {
            UserFullName = _settingsUser.GetAppUserFullName();
        }

        /// <summary>
        /// Rafraîchit la langue (drapeau).
        /// </summary>
        private void RefreshLanguageIcon()
        {
            LanguageIcon = ImageConverter.FromUri(_flag.GetAppFlagUri());
        }

        /// <summary>
        /// Met à jour automatiquement la visibilité de la bannière
        /// en fonction de l’état de connexion utilisateur.
        /// </summary>
        private void UpdateBannerVisibility()
        {
            IsBannerVisible = _settingsUser.GetAppUserId() > 0;
        }

        /// <summary>
        /// Réagit aux changements d’état applicatif global (par ex. messages non lus).
        /// </summary>
        private void OnSettingsAppPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "HasUnreadMessages")
                HasUnreadMessages = _settingsApp.GetHasUnreadMessages();
        }

        private void OnSettingsUserPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AppUserFullName")
                RefreshUserFullName();

            if (e.PropertyName == "AppUserId")
                IsBannerVisible = _settingsUser.GetAppUserId() > 0;
        }

        private void OnLanguageIconPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AppFlagUri")
                RefreshLanguageIcon();
        }

        #endregion

        #region === INotifyPropertyChanged ===

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}