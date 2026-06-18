using BatchStockRelease.A_Domain.Common.Enums;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;
using BatchStockRelease.D_Presentation.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Windows;

namespace BatchStockRelease
{
    /// <summary>
    /// <b>Fenêtre principale de l’application BatchStockRelease.</b>
    /// </summary>
    /// <para>
    /// Cette classe constitue le point d’entrée graphique principal de l’application.
    /// Elle orchestre l’initialisation de l’interface, la gestion de la session utilisateur,
    /// la surveillance des commandes d’administration et la navigation globale entre pages.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// La fenêtre est chargée après l’exécution du UseCase <c>UC_Application_OnStart</c>.
    /// Elle crée dynamiquement la bannière (<c>BA_MainWindow</c>) une fois le conteneur
    /// de dépendances initialisé, puis exécute les opérations suivantes :
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Applique les dimensions minimales et maximise la fenêtre principale.</description></item>
    ///   <item><description>Abonne les événements de cycle de vie (chargement, fermeture, redimensionnement).</description></item>
    ///   <item><description>Démarre l’écoute asynchrone des messages et commandes de session.</description></item>
    ///   <item><description>Initialise le ViewModel (<c>VM_MainWindow</c>) pour exécuter les cas d’usage de démarrage.</description></item>
    /// </list>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Offrir une base stable, modulaire et testable pour l’ensemble des processus UI.
    /// La logique métier (authentification, navigation, fermeture) est déléguée aux
    /// UseCases et services injectés via le conteneur DI.
    /// </para>
    ///
    /// <b>ViewModel associé :</b>
    /// <para><c>VM_MainWindow</c> — gère l’état global et les interactions principales de la fenêtre.</para>
    ///
    /// <b>Composants intégrés :</b>
    /// <list type="bullet">
    ///   <item><description><c>CommonBackgroundPad</c> : fond visuel partagé entre les modules.</description></item>
    ///   <item><description><c>BA_MainWindow</c> : bannière principale chargée dynamiquement.</description></item>
    ///   <item><description><c>ActiveHorizontalMenu</c> : zone de menu horizontal.</description></item>
    ///   <item><description><c>ActivePage</c> : conteneur pour les pages métier (PageXX).</description></item>
    /// </list>
    ///
    /// <b>Services injectés :</b>
    /// <list type="bullet">
    ///   <item><description><c>IS_Window</c> — gestion des dimensions et position de la fenêtre.</description></item>
    ///   <item><description><c>IS_Messages</c> — gestion et écoute des messages utilisateurs.</description></item>
    ///   <item><description><c>IS_UserSessionsAdmin</c> — écoute et exécution des commandes administratives.</description></item>
    ///   <item><description><c>IU_User_Authentification</c> — identification utilisateur.</description></item>
    ///   <item><description><c>IU_User_CloseApplication</c> — fermeture contrôlée de la session applicative.</description></item>
    /// </list>
    ///
    /// <b>Cycle de vie :</b>
    /// <list type="number">
    ///   <item><description><c>OnLoaded</c> : initialise les dimensions, charge la bannière, lance l’initialisation du ViewModel.</description></item>
    ///   <item><description><c>OnClosing</c> : intercepte la demande de fermeture, exécute la logique métier associée.</description></item>
    ///   <item><description><c>OnClosed</c> : nettoie les ressources et annule les tâches asynchrones.</description></item>
    ///   <item><description><c>OnWindowSizeChanged / OnWindowStateChanged / OnWindowLocationChanged</c> : met à jour la configuration de la fenêtre en temps réel.</description></item>
    /// </list>
    ///
    /// <b>Remarques techniques :</b>
    /// <para>
    /// Les événements de fermeture et de redimensionnement sont synchronisés avec le service
    /// <c>IS_Window</c> afin d’assurer la cohérence visuelle et la persistance des dimensions.
    /// Les appels asynchrones utilisent un <c>CancellationTokenSource</c> annulé proprement
    /// lors de la fermeture.
    /// </para>

    public partial class MainWindow : Window
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom de la classe pour la traçabilité (inclus dans les chaînes <c>callChain</c>).
        /// </summary>
        private readonly string _callee;

        /// <summary>
        /// Source du jeton d'annulation utilisée par les listeners temps réel
        /// (écoute administrative et messagerie interne).
        /// On recrée un nouveau token à chaque redémarrage des listeners.
        /// </summary>
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// Tâche d’écoute des commandes administratives (déconnexion forcée, etc.).
        /// Mémorisée pour pouvoir attendre sa fin lors de l’arrêt.
        /// </summary>
        private Task? _adminListenerTask;

        /// <summary>
        /// Tâche d’écoute de la messagerie interne (notifications utilisateur, messages non lus).
        /// Mémorisée pour pouvoir attendre sa fin lors de l’arrêt.
        /// </summary>
        private Task? _messagesListenerTask;

