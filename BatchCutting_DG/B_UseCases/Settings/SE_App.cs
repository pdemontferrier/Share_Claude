using System.IO;

namespace BatchCutting_DG.B_UseCases.Settings
{
    public static class SE_App
    {
        // Définir l'environnement
        public static readonly string Environment = "Prod"; //  "Prod" - "Dev";

        // Identifiant de l'application
        public static readonly int AppID = 44;

        // Droit d'accès à l'application
        public static readonly int AppAccess = 51;

        // Dossier CommonResources
        private static readonly Uri commonRessourcesPath_dev = new Uri(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\..\..\09_Deployment\00_CommonResources")));
        private static readonly Uri commonRessourcesPath_prod = new Uri(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\00_CommonResources")));
        private static readonly Uri commonRessourcesPath = Environment == "Prod" ? commonRessourcesPath_prod : commonRessourcesPath_dev;
        public static Uri CommonRessourcesPath => commonRessourcesPath;

        // Date et heure de référence
        public static DateTime AppDate => DateTime.Today;
        public static DateTime AppDateTime => DateTime.Now; 

        // Délais en secondes
        public static readonly int ShowDialogWindowDelay = 5;
        public static readonly int CloseCommandDelay = 60;
        public static readonly int MessageCheckDelay = 60;
        public static readonly int MessageNotificationDelay = 300;

    }
}