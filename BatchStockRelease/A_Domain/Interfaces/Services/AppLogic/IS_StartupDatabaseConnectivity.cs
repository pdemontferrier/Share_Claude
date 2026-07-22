
namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier responsable de la vérification de la connectivité entre l’application
    /// et la base de données principale. Il constitue la première étape de validation
    /// au démarrage du système.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé par le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> lors du processus
    /// d’initialisation de l’application. Il permet de s’assurer que la base de données
    /// est disponible et opérationnelle avant de poursuivre les vérifications suivantes.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Confirmer la disponibilité de la base de données et détecter tout problème
    /// de connectivité ou d’infrastructure avant l’ouverture de la fenêtre principale.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Établir une connexion à la base de données via le handler <see cref="BatchStockRelease.A_Domain.Interfaces.Handlers.Queries.IQ_User"/>.</description></item>
    /// <item><description>Exécuter une requête de test simple pour valider la communication.</description></item>
    /// <item><description>Retourner <c>true</c> si la base répond correctement, sinon <c>false</c>.</description></item>
    /// <item><description>Journaliser les erreurs de connectivité via <see cref="IS_LogAndNotify"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IS_StartupDatabaseConnectivity
    {
        /// <summary>
        /// Teste la connexion à la base de données et retourne le statut de la communication.
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (CallChain).</param>
        /// <returns><c>true</c> si la connexion est opérationnelle, sinon <c>false</c>.</returns>
        Task<bool> TestConnectionAsync(string caller);
    }
}
