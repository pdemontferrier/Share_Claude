using System;
using System.ComponentModel;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// <b>Interface de service des paramètres applicatifs généraux.</b>
    /// <para>
    /// Cette interface définit les membres publics du service <see cref="BatchStockRelease.B_UseCases.Services.AppLogic.SR_Settings_App"/>.
    /// Elle centralise les méthodes d’accès aux paramètres d’application, aux ressources partagées,
    /// et aux états globaux de l’environnement d’exécution.
    /// </para>
    /// <para>
    /// Elle inclut également les événements de notification liés à la surveillance de la connexion
    /// à la base de données et les informations de monitoring associées.
    /// </para>
    /// </summary>
    public interface IS_Settings_App : INotifyPropertyChanged
    {
        #region === Événements ===

        /// <summary>
        /// Événement levé lorsque l’application perd la connexion à la base de données.
        /// <para>
        /// Permet à la couche présentation (ViewModels, MainWindow) de suspendre
        /// les processus temps réel et d’afficher une modale d’avertissement.
        /// </para>
        /// </summary>
        event EventHandler? ConnectionLost;

        /// <summary>
        /// Événement levé lorsque l’application retrouve la connexion à la base de données.
        /// <para>
        /// Permet à la couche présentation de relancer les listeners ou tâches suspendues
        /// et de fermer la fenêtre modale d’attente.
        /// </para>
        /// </summary>
        event EventHandler? ConnectionRestored;

        #endregion

        #region === Gestion des messages non lus ===

        /// <summary>
        /// Retourne <see langword="true"/> si des messages non lus existent pour l’utilisateur.
        /// </summary>
        bool GetHasUnreadMessages();

        /// <summary>
        /// Définit la valeur du flag indiquant la présence de messages non lus.
        /// </summary>
        /// <param name="hasUnreadMessages">Valeur booléenne à appliquer.</param>
        void SetHasUnreadMessages(bool hasUnreadMessages);

        #endregion

        #region === Informations générales de l’application ===

        int GetAppId();
        int GetAppAction();
        DateTime GetAppDate();
        DateTime GetAppDateTime();
        string GetErrorLogFolder();
        string GetErrorLogFileName();

        int GetShowDialogWindowDelay();
        int GetCloseCommandDelay();
        int GetMessageCheckDelay();
        int GetMessageNotificationDelay();

        Uri GetCommonRessourcesPath();

        #endregion

        #region === Monitoring de la connexion base de données ===

        /// <summary>
        /// Retourne l’état de connexion courant de l’application.
        /// <para><c>true</c> = base accessible / <c>false</c> = déconnexion détectée.</para>
        /// </summary>
        bool GetIsConnected();

        /// <summary>
        /// Retourne l’état d’ouverture de la fenêtre modale de déconnexion.
        /// <para><c>true</c> = modale affichée / <c>false</c> = modale fermée.</para>
        /// </summary>
        bool GetIsDialogOpen();

        /// <summary>
        /// Met à jour l’état d’ouverture de la fenêtre modale de déconnexion.
        /// <para><c>true</c> = modale affichée / <c>false</c> = modale fermée.</para>
        /// </summary>
        void SetIsDialogOpen(bool value);

        /// <summary>
        /// Retourne l’intervalle (en secondes) entre deux vérifications de la connexion.
        /// </summary>
        int GetDatabaseCheckInterval();

        /// <summary>
        /// Retourne le délai du premier cycle de reconnexion rapide (en secondes).
        /// </summary>
        int GetDatabaseCheckFirstLoop();

        /// <summary>
        /// Retourne le délai du second cycle de reconnexion lente (en secondes).
        /// </summary>
        int GetDatabaseCheckSecondLoop();

        /// <summary>
        /// Notifie la perte de connexion à la base de données
        /// et déclenche l’événement <see cref="ConnectionLost"/>.
        /// </summary>
        void NotifyConnectionLost();

        /// <summary>
        /// Notifie la restauration de la connexion à la base de données
        /// et déclenche l’événement <see cref="ConnectionRestored"/>.
        /// </summary>
        void NotifyConnectionRestored();

        #endregion

        #region === Paramètres communs issus de CommonResources ===

        string GetApplicationTitle();
        void SetApplicationTitle(string appTitle);

        string GetCredentialsGeststock_prod();
        string GetCredentialsGeststock_dev();

        string GetDW_Title();
        void SetDW_Title(string dwTitle);

        string GetDW_Content();
        void SetDW_Content(string dwContent);

        int GetDW_Width();
        int GetDW_Height();

        #endregion

        #region === Répertoires partagés ===

        string GetDirectoryPathServer1();
        string GetDirectoryPathServer2();
        string GetDirectoryPathGed1();
        string GetDirectoryPathGed2();
        string GetDirectoryPathGed3();
        string GetDirectoryPathProjectModel();
        string GetDirectoryPathBatchModel();
        string GetDirectoryPathLotsDeFabrication();
        string GetDirectoryPathElumatec();
        string GetDirectoryPathElumatec1();
        string GetDirectoryPathElumatec2();
        string GetDirectoryPathElumatec3();

        #endregion

        #region === Accès base de données ===

        /// <summary>
        /// Retourne la chaîne de connexion GestStock correspondant à l’environnement actif
        /// (<c>Prod</c> ou <c>Dev</c>).
        /// </summary>
        string GetCredentialsGeststock();

        #endregion
    }
}


/*
 * 

using System.ComponentModel;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// Interface de gestion des paramètres applicatifs généraux.
    /// Centralise l’accès aux constantes, dossiers, et temporisations de l’application.
    /// </summary>
    public interface IS_Settings_App
    {
        // Relayer les abonnements à PropertyChanged
        event PropertyChangedEventHandler? PropertyChanged;

        // SE_App
        int GetAppId();
        int GetAppAction();
        DateTime GetAppDate();
        DateTime GetAppDateTime();
        string GetErrorLogFolder();
        string GetErrorLogFileName();

        int GetShowDialogWindowDelay();
        int GetCloseCommandDelay();
        int GetMessageCheckDelay();
        int GetMessageNotificationDelay();

        Uri GetCommonRessourcesPath();
        bool GetHasUnreadMessages();
        void SetHasUnreadMessages(bool hasUnreadMessages);

        int GetDatabaseCheckInterval();
        int GetDatabaseCheckFirstLoop();
        int GetDatabaseCheckSecondLoop();


        // CommonResources.Settings
        string GetApplicationTitle();
        void SetApplicationTitle(string appTitle);

        string GetCredentialsGeststock_prod();
        string GetCredentialsGeststock_dev();

        string GetDW_Title();
        void SetDW_Title(string dwTitle);

        string GetDW_Content();
        void SetDW_Content(string dwContent);

        int GetDW_Width();
        int GetDW_Height();

        string GetDirectoryPathServer1();
        string GetDirectoryPathServer2();
        string GetDirectoryPathGed1();
        string GetDirectoryPathGed2();
        string GetDirectoryPathGed3();
        string GetDirectoryPathProjectModel();
        string GetDirectoryPathBatchModel();
        string GetDirectoryPathLotsDeFabrication();
        string GetDirectoryPathElumatec();
        string GetDirectoryPathElumatec1();
        string GetDirectoryPathElumatec2();
        string GetDirectoryPathElumatec3();


        // Acces base de données
        string GetCredentialsGeststock();
    }
}*/