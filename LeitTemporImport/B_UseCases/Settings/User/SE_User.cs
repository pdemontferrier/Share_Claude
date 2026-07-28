using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using LeitTemporImport.B_UseCases.Settings.App;

namespace LeitTemporImport.B_UseCases.Settings.User
{
    /// <summary>
    /// Fournit les informations relatives à l'utilisateur courant et au poste d'exécution.
    /// <para>Cette classe centralise les identifiants de session, de machine, d'utilisateur
    /// ainsi que les droits d'accès aux pages.</para>
    /// </summary>
    public class SE_User
    {
        /// <summary>
        /// Identification utilisateur
        /// </summary>
        private static int _appUserID = SE_App.Environment == "Prod" ? 0 : 0;
        /// <summary>
        /// Identification utilisateur
        /// </summary>
        public static int AppUserId
        {
            get => _appUserID;
            set
            {
                if (_appUserID != value)
                {
                    _appUserID = value;
                    OnPropertyChanged(nameof(AppUserId));
                }
            }
        }

        /// <summary>
        /// Identification du FullName
        /// </summary>
        private static string _appUserFullName = "Utilisateur non identifié !";
        /// <summary>
        /// Identification du FullName
        /// </summary>
        public static string AppUserFullName
        {
            get => _appUserFullName;
            internal set
            {
                if (_appUserFullName != value)
                {
                    _appUserFullName = value;
                    OnPropertyChanged(nameof(AppUserFullName));
                }
            }
        }

        /// <summary>
        /// Nom de la machine exécutant l'application.
        /// </summary>
        public static string AppDeviceId => Environment.MachineName;

        /// <summary>
        /// Adresse IP locale du poste.
        /// </summary>
        public static string AppDeviceIP => GetPrimaryIPv4Address();

        /// <summary>
        /// Récupère la première adresse IPv4 valide du poste.
        /// </summary>
        /// <returns>Une chaîne représentant l'adresse IPv4 ou "127.0.0.1" si aucune interface active n'est trouvée.</returns>
        private static string GetPrimaryIPv4Address()
        {
            foreach (var ni in GetActiveNetworkInterfaces())
            {
                var ip = GetValidIPv4FromInterface(ni);
                if (!string.IsNullOrEmpty(ip))
                    return ip;
            }

            // Aucun réseau actif trouvé
            return "127.0.0.1";
        }

        /// <summary>
        /// Retourne les interfaces réseau actives, physiques (Ethernet ou Wi-Fi) et non virtuelles.
        /// </summary>
        private static IEnumerable<NetworkInterface> GetActiveNetworkInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(ni =>
                    ni.OperationalStatus == OperationalStatus.Up &&
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) &&
                    !ni.Description.ToLower().Contains("virtual") &&
                    !ni.Description.ToLower().Contains("pseudo"));
        }

        /// <summary>
        /// Extrait la première adresse IPv4 valide (non APIPA) d'une interface disposant d'une passerelle.
        /// </summary>
        private static string? GetValidIPv4FromInterface(NetworkInterface ni)
        {
            var ipProps = ni.GetIPProperties();

            if (!ipProps.GatewayAddresses.Any(g => g.Address.AddressFamily == AddressFamily.InterNetwork))
                return null;

            foreach (var ip in ipProps.UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork &&
                    !ip.Address.ToString().StartsWith("169.254"))
                {
                    return ip.Address.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Nom de l'utilisateur Windows connecté.
        /// </summary>
        public static string AppDeviceUser
        {
            get
            {
                var name = WindowsIdentity.GetCurrent().Name;
                return name.Contains('\\') ? name.Split('\\').Last() : name;
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Identifiant de la session applicative courante (<see cref="UserAppSession"/>).
        /// </para>
        /// <para>Contexte</para>
        /// <para>
        /// Cette valeur est initialisée au démarrage du programme console après création
        /// (ou récupération) d’une session utilisateur via les CommandHandlers/QueryHandlers
        /// dédiés à <c>UserAppSession</c>.
        /// </para>
        /// <para>
        /// Elle est ensuite partagée entre les différents UseCases durant toute
        /// l’exécution du process afin d’assurer une traçabilité cohérente.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Permettre :
        /// </para>
        /// <list type="bullet">
        /// <item><description>L’association des écritures applicatives à une session unique.</description></item>
        /// <item><description>La mise à jour correcte de la session lors de la fermeture du programme.</description></item>
        /// <item><description>La cohérence entre UserAppSession, UserAppEventStore et UserAppErrorLog.</description></item>
        /// </list>
        /// </summary>
        public static int AppSessionId { get; set; }

        public static event PropertyChangedEventHandler? PropertyChanged;
        internal static void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }
}