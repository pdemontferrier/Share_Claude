
namespace LeitTemporImport.B_UseCases.Settings.App
{
    /// <summary>
    /// Classe statique centrale de l’application.
    /// <para>
    /// La classe <c>SE_App</c> fournit un point d’accès global aux paramètres,
    /// constantes et états partagés de l’application Feld10Processor.
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
        public static readonly int AppId = 1;

        /// <summary>
        /// Retourne la date du jour (sans heure).
        /// </summary>
        public static DateTime AppDate => DateTime.Today;

        /// <summary>
        /// Retourne la date et l’heure système actuelles.
        /// </summary>
        public static DateTime AppDateTime => DateTime.Now;

        /// <summary>
        /// Nom du dossier contenant les fichiers de logs d’erreurs.
        /// </summary>
        public static readonly string ErrorLogFolder = "99_Errorlog";

        /// <summary>
        /// Nom du fichier CSV principal de logs d’erreurs.
        /// </summary>
        public static readonly string ErrorLogFileName = "error_log.csv";

        #endregion

    }
}