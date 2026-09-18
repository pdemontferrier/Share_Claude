
namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// <b>Service de diagnostic ou de vérification</b> chargé de vérifier la disponibilité de l’application dans le système.
    /// Il interroge la table ou la vue de suivi applicatif afin de déterminer
    /// si l’application est actuellement ouverte à l’utilisation ou verrouillée
    /// pour maintenance, mise à jour ou autre raison.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est utilisé au démarrage de l’application par le 
    /// <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> pour s’assurer que l’accès
    /// à l’application est autorisé avant d’initialiser les vues principales.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Garantir que l’application n’est pas en maintenance ou désactivée.
    /// Empêcher l’ouverture d’une session utilisateur si l’application
    /// est temporairement inaccessible.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Interroger la table de surveillance applicative (<c>vie_application</c>).</description></item>
    /// <item><description>Déterminer si l’application est marquée comme accessible.</description></item>
    /// <item><description>Retourner <c>true</c> si l’accès est autorisé, sinon <c>false</c>.</description></item>
    /// <item><description>Journaliser les erreurs de connexion ou d’infrastructure via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IS_ApplicationAvailability
    {
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Vérifie si l’application est actuellement accessible aux utilisateurs.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée lors du démarrage de l’application pour éviter
        /// d’ouvrir une session lorsque l’application est verrouillée ou en maintenance.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Empêcher tout accès non autorisé à l’application en cas d’inaccessibilité temporaire.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Construire la chaîne de traçabilité complète (<c>callChain</c>).</description></item>
        /// <item><description>Interroger la vue <c>vie_application</c> pour déterminer le statut de l’application.</description></item>
        /// <item><description>Retourner <c>true</c> si l’application est accessible, sinon journaliser l’erreur et retourner <c>false</c>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="appId">Identifiant unique de l’application à vérifier.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si l’application est accessible, sinon <c>false</c>.</returns>
        Task<bool> IsAppAccessibleAsync(int appId, string caller);
    }
}
