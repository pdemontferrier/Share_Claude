using BatchStockRelease.A_Domain.App.Entities;
using BatchStockRelease.A_Domain.Entities.GestStock;
using System.ComponentModel;

namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// Interface de gestion des paramètres utilisateur et des informations de session.
    /// </summary>
    public interface IS_Settings_User
    {
        // Relayer les abonnements à PropertyChanged
        event PropertyChangedEventHandler? PropertyChanged;

        // Gestion des propriétés utilisateur
        void UpdateUserSetting<T>(string propertyName, ref T field, T newValue);

        // Identité de l’utilisateur
        int GetAppUserId();
        void SetAppUserId(int userID);

        string GetAppUserFullName();
        void SetAppUserFullName(string fullName);

        // Poste client
        string GetAppDeviceUser();
        string GetAppDeviceId();
        string GetAppDeviceIP();

        // Tentatives de connexion
        int GetUserAttempt();
        void SetUserAttempt(int attempt);
        void IncrementUserAttempt();

        // Commande de fermeture et état
        string GetCloseCommandType();
        bool GetForceClose();
        void SetForceClose(bool forceClose);

        // Sessions utilisateur
        int GetSessionId();
        void SetSessionId(int sessionId);
        int GetSelectedSessionId();
        void SetSelectedSessionId(int selectedSessionId);
        string GetSelectedSessionFullName();
        void SetSelectedSessionFullName(string fullName);

        // Droits d’accès
        bool GetCanUserAccessApp();
        void SetCanUserAccessApp(bool value);

        // Droits
        void InitializeUserDefaultAccesses();
        List<KeyValuePair<string, PageRights>> GetAllPagesUserRights();
        void SetPageAccessRights(List<UserAppPageDroit> pageAccessRights);
        PageRights? GetPageRights(string pageName);
    }
}