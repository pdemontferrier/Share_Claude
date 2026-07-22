using System.ComponentModel;
using BatchStockRelease.A_Domain.App.Entities;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.B_UseCases.Settings.UserLogic;

namespace BatchStockRelease.B_UseCases.Services.UserLogic
{
    /// <summary>
    /// Service de gestion des paramètres utilisateur.
    /// <para>
    /// Centralise les informations relatives à la session, aux tentatives de connexion,
    /// et aux métadonnées du poste client (utilisateur, machine, adresse IP).
    /// </para>
    /// <para>
    /// Cette classe agit comme un proxy entre la logique applicative et la couche statique
    /// <see cref="SE_User"/>, en assurant le relais des notifications de changement
    /// pour permettre la synchronisation avec les ViewModels.
    /// </para>
    /// </summary>
    public class SR_Settings_User : IS_Settings_User, INotifyPropertyChanged
    {
        #region === Événement PropertyChanged ===

        /// <summary>
        /// Événement déclenché lorsqu'une propriété utilisateur est modifiée.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructeur : relaye automatiquement les changements de <see cref="SE_User"/>
        /// vers le service pour informer les ViewModels abonnés.
        /// </summary>
        public SR_Settings_User()
        {
            SE_User.PropertyChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, e);
            };
        }

        #endregion

        #region === Méthodes génériques ===

        /// <summary>
        /// Met à jour une propriété utilisateur et notifie du changement.
        /// </summary>
        /// <typeparam name="T">Type de la propriété.</typeparam>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        /// <param name="field">Référence du champ à mettre à jour.</param>
        /// <param name="newValue">Nouvelle valeur à appliquer.</param>
        public void UpdateUserSetting<T>(string propertyName, ref T field, T newValue)
        {
            if (!EqualityComparer<T>.Default.Equals(field, newValue))
            {
                field = newValue;
                SE_User.OnPropertyChanged(propertyName);
            }
        }

        #endregion

        #region === Identifiant utilisateur ===

        /// <summary>
        /// Retourne l’identifiant de l’utilisateur connecté.
        /// </summary>
        public int GetAppUserId() => SE_User.AppUserId;

        /// <summary>
        /// Définit l’identifiant de l’utilisateur connecté.
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        public void SetAppUserId(int userId)
        {
            if (SE_User.AppUserId != userId)
            {
                SE_User.AppUserId = userId;
                SE_User.OnPropertyChanged(nameof(SE_User.AppUserId));
            }
        }

        #endregion

        #region === Nom complet utilisateur ===

        /// <summary>
        /// Retourne le nom complet de l’utilisateur connecté.
        /// </summary>
        public string GetAppUserFullName() => SE_User.AppUserFullName;

        /// <summary>
        /// Définit le nom complet de l’utilisateur connecté.
        /// </summary>
        /// <param name="fullName">Nom complet à définir.</param>
        public void SetAppUserFullName(string fullName)
        {
            if (SE_User.AppUserFullName != fullName)
            {
                SE_User.AppUserFullName = fullName;
                SE_User.OnPropertyChanged(nameof(SE_User.AppUserFullName));
            }
        }

        #endregion

        /// <summary>
        /// Informations sur le poste client : nom d'utilisateur, identifiant machine, IP.
        /// </summary>
        public string GetAppDeviceUser() => SE_User.AppDeviceUser;
        public string GetAppDeviceId() => SE_User.AppDeviceId;
        public string GetAppDeviceIP() => SE_User.AppDeviceIP;

        /// <summary>
        /// Gestion des tentatives de connexion utilisateur.
        /// </summary>
        public int GetUserAttempt() => SE_User.UserAttempt;
        public void SetUserAttempt(int attempt) => SE_User.UserAttempt = attempt;
        public void IncrementUserAttempt() => SE_User.UserAttempt++;

        /// <summary>
        /// Indique le mode de fermeture de session et le statut de fermeture forcée.
        /// </summary>
        public string GetCloseCommandType() => SE_User.CloseCommandType;
        public bool GetForceClose() => SE_User.ForceClose;
        public void SetForceClose(bool forceClose) => SE_User.ForceClose = forceClose;

        /// <summary>
        /// Gestion des sessions actives et sélectionnées.
        /// </summary>
        public int GetSessionId() => SE_User.SessionId;
        public void SetSessionId(int sessionId) => SE_User.SessionId = sessionId;

        public int GetSelectedSessionId() => SE_User.SelectedSessionId;
        public void SetSelectedSessionId(int selectedSessionId) => SE_User.SelectedSessionId = selectedSessionId;

        public string GetSelectedSessionFullName() => SE_User.SelectedSessionFullName;
        public void SetSelectedSessionFullName(string fullName) => SE_User.SelectedSessionFullName = fullName;

        /// <summary>
        /// Gestion des droits d’accès utilisateur à l’application.
        /// </summary>
        public bool GetCanUserAccessApp() => SE_User.CanUserAccessApp;
        public void SetCanUserAccessApp(bool value) => SE_User.CanUserAccessApp = value;


        /// <summary>
        /// Gestion des droitd d’accès et permissions utilisateur
        /// </summary>
        public void InitializeUserDefaultAccesses() => SE_User.InitializeDefaultPageAccessRights();

        public List<KeyValuePair<string, PageRights>> GetAllPagesUserRights()
            => SE_User.PagesUserRights.ToList();

        public void SetPageAccessRights(List<UserAppPageDroit> pageAccessRights)
            => SE_User.SetUserPageAccessRights(pageAccessRights);

        public PageRights? GetPageRights(string pageName)
            => SE_User.GetPageRights(pageName);
    }
}