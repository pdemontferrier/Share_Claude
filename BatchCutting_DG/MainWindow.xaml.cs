using System.Windows;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.UserLogic;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.B_UseCases.Settings;
using System.Windows.Media.Imaging;

namespace BatchCutting_DG
{
    public partial class MainWindow : Window
    {
        private readonly IS_Settings _settings;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Flag _flag;
        private readonly IS_Icons _icons;
        private readonly IS_UserSession _userSession;
        private readonly IS_Notification _notification;
        private readonly IS_Window _window;
        private readonly IS_Navigation _navigation;
        private readonly IS_Dictionary _dictionary;
        private readonly IS_UserAuthentification _userAuthentification;
        private readonly IS_UserSessionsAdmin _userSessionsAdmin;
        private readonly IS_Messages _messages;
        private readonly IS_Application _application;
        private readonly IS_CuttingMachineIdentification _cuttingMachineIdentification;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
            _settings = App.ServiceProvider.GetRequiredService<IS_Settings>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _flag = App.ServiceProvider.GetRequiredService<IS_Flag>();
            _icons = App.ServiceProvider.GetRequiredService<IS_Icons>();
            _userSession = App.ServiceProvider.GetRequiredService<IS_UserSession>();
            _notification = App.ServiceProvider.GetRequiredService<IS_Notification>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            _navigation = App.ServiceProvider.GetRequiredService<IS_Navigation>();
            _dictionary = App.ServiceProvider.GetRequiredService<IS_Dictionary>();
            _userAuthentification = App.ServiceProvider.GetRequiredService<IS_UserAuthentification>();
            _userSessionsAdmin = App.ServiceProvider.GetRequiredService<IS_UserSessionsAdmin>();
            _messages = App.ServiceProvider.GetRequiredService<IS_Messages>();
            _application = App.ServiceProvider.GetRequiredService<IS_Application>();
            _cuttingMachineIdentification = App.ServiceProvider.GetRequiredService<IS_CuttingMachineIdentification>();

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.Closing += OnClosing;
            this.Closed += OnClosed;
            this.SizeChanged += OnWindowSizeChanged;
            this.StateChanged += OnWindowStateChanged;
            this.LocationChanged += OnWindowLocationChanged;

            // Écouter pour la femeture forcée de l'application
            _ = _userSessionsAdmin.ListenForCommandsAsync(_cancellationTokenSource.Token);

            // Écouter pour les messages non lus
            _ = _messages.ListenForCommandsAsync(_cancellationTokenSource.Token);

            // S'abonner aux changements du nom de l'utilisateur
            _settings.PropertyChanged += OnSettingsPropertyChanged;

            // S'abonner aux changements de statut des messages
            _messages.UnreadMessagesStatusChanged += OnUnreadMessagesStatusChanged;
        }


        // Méthodes relatives à la page
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Dimensions minimales
            MainWindowWindow.MinWidth = _window.GetMainWindowMinWidth();
            MainWindowWindow.MinHeight = _window.GetMainWindowMinHeight();

            // Maximiser la fenêtre
            WindowState = WindowState.Maximized;

            // Initialiser les dimensions de la fenêtre
            _window.UpdateWindowDimensions(this);

            // Appliquer les styles
            StyleControls();

            // Vérifier les autorisations de l'utilisateur
            await CheckUserAuthentificationAsync();

