using LeitTemporImport.B_UseCases.Settings.User;

namespace LeitTemporImport.A_Domain.Interfaces.Settings.User
{
    /// <summary>
    /// Interface de gestion des paramètres utilisateur et des informations de session.
    /// </summary>
    public interface ISE_User
    {
        // Identité de l’utilisateur
        int GetAppUserId();
        void SetAppUserId(int userId);

        /// <summary>
        /// Retourne l’identifiant de la session connectée.
        /// </summary>
        int GetAppSessionId() => SE_User.AppSessionId;

        /// <summary>
        /// Définit l’identifiant de la session connectée.
        /// </summary>
        /// <param name="sessionId">Identifiant de la session.</param>
        void SetAppSessionId(int sessionId);

        // Poste client
        string GetAppDeviceUser();
        string GetAppDeviceId();
        string GetAppDeviceIP();
    }
}