
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier chargé de vérifier l’intégrité des sessions utilisateur actives.
    /// Il permet de détecter si un même utilisateur tente d’ouvrir une session
    /// sur un autre poste alors qu’une session est déjà active.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé au démarrage de l’application depuis le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/>
    /// afin de garantir la cohérence et l’unicité de la session utilisateur dans le système.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Identifier toute session utilisateur active sur un poste différent
    /// et empêcher la création d’une session concurrente.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Récupérer les sessions actives pour l’utilisateur et l’application courante.</description></item>
    /// <item><description>Comparer l’identifiant du poste avec celui de la session enregistrée.</description></item>
    /// <item><description>Retourner <c>true</c> si une autre session active est détectée.</description></item>
    /// <item><description>Journaliser toute erreur de vérification via <see cref="BatchStockRelease.A_Domain.Interfaces.Services.AppLogic.IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IS_UserSession_Integrity
    {
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si une session utilisateur est déjà active sur un autre poste.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée lors du démarrage de l’application pour garantir
        /// qu’un utilisateur ne dispose que d’une seule session active dans le système.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Empêcher l’ouverture d’une nouvelle session si l’utilisateur est déjà connecté
        /// depuis un autre poste.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Lire la liste des sessions actives pour l’utilisateur courant.</description></item>
        /// <item><description>Comparer les identifiants de poste pour détecter des connexions multiples.</description></item>
        /// <item><description>Retourner <c>true</c> si une session concurrente est détectée, sinon <c>false</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="appId">Identifiant de l’application à vérifier.</param>
        /// <param name="userId">Identifiant de l’utilisateur concerné.</param>
        /// <param name="appDeviceId">Identifiant unique du poste local (device).</param>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (<c>CallChain</c>).</param>
        /// <returns><c>true</c> si une session active est détectée sur un autre poste, sinon <c>false</c>.</returns>
        Task<bool> HasActiveSessionOnAnotherDeviceAsync(int appId, int userId, string appDeviceId, string caller);
    }
}
