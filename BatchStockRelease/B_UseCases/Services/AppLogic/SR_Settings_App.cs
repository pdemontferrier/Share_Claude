using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.B_UseCases.Settings.AppLogic;
using CommonResources.Settings;
using System;
using System.ComponentModel;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// <b>Service de gestion des paramètres applicatifs généraux.</b>
    /// <para>
    /// Ce service agit comme une façade entre les constantes définies dans <see cref="SE_App"/>
    /// et les paramètres partagés contenus dans <see cref="CR_CommonSettings"/> et <see cref="CR_DataBaseSettings"/>.
    /// Il centralise les informations globales nécessaires à l’application, telles que :
    /// </para>
    /// <list type="bullet">
    /// <item><description>les constantes d’initialisation et de configuration,</description></item>
    /// <item><description>les chemins communs et répertoires partagés,</description></item>
    /// <item><description>les délais de rafraîchissement et de notifications,</description></item>
    /// <item><description>les états applicatifs (connexion, messages non lus),</description></item>
    /// <item><description>et les paramètres d’accès aux bases de données.</description></item>
    /// </list>
    /// </summary>
    public class SR_Settings_App : IS_Settings_App
    {
        #region === Événements ===

        /// <summary>
        /// Événement déclenché lorsqu'une propriété locale ou globale est modifiée.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructeur : relaye automatiquement les événements de <see cref="SE_App"/>
        /// vers les ViewModels abonnés au service.
        /// </summary>
        public SR_Settings_App()
        {
            // Relais des PropertyChanged de SE_App
            SE_App.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);

            // Relais des événements de connexion pour la couche UI
            SE_App.ConnectionLost += (s, e) => ConnectionLost?.Invoke(this, e);
            SE_App.ConnectionRestored += (s, e) => ConnectionRestored?.Invoke(this, e);
        }

        /// <summary>
        /// Événement levé lorsque l’application perd la connexion à la base de données.
        /// </summary>
        public event EventHandler? ConnectionLost;

        /// <summary>
        /// Événement levé lorsque l’application retrouve la connexion à la base de données.
        /// </summary>
        public event EventHandler? ConnectionRestored;

        #endregion

        #region === Gestion des messages non lus ===

        /// <summary>
        /// Retourne <see langword="true"/> si des messages non lus existent pour l’utilisateur courant.
        /// </summary>
        public bool GetHasUnreadMessages() => SE_App.HasUnreadMessages;

        /// <summary>
        /// Définit l’état de présence de messages non lus.
        /// </summary>
        /// <param name="hasUnreadMessages">Valeur indiquant la présence de messages non lus.</param>
        public void SetHasUnreadMessages(bool hasUnreadMessages)
        {
            if (SE_App.HasUnreadMessages != hasUnreadMessages)
            {
                SE_App.HasUnreadMessages = hasUnreadMessages;
                SE_App.OnPropertyChanged(nameof(SE_App.HasUnreadMessages));
            }
        }

        #endregion

        #region === Informations générales de l’application ===

        /// <summary>Retourne l’identifiant unique de l’application.</summary>
        public int GetAppId() => SE_App.AppId;

        /// <summary>Retourne l’identifiant d’action lié à l’accès ERP.</summary>
        public int GetAppAction() => SE_App.AppAction;

        /// <summary>Retourne la date du jour.</summary>
        public DateTime GetAppDate() => SE_App.AppDate;

        /// <summary>Retourne la date et l’heure actuelles.</summary>
        public DateTime GetAppDateTime() => SE_App.AppDateTime;

        /// <summary>Retourne le chemin du dossier des logs d’erreurs.</summary>
        public string GetErrorLogFolder() => SE_App.ErrorLogFolder;

        /// <summary>Retourne le nom du fichier CSV de logs d’erreurs.</summary>
        public string GetErrorLogFileName() => SE_App.ErrorLogFileName;

        /// <summary>Retourne le délai avant ouverture de la fenêtre modale (en secondes).</summary>
        public int GetShowDialogWindowDelay() => SE_App.ShowDialogWindowDelay;

        /// <summary>Retourne le délai avant fermeture d’une commande utilisateur (en secondes).</summary>
        public int GetCloseCommandDelay() => SE_App.CloseCommandDelay;

        /// <summary>Retourne le délai entre deux vérifications de messages (en secondes).</summary>
        public int GetMessageCheckDelay() => SE_App.MessageCheckDelay;

        /// <summary>Retourne le délai entre deux notifications de message (en secondes).</summary>
        public int GetMessageNotificationDelay() => SE_App.MessageNotificationDelay;

        /// <summary>Retourne le chemin absolu du répertoire CommonResources.</summary>
        public Uri GetCommonRessourcesPath() => SE_App.CommonRessourcesPath;

        #endregion

        #region === Monitoring de la connexion base de données ===

        /// <summary>Retourne l’état de connexion courant de l’application.</summary>
        public bool GetIsConnected() => SE_App.IsConnected;

        /// <summary>Retourne l’état d’ouverture de la fenêtre modale de déconnexion.</summary>
        public bool GetIsDialogOpen() => SE_App.IsDialogOpen;

        /// <summary>Met à jour l’état d’ouverture de la fenêtre modale de déconnexion.</summary>
        public void SetIsDialogOpen(bool value) => SE_App.IsDialogOpen = value;

        /// <summary>Retourne l’intervalle entre deux tests de connexion (en secondes).</summary>
        public int GetDatabaseCheckInterval() => SE_App.DatabaseCheckInterval;

        /// <summary>Retourne le délai du premier cycle de reconnexion rapide (en secondes).</summary>
        public int GetDatabaseCheckFirstLoop() => SE_App.DatabaseCheckFirstLoop;

        /// <summary>Retourne le délai du second cycle de reconnexion lente (en secondes).</summary>
        public int GetDatabaseCheckSecondLoop() => SE_App.DatabaseCheckSecondLoop;

        /// <summary> Déclenche la notification globale de perte de connexion.</summary>
        public void NotifyConnectionLost() => SE_App.NotifyConnectionLost();

        /// <summary> Déclenche la notification globale de reconnexion.</summary>
        public void NotifyConnectionRestored() => SE_App.NotifyConnectionRestored();


        #endregion

        #region === Paramètres communs issus de CommonResources ===

        /// <summary>Retourne le titre principal de l’application.</summary>
        public string GetApplicationTitle() => CR_CommonSettings.ApplicationTitle;

        /// <summary>Met à jour le titre principal de l’application.</summary>
        public void SetApplicationTitle(string appTitle) => CR_CommonSettings.ApplicationTitle = appTitle;

        /// <summary>Retourne la chaîne de connexion à la base GestStock en production.</summary>
        public string GetCredentialsGeststock_prod() => CR_DataBaseSettings.CredentialsGeststock_prod;

        /// <summary>Retourne la chaîne de connexion à la base GestStock en développement.</summary>
        public string GetCredentialsGeststock_dev() => CR_DataBaseSettings.CredentialsGeststock_dev;

        /// <summary>Retourne le titre de la fenêtre modale en cours.</summary>
        public string GetDW_Title() => CR_CommonSettings.DW_Title;

        /// <summary>Définit le titre de la fenêtre modale.</summary>
        public void SetDW_Title(string dwTitle) => CR_CommonSettings.DW_Title = dwTitle;

        /// <summary>Retourne le contenu affiché dans la fenêtre modale.</summary>
        public string GetDW_Content() => CR_CommonSettings.DW_Content;

        /// <summary>Définit le contenu de la fenêtre modale.</summary>
        public void SetDW_Content(string dwContent) => CR_CommonSettings.DW_Content = dwContent;

        /// <summary>Retourne la largeur de la fenêtre modale.</summary>
        public int GetDW_Width() => CR_CommonSettings.DW_Width;

        /// <summary>Retourne la hauteur de la fenêtre modale.</summary>
        public int GetDW_Height() => CR_CommonSettings.DW_Height;

        #endregion

        #region === Répertoires partagés ===

        public string GetDirectoryPathServer1() => CR_CommonSettings.DirectoryPathServer1;
        public string GetDirectoryPathServer2() => CR_CommonSettings.DirectoryPathServer2;
        public string GetDirectoryPathGed1() => CR_CommonSettings.DirectoryPathGed1;
        public string GetDirectoryPathGed2() => CR_CommonSettings.DirectoryPathGed2;
        public string GetDirectoryPathGed3() => CR_CommonSettings.DirectoryPathGed3;
        public string GetDirectoryPathProjectModel() => CR_CommonSettings.DirectoryPathProjectModel;
        public string GetDirectoryPathBatchModel() => CR_CommonSettings.DirectoryPathBatchModel;
        public string GetDirectoryPathLotsDeFabrication() => CR_CommonSettings.DirectoryPathLotsDeFabrication;
        public string GetDirectoryPathElumatec() => CR_CommonSettings.DirectoryPathElumatec;
        public string GetDirectoryPathElumatec1() => CR_CommonSettings.DirectoryPathElumatec1;
        public string GetDirectoryPathElumatec2() => CR_CommonSettings.DirectoryPathElumatec2;
        public string GetDirectoryPathElumatec3() => CR_CommonSettings.DirectoryPathElumatec3;

        #endregion

        #region === Accès base de données ===

        /// <summary>
        /// Retourne la chaîne de connexion GestStock correspondant à l’environnement actif.
        /// </summary>
        public string GetCredentialsGeststock()
        {
            return SE_App.Environment == "Prod"
                ? CR_DataBaseSettings.CredentialsGeststock_prod
                : CR_DataBaseSettings.CredentialsGeststock_dev;
        }

        #endregion
    }
}