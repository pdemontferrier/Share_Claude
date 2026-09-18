
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// Service de gestion des sessions utilisateurs.
    /// <para>
    /// Ce service orchestre la création, la mise à jour et la fermeture
    /// des sessions liées à un utilisateur et à une application spécifique.
    /// </para>
    /// </summary>
    public interface IS_UserSession_Close
    {
        /// <summary>
        /// Ferme la session utilisateur spécifiée.
        /// </summary>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="sessionId">Identifiant unique de la session à fermer.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        Task ExecuteAsync(int userId, int sessionId, string caller);
    }
}