        #endregion

        #region === Dépendances privées ===

        private readonly VM_MainWindow _viewModel;

        private readonly IS_Window _window;
        private readonly IS_Messages _messages;
        private readonly IS_UserSession_Admin _userSessionsAdmin;
        private readonly IS_Settings_App _settingsApp;
        private readonly IU_User_Authentification _userAuthentification;
        private readonly IU_User_CloseApplication _userCloseApplication;
        private readonly IU_DB_Monitoring _dbMonitoring;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise la fenêtre principale et résout les dépendances via DI.
        /// </summary>
        public MainWindow(
            VM_MainWindow viewModel,
            IS_Window window,
            IS_Messages messages,
            IS_UserSession_Admin userSessionsAdmin,
            IS_Settings_App settingsApp,
            IU_User_Authentification userAuthentification,
            IU_User_CloseApplication userCloseApplication,
            IU_DB_Monitoring dbMonitoring)
        {
            InitializeComponent();

            _callee = GetType().Name;

            // Résolution DI
            _viewModel = viewModel;
            _window = window;
            _messages = messages;
            _userSessionsAdmin = userSessionsAdmin;
            _settingsApp = settingsApp;
            _userAuthentification = userAuthentification;
            _userCloseApplication = userCloseApplication;
            _dbMonitoring = dbMonitoring;

            DataContext = _viewModel;

            // Abonnement aux événements du cycle de vie WPF
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.Closing += OnClosing;
            this.Closed += OnClosed;
            this.SizeChanged += OnWindowSizeChanged;
            this.StateChanged += OnWindowStateChanged;
            this.LocationChanged += OnWindowLocationChanged;

            // Abonnement aux événements globaux de connexion (via IS_Settings_App)
            _settingsApp.ConnectionLost += OnConnectionLost;
            _settingsApp.ConnectionRestored += OnConnectionRestored;
        }

        #endregion

        #region === Méthodes publiques ===

        // A compléter

        #endregion

        #region === Événements WPF : Chargement / Déchargement / Fermeture ===

