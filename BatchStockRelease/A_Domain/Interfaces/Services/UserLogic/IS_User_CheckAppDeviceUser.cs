
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Interface du service métier responsable de l’identification automatique de l’utilisateur
    /// à partir de son compte Windows (<c>LoginWindows</c>).
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Appelée au démarrage de l’application, cette vérification permet de détecter si
    /// le poste courant appartient à un utilisateur connu et de pré-remplir le contexte utilisateur.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Associer automatiquement un compte Windows à un utilisateur du système, afin de
    /// simplifier le processus de connexion.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Lire le login Windows depuis le service <see cref="IS_Settings_User"/>.</description></item>
    /// <item><description>Rechercher un utilisateur correspondant dans la base.</description></item>
    /// <item><description>Mettre à jour le contexte utilisateur si une correspondance est trouvée.</description></item>
    /// </list>
    /// </summary>
    public interface IS_User_CheckAppDeviceUser
    {
        /// <summary>
        /// Vérifie si le poste courant correspond à un utilisateur enregistré
        /// et met à jour le contexte utilisateur si c’est le cas.
        /// </summary>
        /// <param name="deviceUser">Nom d’utilisateur Windows du poste courant.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns>Identifiant de l’utilisateur si trouvé, sinon <c>0</c>.</returns>
        Task<int> ExecuteAsync(string deviceUser, string caller);
    }
}
