using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using BatchStockRelease.A_Domain.App.Entities;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.B_UseCases.Settings.AppLogic;

namespace BatchStockRelease.B_UseCases.Settings.UserLogic
{
    /// <summary>
    /// Fournit les informations relatives à l'utilisateur courant et au poste d'exécution.
    /// <para>Cette classe centralise les identifiants de session, de machine, d'utilisateur
    /// ainsi que les droits d'accès aux pages.</para>
    /// </summary>
    public class SE_User
    {
        // --- Identification utilisateur ---
        private static int _appUserID = SE_App.Environment == "Prod" ? 0 : 0;
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

        // Identification du FullName
        private static string _appUserFullName = "Utilisateur non identifié !";
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

        // --- Autres propriétés statiques ---
        public static int UserAttempt = 0;

        // Commande pour la fermeture de session
        public static string CloseCommandType = "CloseSession";
        public static bool ForceClose = false;

        // Identifiant de la session
        public static int SessionId = 0;
        public static int SelectedSessionId = 0;
        public static string SelectedSessionFullName = string.Empty;

        // Enregistrer si l'utilisateur à le droit d'accéder à l'application
        public static bool CanUserAccessApp = false;

        // Dictionnaire pour mapper le nom de la page et le droit d'accès
        public static Dictionary<string, PageRights> PagesUserRights = new();

        /// <summary>
        /// Initialise les droits par défaut pour les pages de l'application.
        /// </summary>
        public static void InitializeDefaultPageAccessRights()
        {
            for (int i = 0; i <= 99; i++)
            {
                var pageName = $"Page{i:00}";
                if (!PagesUserRights.ContainsKey(pageName))
                {
                    PagesUserRights[pageName] = new PageRights
                    {
                        CanAccess = pageName is "Page00" or "Page99"
                    };
                }
            }
        }

        /// <summary>
        /// Met à jour les droits des pages après authentification.
        /// </summary>
        public static void SetUserPageAccessRights(List<UserAppPageDroit> pageAccessRights)
        {
            foreach (var pageAccess in pageAccessRights)
            {
                if (PagesUserRights.TryGetValue(pageAccess.Page, out var accessRights))
                {
                    accessRights.CanAccess = pageAccess.UserCanAccess;
                    accessRights.CanCreate = pageAccess.UserCanCreate;
                    accessRights.CanRead = pageAccess.UserCanRead;
                    accessRights.CanUpdate = pageAccess.UserCanUpdate;
                    accessRights.CanDelete = pageAccess.UserCanDelete;
                    accessRights.CanControl = pageAccess.UserCanControl;
                    accessRights.CanValidate = pageAccess.UserCanValidate;
                    accessRights.CanSupervise = pageAccess.UserCanSupervise;
                    accessRights.CanMonitor = pageAccess.UserCanMonitor;
                    accessRights.CanAdmin = pageAccess.UserCanAdmin;
                }
            }
        }

        /// <summary>
        /// Obtient les droits d'accès d'une page spécifique.
        /// </summary>
        public static PageRights? GetPageRights(string pageName) =>
            PagesUserRights.TryGetValue(pageName, out var rights) ? rights : null;

        public static event PropertyChangedEventHandler? PropertyChanged;
        internal static void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }
}