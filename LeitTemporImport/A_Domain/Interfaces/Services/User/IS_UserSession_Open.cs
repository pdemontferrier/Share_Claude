
namespace LeitTemporImport.A_Domain.Interfaces.Services.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de service User responsable de l’ouverture (ou réactivation) d’une session applicative utilisateur.
    /// Ce service orchestre la gestion des enregistrements <c>UserAppSession</c> au démarrage du programme console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé lors de l’initialisation de l’application, après identification de l’utilisateur et de l’application
    /// (appId, userId). Le service garantit l’existence d’une session “active” et met à jour le contexte utilisateur
    /// (ex : stockage de l’identifiant de session) pour assurer la traçabilité des opérations.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser l’ouverture de session en appliquant les règles suivantes :
    /// </para>
    /// <list type="bullet">
    /// <item><description>Rechercher les sessions existantes pour (userId, appId).</description></item>
    /// <item><description>Activer la session principale et supprimer les sessions supplémentaires (hard delete).</description></item>
    /// <item><description>Créer une session si aucune n’existe.</description></item>
    /// <item><description>Propager l’identifiant de session dans le contexte utilisateur global.</description></item>
    /// </list>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases User et pipeline de démarrage de l’application console.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Orchestrer les QueryHandlers et CommandHandlers associés à <c>UserAppSession</c>.</description></item>
    /// <item><description>Assurer la traçabilité via la callChain (<paramref name="caller"/>).</description></item>
    /// </list>
    /// </summary>
    public interface IS_UserSession_Open
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Ouvre (ou réactive) une session utilisateur pour l’application courante.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Exécuté au démarrage du programme console. La méthode doit garantir qu’une session active existe
        /// pour le couple (userId, appId), supprimer les doublons éventuels (hard delete) et mettre à jour
        /// le contexte utilisateur avec l’identifiant de session.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Préparer une exécution traçable et cohérente de l’application en initialisant une session unique.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Rechercher les sessions existantes.</description></item>
        /// <item><description>Mettre à jour la session principale et supprimer les sessions supplémentaires.</description></item>
        /// <item><description>Créer une session si aucune n’existe.</description></item>
        /// <item><description>Mettre à jour le contexte utilisateur (AppSessionId).</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="userId">Identifiant utilisateur.</param>
        /// <param name="appId">Identifiant application.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// </summary>
        Task ExecuteAsync(string caller, int userId, int appId);
    }
}