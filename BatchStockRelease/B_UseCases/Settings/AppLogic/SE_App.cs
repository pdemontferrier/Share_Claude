using System.ComponentModel;
using System.IO;

namespace BatchStockRelease.B_UseCases.Settings.AppLogic
{
    /// <summary>
    /// <b>Classe statique centrale de l’application.</b>
    /// <para>
    /// La classe <c>SE_App</c> fournit un point d’accès global aux paramètres,
    /// constantes et états partagés de l’application <b>BatchStockRelease</b>.
    /// Elle centralise :
    /// </para>
    /// <list type="bullet">
    /// <item><description>les variables d’environnement (Dev / Prod),</description></item>
    /// <item><description>les délais standards et chemins communs,</description></item>
    /// <item><description>les états applicatifs (messages, connexion),</description></item>
    /// <item><description>et les événements statiques de notification.</description></item>
    /// </list>
    /// </summary>
    public static class SE_App
    {
        #region === Propriétés globales ===

        /// <summary>
        /// Définit l’environnement d’exécution courant.
        /// Valeurs possibles : <c>"Dev"</c> ou <c>"Prod"</c>.
        /// </summary>
        public static readonly string Environment = "Dev";

        /// <summary>
        /// Identifiant unique de l’application dans l’écosystème ERP.
        /// </summary>
        public static readonly int AppId = 41;

        /// <summary>
        /// Identifiant de l’action associée à l’accès utilisateur dans l’ERP.
        /// </summary>
        public static readonly int AppAction = 47;

        /// <summary>
        /// Retourne la date du jour (sans heure).
        /// </summary>
        public static DateTime AppDate => DateTime.Today;

        /// <summary>
        /// Retourne la date et l’heure système actuelles.
        /// </summary>
        public static DateTime AppDateTime => DateTime.Now;

        /// <summary>
        /// Chemin absolu vers le répertoire commun <c>CommonResources</c>.
        /// </summary>
        public static Uri CommonRessourcesPath =>
            Environment == "Prod"
                ? new Uri(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\00_CommonResources")))
                : new Uri(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\..\09_Deployment\00_CommonResources")));

        /// <summary>
        /// Nom du dossier contenant les fichiers de logs d’erreurs.
        /// </summary>
        public static readonly string ErrorLogFolder = "99_Errorlog";

        /// <summary>
        /// Nom du fichier CSV principal de logs d’erreurs.
        /// </summary>
        public static readonly string ErrorLogFileName = "error_log.csv";

        /// <summary>
        /// Délai (en secondes) avant l’ouverture automatique de la fenêtre modale d’attente.
        /// </summary>
        public static readonly int ShowDialogWindowDelay = 5;

        /// <summary>
        /// Délai (en secondes) avant la fermeture automatique d’une commande utilisateur.
        /// </summary>
        public static readonly int CloseCommandDelay = 60;

        /// <summary>
        /// Délai (en secondes) entre deux vérifications de nouveaux messages.
        /// </summary>
        public static readonly int MessageCheckDelay = 60;

        /// <summary>
        /// Délai (en secondes) entre deux notifications de message.
        /// </summary>
        public static readonly int MessageNotificationDelay = 300;

        #endregion

        #region === État des messages utilisateur ===

        private static bool _hasUnreadMessages;

        /// <summary>
        /// Indique si des messages non lus sont présents pour l’utilisateur.
        /// <para>Déclenche l’événement <see cref="PropertyChanged"/> lors d’une modification.</para>
        /// </summary>
        public static bool HasUnreadMessages
        {
            get => _hasUnreadMessages;
            set
            {
                if (_hasUnreadMessages != value)
                {
                    _hasUnreadMessages = value;
                    OnPropertyChanged(nameof(HasUnreadMessages));
                }
            }
        }

        #endregion

        #region === Monitoring de la connexion base de données ===

        /// <summary>
        /// Intervalle en secondes entre deux vérifications de la connexion.
        /// </summary>
        public static readonly int DatabaseCheckInterval = 5;

        /// <summary>
        /// Délai du premier cycle d’observation rapide (en secondes).
        /// </summary>
        public static readonly int DatabaseCheckFirstLoop = 30;

        /// <summary>
        /// Délai du second cycle d’observation lente (en secondes).
        /// </summary>
        public static readonly int DatabaseCheckSecondLoop = 60;

        private static bool _isConnected = true;
        private static bool _dialogOpen = false;

        /// <summary>
        /// Indique si la connexion à la base de données est actuellement considérée comme active.
        /// </summary>
        public static bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        /// <summary>
        /// Indique si la fenêtre modale de perte de connexion est ouverte.
        /// </summary>
        public static bool IsDialogOpen
        {
            get => _dialogOpen;
            set
            {
                if (_dialogOpen != value)
                {
                    _dialogOpen = value;
                    OnPropertyChanged(nameof(IsDialogOpen));
                }
            }
        }

        /// <summary>
        /// Événement global levé lorsque l’application perd la connexion à la base.
        /// </summary>
        public static event EventHandler? ConnectionLost;

        /// <summary>
        /// Événement global levé lorsque l’application retrouve la connexion à la base.
        /// </summary>
        public static event EventHandler? ConnectionRestored;

        /// <summary>
        /// Déclenche l’événement <see cref="ConnectionLost"/> et met à jour <see cref="IsConnected"/>.
        /// </summary>
        public static void NotifyConnectionLost()
        {
            IsConnected = false;
            ConnectionLost?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Déclenche l’événement <see cref="ConnectionRestored"/> et met à jour <see cref="IsConnected"/>.
        /// </summary>
        public static void NotifyConnectionRestored()
        {
            IsConnected = true;
            ConnectionRestored?.Invoke(null, EventArgs.Empty);
        }

        #endregion

        #region === INotifyPropertyChanged statique ===

        /// <summary>
        /// Événement levé lors de la modification d’une propriété statique.
        /// Permet à la couche UI (WPF) de se synchroniser automatiquement.
        /// </summary>
        public static event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Déclenche l’événement <see cref="PropertyChanged"/> pour la propriété spécifiée.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        internal static void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}