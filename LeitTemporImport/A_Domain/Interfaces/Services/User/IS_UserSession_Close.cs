
namespace LeitTemporImport.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de service User responsable de la fermeture (déconnexion) d’une session applicative utilisateur.
    /// Ce service orchestre la mise à jour d’un enregistrement <c>UserAppSession</c> afin de clôturer proprement
    /// une exécution de l’application console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé lors de l’arrêt du programme console. La session cible est identifiée par son <c>sessionId</c>
    /// (clé technique de <c>UserAppSession</c>) et doit correspondre à l’utilisateur courant (<c>userId</c>).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser la fermeture d’une session en appliquant les règles suivantes :
    /// </para>
    /// <list type="bullet">
    /// <item><description>Charger la session par Id.</description></item>
    /// <item><description>Vérifier la cohérence de l’utilisateur propriétaire (sécurité fonctionnelle).</description></item>
    /// <item><description>Mettre à jour la session en état déconnecté (IsConnected=false, date de déconnexion).</description></item>
    /// <item><description>Assurer la traçabilité via la callChain (<paramref name="caller"/>).</description></item>
    /// </list>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases User et pipeline de fermeture de l’application console.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Orchestrer les QueryHandlers et CommandHandlers associés à <c>UserAppSession</c>.</description></item>
    /// <item><description>Garantir une fermeture cohérente et traçable de la session courante.</description></item>
    /// </list>
    /// </summary>
    public interface IS_UserSession_Close
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Ferme (déconnecte) la session utilisateur spécifiée.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Exécuté à l’arrêt du programme console. La méthode doit retrouver la session par <paramref name="sessionId"/>,
        /// vérifier qu’elle appartient à <paramref name="userId"/>, puis appliquer la mise à jour de déconnexion.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Marquer la session comme déconnectée afin de conserver un historique fiable et exploitable
        /// des exécutions de l’application.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Charger la session par Id.</description></item>
        /// <item><description>Vérifier la cohérence utilisateur/session.</description></item>
        /// <item><description>Mettre à jour la session en état déconnecté.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="sessionId">Identifiant de la session à fermer.</param>
        /// <returns>Tâche asynchrone.</returns>
        Task ExecuteAsync(string caller, int userId, int sessionId);
    }
}