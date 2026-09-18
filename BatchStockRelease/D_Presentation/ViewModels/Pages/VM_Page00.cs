using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.ViewModels.Generic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page00 — Page de connexion utilisateur.
    ///
    /// <para><b>Contexte :</b> Cette page constitue la porte d’entrée de l’application.
    /// Elle gère l’authentification de l’utilisateur ainsi que la validation de ses droits
    /// d’accès à l’application courante.</para>
    ///
    /// <para><b>Objectif :</b> Vérifier les identifiants, appliquer les règles de sécurité
    /// (3 tentatives maximum) et initialiser la session utilisateur.</para>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les collaborateurs accédant à BatchStockRelease
    /// depuis la plateforme CITRIX (tablette ou poste fixe).</para>
    ///
    /// <para><b>View associée :</b> <c>Page00.xaml</c></para>
    ///
    /// <list type="bullet">
    ///   <item><description>Contrôle des identifiants (login / mot de passe).</description></item>
    ///   <item><description>Validation des droits d’accès à l’application.</description></item>
    ///   <item><description>Gestion des tentatives et fermeture après 3 échecs.</description></item>
    /// </list>
    /// </summary>
    public class VM_Page00 : VM_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IU_User_Authentification _userAuthentification;
        private readonly IS_User_IsLoginPasswordValid _user_IsLoginPasswordValid;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_Notification _notification;
        private readonly IS_Shutdown _application;
        #endregion

        #region === Commandes ===

        /// <summary>
        /// Commande exécutée lors du clic sur le bouton "Valider" dans la vue.
        /// </summary>
        public ICommand LoginCommand { get; }

        /// <summary>
        /// Vérifie si la commande de connexion peut être exécutée.
        /// </summary>
        private bool CanExecuteLogin()
            => !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

        /// <summary>
        /// Méthode exécutée lors de l’exécution de la commande de connexion.
        /// </summary>
        private async Task ExecuteLoginAsync()
        {
            string caller = $"{_callee} > {nameof(ExecuteLoginAsync)}";
            await UserAuthenticationAsync(Login, Password);
            Password = string.Empty;
        }

        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel de la page de connexion avec les services nécessaires.
        /// </summary>
        /// <param name="userAuthentification">UseCase d’authentification des utilisateurs.</param>
        /// <param name="user_IsLoginPasswordValid">Service de validation du Login et Password.</param>
        /// <param name="settingsApp">Service des paramètres applicatifs.</param>
        /// <param name="settingsUser">Service des paramètres utilisateur (tentatives, session, etc.).</param>
        /// <param name="notification">Service d’affichage des notifications (avertissements, erreurs).</param>
        /// <param name="application">Service de gestion de l’application (fermeture, redémarrage).</param>
        /// <param name="dictionary">Service multilingue.</param>
        /// <param name="settingsUseCase">Service de gestion des paramètres métier génériques.</param>
        /// <param name="navigation">Service de navigation entre pages.</param>
        /// <param name="logAndNotify">Service de journalisation des erreurs.</param>
        public VM_Page00(
            IU_User_Authentification userAuthentification,
            IS_User_IsLoginPasswordValid user_IsLoginPasswordValid,
            IS_Settings_App settingsApp,
            IS_Settings_User settingsUser,
            IS_Notification notification,
            IS_Shutdown application,
            IS_Dictionary dictionary,
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_LogAndNotify logAndNotify)
            : base(settingsUseCase, navigation, dictionary, logAndNotify)
        {
            _userAuthentification = userAuthentification;
            _user_IsLoginPasswordValid = user_IsLoginPasswordValid;
            _settingsApp = settingsApp;
            _settingsUser = settingsUser;
            _notification = notification;
            _application = application;

            // Initialisation des commandes
            LoginCommand = new UT_RelayCommandArg0Async(ExecuteLoginAsync, CanExecuteLogin);
        }
        #endregion

        #region === Propriétés publiques liées à la vue ===

        private string _login = string.Empty;
        /// <summary>
        /// Identifiant de connexion saisi par l’utilisateur.
        /// </summary>
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        private string _password = string.Empty;
        /// <summary>
        /// Mot de passe saisi par l’utilisateur.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        #endregion

        #region === Méthodes publiques ===
        /// <summary>
        /// Authentifie l’utilisateur à partir du login et du mot de passe fournis.
        /// </summary>
        /// <param name="login">Identifiant de l’utilisateur.</param>
        /// <param name="password">Mot de passe de l’utilisateur.</param>
        /// <returns>Tâche asynchrone représentant le processus d’authentification.</returns>
        public async Task UserAuthenticationAsync(string login, string password)
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(UserAuthenticationAsync));

            await ExecuteSafeAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
                {
                    UserAttempt("No_Se_01", callChain);
                    return;
                }

                var user = await _user_IsLoginPasswordValid.ExecuteAsync(login, password, callChain);
                if (user is not null)
                {
                    // Mettre à jour _settingsUser.GetAppUserId();
                    _settingsUser.SetAppUserId(user.Id);

                    // Appeller le usecase d'authentification 
                    await _userAuthentification.ExecuteAsync(callChain);
                }
                else
                {
                    UserAttempt("No_Wa_05", callChain);
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Gère les tentatives d’authentification échouées.
        /// </summary>
        /// <param name="messageText">Clé du message d’avertissement à afficher.</param>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        private void UserAttempt(string messageText, string caller)
        {
            string callChain = $"{caller} > {nameof(UserAttempt)}";

            _settingsUser.IncrementUserAttempt();
            int attempt = _settingsUser.GetUserAttempt();

            if (attempt >= 3)
            {
                _application.ForceShutdown("No_Wa_07");
            }
            else
            {
                _notification.Warning(
                    messageText,
                    $"\n{attempt} {_dictionary.GetText("No_Wa_06")}");
            }
        }

        #endregion
    }
}