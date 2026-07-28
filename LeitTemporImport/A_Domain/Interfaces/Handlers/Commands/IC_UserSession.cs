using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de CommandHandler dédié à <see cref="UserAppSession"/>.
    /// Définit les commandes nécessaires à la gestion des sessions applicatives
    /// (ouverture/fermeture du programme) et à la mise à jour des informations
    /// utilisateur / device associées.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases de démarrage et d’arrêt de l’application console
    /// pour créer une session, basculer l’état connecté/déconnecté et gérer d’éventuels doublons.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser les opérations d’écriture sur <see cref="UserAppSession"/> tout en assurant
    /// la traçabilité (callChain) et la journalisation via la mécanique générique des handlers.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application (App / Business).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Créer une nouvelle session utilisateur connectée.</description></item>
    /// <item><description>Mettre à jour une session existante (connexion/déconnexion).</description></item>
    /// <item><description>Supprimer des sessions supplémentaires si nécessaire.</description></item>
    /// </list>
    /// </summary>
    public interface IC_UserSession
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Crée un nouvel enregistrement <see cref="UserAppSession"/> en état connecté.</para>
        /// <para>Contexte</para>
        /// <para>Appelé à l’ouverture du programme console.</para>
        /// <para>Objectif</para>
        /// <para>Insérer une session avec les informations d’application, utilisateur et device courantes.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire l’entité session.</description></item>
        /// <item><description>Persister l’entité via le handler générique (add).</description></item>
        /// <item><description>Déclencher la journalisation snapshot via Event Store.</description></item>
        /// </list>
        /// </summary>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <returns>Tâche asynchrone.</returns>
        Task HandleCreateNewUserSessionAsync(string callChain);

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour une session en positionnant l’état connecté/déconnecté et la date correspondante.</para>
        /// <para>Contexte</para>
        /// <para>Appelé lors de l’ouverture (connecté) ou de la fermeture (déconnecté) du programme console.</para>
        /// <para>Objectif</para>
        /// <para>Mettre à jour l’état et les informations de device associées à la session.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Mettre à jour les informations device.</description></item>
        /// <item><description>Mettre à jour <c>IsConnected</c> et la date associée.</description></item>
        /// <item><description>Persister via le handler générique (update).</description></item>
        /// <item><description>Déclencher la journalisation snapshot via Event Store.</description></item>
        /// </list>
        /// </summary>
        /// <param name="entity">Entité session à mettre à jour.</param>
        /// <param name="isConnected">Nouvel état connecté/déconnecté.</param>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <returns>Tâche asynchrone.</returns>
        Task HandleUpdateUserSessionAsync(UserAppSession entity, bool isConnected, string callChain);

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime des sessions supplémentaires (hard delete) afin d’éviter les doublons.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lorsqu’une règle métier impose de ne conserver qu’une session pertinente.</para>
        /// <para>Objectif</para>
        /// <para>Supprimer physiquement les sessions fournies et journaliser chaque suppression.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Parcourir les sessions à supprimer.</description></item>
        /// <item><description>Supprimer via le handler générique (delete).</description></item>
        /// <item><description>Déclencher la journalisation snapshot via Event Store.</description></item>
        /// </list>
        /// </summary>
        /// <param name="additionalSessions">Liste des sessions à supprimer.</param>
        /// <param name="callChain">Chaîne d’appel amont (non modifiée).</param>
        /// <returns>Tâche asynchrone.</returns>
        Task HandleDeleteAdditionalSessionsAsync(IEnumerable<UserAppSession> additionalSessions, string callChain);
    }
}