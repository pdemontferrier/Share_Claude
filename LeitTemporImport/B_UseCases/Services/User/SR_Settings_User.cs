using LeitTemporImport.A_Domain.Interfaces.Settings.User;
using LeitTemporImport.B_UseCases.Settings.User;

namespace LeitTemporImport.B_UseCases.Services.User
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
    public class SR_Settings_User : ISE_User
    {

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
            }
        }

        /// <summary>
        /// Retourne l’identifiant de la session connectée.
        /// </summary>
        public int GetAppSessionId() => SE_User.AppSessionId;

        /// <summary>
        /// Définit l’identifiant de la session connectée.
        /// </summary>
        /// <param name="sessionId">Identifiant de la session.</param>
        public void SetAppSessionId(int sessionId)
        {
            if (SE_User.AppSessionId != sessionId)
            {
                SE_User.AppSessionId = sessionId;
            }
        }

        /// <summary>
        /// Informations sur le poste client : nom d'utilisateur, identifiant machine, IP.
        /// </summary>
        public string GetAppDeviceUser() => SE_User.AppDeviceUser;
        public string GetAppDeviceId() => SE_User.AppDeviceId;
        public string GetAppDeviceIP() => SE_User.AppDeviceIP;

        #endregion

    }
}