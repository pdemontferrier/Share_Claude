using System.ComponentModel;
using BatchCutting_DG.A_Domain.AppEntities;
using BatchCutting_DG.A_Domain.Entities.GestStock;


namespace BatchCutting_DG.B_UseCases.Settings
{
    public class SE_User
    {
        // Identification de Id
        private static int _appUserID = SE_App.Environment == "Prod" ? 0 : 0;
        public static int AppUserID
        {
            get => _appUserID;
            set
            {
                if (_appUserID != value)
                {
                    _appUserID = value;
                    OnPropertyChanged(nameof(AppUserID));
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

        // Identification de la connexion
        public static string AppDeviceUser = "Non identifié";
        public static string AppDeviceID = "Non identifié";
        public static string AppDeviceIP = "Non identifié";
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
        public static Dictionary<string, PageRights> PagesUserRights = new Dictionary<string, PageRights>();

        // Initialise les valeurs par défaut pour PagesAccessRights.
        public static void InitializeDefaultPageAccessRights()
        {
            for (int i = 0; i <= 99; i++)
            {
                var pageName = $"Page{i:00}";
                if (!PagesUserRights.ContainsKey(pageName))
                {
                    if (pageName == "Page00" || pageName == "Page99")
                    {
                        // Attribuer un accès spécial pour ces pages
                        PagesUserRights[pageName] = new PageRights
                        {
                            CanAccess = true
                        };
                    }
                    else
                    {
                        PagesUserRights[pageName] = new PageRights();
                    }
                }
            }
        }

        // Mettre à jour les valeurs de PagesAccessRights après initialisation
        public static void SetUserPageAccessRights(List<UserAppPageDroit> pageAccessRights)
        {
            foreach (var pageAccess in pageAccessRights)
            {
                if (PagesUserRights.TryGetValue(pageAccess.Page, out var accessRights))
                {
                    // Mettre à jour les droits existants
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

        // Méthode pour obtenir les droits d'accès d'une page spécifique
        public static PageRights? GetPageRights(string pageName)
        {
            if (PagesUserRights.TryGetValue(pageName, out var pageRights))
            {
                return pageRights;
            }
            return null; // Retourne null si la page n'existe pas dans le dictionnaire
        }


        // Événement PropertyChanged statique
        public static event PropertyChangedEventHandler? PropertyChanged;

        internal static void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}