            // Vérifier l'adresse IP du poste pour déterminer la machine de découpe
            await CheckDeviceIpForCuttingMachineAsync();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }

        private async void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Si la fermeture est forcée, ne pas afficher de confirmation
            if (_settings.GetForceClose())
            {
                await CloseUserSessionAsync();
                return;
            }

            // Sinon, demander une confirmation
            // Afficher une boîte de dialogue de confirmation
            var result = _notification.ConfirmationReturn("No_AD_01");

            // Si l'utilisateur clique sur "Non", annuler la fermeture
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Annuler temporairement la fermeture pour attendre la déconnexion
                e.Cancel = true;

                // Effectuer la fermeture de session
                await CloseUserSessionAsync();

                // Déconnecter l'événement pour éviter une boucle
                this.Closing -= OnClosing;

                // Fermer l'application sans déclencher `OnClosing` à nouveau
                Application.Current.Dispatcher.Invoke(() => this.Close());
            }
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Mettre à jour les dimensions de la fenêtre
            _window.UpdateWindowDimensions(this);
        }

        private void OnWindowStateChanged(object? sender, EventArgs e)
        {
            // Mettre à jour les dimensions de la fenêtre
            _window.UpdateWindowDimensions(this);
        }

        private void OnWindowLocationChanged(object? sender, EventArgs e)
        {
            // Mettre à jour les dimensions de la fenêtre
            _window.UpdateWindowDimensions(this);
        }


        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Drapeau
            _controlStyler.StyleAppLanguageButton(LanguageButton, LanguageIcon);
            LanguageIcon.Source = new BitmapImage(_flag.GetAppFlagUri());

            // Utilisateur
            _controlStyler.StyleAppUserButton(UserFullNameButton, UserFullName);
            UserFullName.Text = _settings.GetAppUserFullName();

            // AppInfo
            _controlStyler.StyleAppInfoButton(AppInfo, AppInfoSign);

            // Messages
            _controlStyler.StyleAppMessageButton(MessageButton, MessageButtonIcon, MessageNotReadButtonIcon);
            MessageButtonIcon.Source = new BitmapImage(_icons.GetEmailIcon_Source());
            MessageNotReadButtonIcon.Source = new BitmapImage(_icons.GetEmailNotReadIcon_Source());

            // AppClose
            _controlStyler.StyleAppCloseButton(AppCloseButton, AppCloseButtonIcon);
            AppCloseButtonIcon.Source = new BitmapImage(_icons.GetAppCloseBlue_Source());
        }

        private void On_Language_Click(object sender, EventArgs e)
        {
            _navigation.NavigateToNewPage("Page91");
        }

        private void On_UserFullName_Click(object sender, EventArgs e)
        {
            _navigation.NavigateToNewPage("Page90");
        }

        private void On_Message_Click(object sender, EventArgs e)
        {
            _navigation.NavigateToNewPage("Page96");
        }

        private void On_AppInfo_Click(object sender, EventArgs e)
        {
            _navigation.NavigateToNewPage("Page98");
        }

        private void On_AppClose_Click(object sender, EventArgs e)
        {
            _application.ShutdownApplication();
        }


        // Méthodes relatives à l'utilisateur
        private async Task CheckUserAuthentificationAsync()
        {
            if (_settings.GetAppUserID() == 0)
            {
                // Déléguer l'authentification à UserAuthentificationService
                bool isAuthenticated = await _userAuthentification.AuthenticateWindowsUserAsync();

                if (isAuthenticated)
                {
                    // Vérifier si l'utilisateur peut accéder à l'application
                    await CheckUserAccessAppAsync();
                }
                else
                {
                    // Afficher la page d'identification
                    ActivePage.Navigate(_navigation.GetPage00_Source());
                }
            }
            else
            {
                // Vérifier si l'utilisateur peut accéder à l'application
                await CheckUserAccessAppAsync();
            }
        }

        private async Task CheckUserAccessAppAsync()
        {
            // Vérifier si l'utilisateur peut accéder à l'application
            await _userAuthentification.CheckUserAccessAppAsync(
                _settings.GetAppUserID(),
                _settings.GetAppID(),
                _settings.GetAppAccess(),
                _dictionary.GetText("P00_05"));
        }

        public void DisplayButtonBanner()
        {
            LanguageButton.Visibility = Visibility.Visible;
            UserFullNameButton.Visibility = Visibility.Visible;
            MessageButton.Visibility = Visibility.Visible;
            AppInfo.Visibility = Visibility.Visible;
            AppCloseButton.Visibility = Visibility.Visible;
        }

        private async Task CloseUserSessionAsync()
        {
            // Vérifier si l'utilisateur encours est identifié
            if (_settings.GetAppUserID() > 0 && _settings.GetSessionId() > 0)
            {
                // Deconnecter l'utilisateur
                await _userSession.CloseUserSessionAsync(_settings.GetSessionId());
            }
        }

        private void OnUnreadMessagesStatusChanged(bool hasUnreadMessages)
        {
            var userId = _settings.GetAppUserID();
            if (userId == 0)
            {
                MessageButtonIcon.Visibility = Visibility.Collapsed;
                MessageNotReadButtonIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (hasUnreadMessages && _settings.GetAppUserID() != 0)
                {
                    MessageButtonIcon.Visibility = Visibility.Collapsed;
                    MessageNotReadButtonIcon.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageButtonIcon.Visibility = Visibility.Visible;
                    MessageNotReadButtonIcon.Visibility = Visibility.Collapsed;
                }
            }
        }

        private async Task CheckDeviceIpForCuttingMachineAsync()
        {
            await _cuttingMachineIdentification.ExecuteAsync();
        }


        private void OnSettingsPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SE_User.AppUserFullName))
            {
                UserFullName.Text = _settings.GetAppUserFullName();
            }

            if (e.PropertyName == nameof(SE_User.AppUserID))
            {
                if (_settings.GetAppUserID() != 0)
                {
                    Dispatcher.Invoke(DisplayButtonBanner);
                }
            }
        }

        public void RefreshLanguageIcon()
        {
            _controlStyler.StyleAppLanguageButton(LanguageButton, LanguageIcon);
            LanguageIcon.Source = new BitmapImage(_flag.GetAppFlagUri());
        }
    }
}