        /// <summary>
        /// Appelé quand la fenêtre est affichée pour la première fois.
        /// Initialise l'UI, le ViewModel, les listeners et le monitoring DB.
        /// </summary>
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnLoaded)}";

            // 1. Gabarit visuel minimal + plein écran
            MainWindowWindow.MinWidth = _window.GetMainWindowMinWidth();
            MainWindowWindow.MinHeight = _window.GetMainWindowMinHeight();
            WindowState = WindowState.Maximized;

            // 2. Synchronise les dimensions/position auprès du service fenêtre
            _window.UpdateWindowDimensions(this);

            // 3. Charge dynamiquement la bannière si pas déjà présente
            if (BannerHost.Content == null)
            {
                var banner = new D_Presentation.Views.Components.Banner.BA_MainWindow();
                BannerHost.Content = banner;
            }

            // 4. Initialise le ViewModel (ex : récup user courant, droits, page de départ, etc.)
            await _viewModel.InitializeAsync(callChain);

            // 5. Crée un premier token d'écoute
            ResetListenerToken();

            // 6. Démarre les listeners applicatifs (messages, admin, etc.)
            await StartListenersAsync(callChain);

            // 7. Lance la surveillance de la connexion DB
            await StartDatabaseMonitorAsync(callChain);
        }

        /// <summary>
        /// Appelé quand la fenêtre est déchargée du visuel (mais pas forcément détruite).
        /// Utilisé uniquement si tu veux réagir au déchargement logique.
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }

        /// <summary>
        /// Intercepte la demande de fermeture (croix fenêtre, Alt+F4, etc.).
        /// Délègue la décision métier à <c>IU_User_CloseApplication</c>.
        /// </summary>
        private async void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnClosing)}";

            var result = await _userCloseApplication.ExecuteAsync(e, callChain);

            switch (result)
            {
                case En_CloseResult.Cancelled:
                    // Rien à faire : l’utilisateur a annulé la fermeture
                    break;

                case En_CloseResult.Closed:
                case En_CloseResult.ForceClosed:
                    // Désabonner l’événement pour éviter la boucle
                    this.Closing -= OnClosing;

                    // Fermer manuellement
                    Application.Current.Dispatcher.Invoke(() => this.Close());
                    break;
            }
        }

        /// <summary>
        /// Appelé après la fermeture effective de la fenêtre.
        /// Libère toutes les ressources asynchrones restantes (listeners, monitoring DB).
        /// </summary>
        private async void OnClosed(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnClosed)}";

            await StopListenersAsync(callChain);
            DisposeListenerToken();

            // Désabonnement des événements globaux
            _settingsApp.ConnectionLost -= OnConnectionLost;
            _settingsApp.ConnectionRestored -= OnConnectionRestored;
        }

        #endregion

        #region === Événements WPF : dimensions / état fenêtre ==

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

        #endregion


        #region === Gestion connexion DB ===

        /// <summary>
        /// Démarre la surveillance de la connexion base de données.
        /// </summary>
        private async Task StartDatabaseMonitorAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(StartDatabaseMonitorAsync)}";

            await _dbMonitoring.ExecuteAsync(callChain);
        }

        /// <summary>
        /// Handler déclenché quand la connexion DB est perdue.
        /// On arrête les listeners temps-réel de l'application.
        /// </summary>
        private async void OnConnectionLost(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnConnectionLost)}";

            try
            {
                await StopListenersAsync(callChain);
                Debug.WriteLine($"[{callChain}] Listeners suspendus (connexion perdue).");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur OnConnectionLost : {ex.Message}");
            }
        }

        /// <summary>
        /// Handler déclenché quand la connexion DB est restaurée.
        /// On relance les listeners temps-réel.
        /// </summary>
        private async void OnConnectionRestored(object? sender, EventArgs e)
        {
            string callChain = $"{_callee} > {nameof(OnConnectionRestored)}";

            try
            {
                ResetListenerToken();
                await StartListenersAsync(callChain);
                Debug.WriteLine($"[{callChain}] Listeners relancés (connexion restaurée).");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur OnConnectionRestored : {ex.Message}");
            }
        }

        #endregion

        #region === Listeners applicatifs (messages / admin) ===

        /// <summary>
        /// Lance les tâches asynchrones d'écoute (messages utilisateur, commandes admin).
        /// Ces tâches s'exécutent tant que la connexion DB est considérée OK.
        /// </summary>
        private async Task StartListenersAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(StartListenersAsync)}";

            try
            {
                // Par sécurité, si on n'a pas de CTS actif (appli relancée après reconnexion), on en crée un.
                if (_cancellationTokenSource == null)
                {
                    ResetListenerToken();
                }
                
                // Authentification utilisateur si nécessaire
                // (selon logique projet, si tu dois garantir qu'un user est identifié avant d'écouter)
                await _userAuthentification.ExecuteAsync(callChain);

                // Lancer l'écoute admin (fermeture forcée, etc.)
                _ = _userSessionsAdmin.ListenForCommandsAsync(_cancellationTokenSource.Token);

                // Lancer l'écoute des messages non lus / notifications applicatives
                _ = _messages.ListenForCommandsAsync(_cancellationTokenSource.Token, _callee);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur StartListenersAsync: {ex.Message}");
            }
        }

        /// <summary>
        /// Arrête proprement les tâches d'écoute utilisateur/admin
        /// en annulant le CancellationTokenSource courant.
        /// </summary>
        private async Task StopListenersAsync(string caller)
        {
            string callChain = $"{caller} > {nameof(StopListenersAsync)}";

            try
            {
                // Annule le token si pas encore annulé
                if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();

                // Attend la fin des tâches si elles existent
                if (_adminListenerTask != null)
                {
                    try { await _adminListenerTask; }
                    catch { /* on ignore ici, car l'annulation peut lever */ }
                    _adminListenerTask = null;
                }

                if (_messagesListenerTask != null)
                {
                    try { await _messagesListenerTask; }
                    catch { /* idem */ }
                    _messagesListenerTask = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur StopListenersAsync: {ex.Message}");
            }
        }

        #endregion

        #region === Gestion du CancellationTokenSource ===

        /// <summary>
        /// Crée un nouveau <c>CancellationTokenSource</c> pour un cycle d'écoute.
        /// Si un token précédent existait, il est d'abord annulé et libéré.
        /// </summary>
        private void ResetListenerToken()
        {
            string caller = nameof(ResetListenerToken);
            string callChain = $"{_callee} > {caller}";

            // On annule et on libère l'ancien cycle d'écoute s'il existe encore
            if (_cancellationTokenSource != null)
            {
                try
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                        _cancellationTokenSource.Cancel();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{callChain}] Erreur Cancel ancien CTS: {ex.Message}");
                }

                try
                {
                    _cancellationTokenSource.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{callChain}] Erreur Dispose ancien CTS: {ex.Message}");
                }
            }

            // Nouveau cycle propre
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Libère définitivement le <c>CancellationTokenSource</c> courant
        /// après la fermeture de l'application.
        /// </summary>
        private void DisposeListenerToken()
        {
            string caller = nameof(DisposeListenerToken);
            string callChain = $"{_callee} > {caller}";

            if (_cancellationTokenSource == null)
                return;

            try
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur Cancel final CTS: {ex.Message}");
            }

            try
            {
                _cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{callChain}] Erreur Dispose final CTS: {ex.Message}");
            }

            _cancellationTokenSource = null;
        }

        #endregion

    }
}