
namespace LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat d’un service permettant d’exécuter des procédures stockées SQL Server dans le cadre
    /// des traitements d’import et de synchronisation du projet 104, en respectant les standards de traçabilité
    /// (CallChain) et d’exécution asynchrone.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases orchestrateurs lorsque certaines étapes de traitement sont implémentées côté base
    /// via des procédures stockées (ex : consolidation, synchronisation, post-traitements).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une API stable et homogène pour déclencher des procédures stockées, avec ou sans paramètres,
    /// sans exposer les détails techniques d’accès aux données aux couches supérieures.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services applicatifs du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exécuter une procédure stockée sans paramètre.</description></item>
    /// <item><description>Exécuter une procédure stockée avec un paramètre.</description></item>
    /// </list>
    /// </summary>
    public interface IS_StoredProcedure
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée sans paramètre.</para>
        /// <para>Contexte</para>
        /// <para>Appelée depuis un UseCase orchestrateur d’import/synchronisation.</para>
        /// <para>Objectif</para>
        /// <para>Déclencher un traitement SQL côté serveur en conservant une traçabilité complète via la CallChain.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Recevoir la CallChain amont via <paramref name="caller"/>.</description></item>
        /// <item><description>Exécuter la procédure stockée identifiée par <paramref name="procedureName"/>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont (origine de l’appel).</param>
        /// <param name="procedureName">Nom SQL (schema.proc) de la procédure stockée.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// </summary>
        Task ExecuteAsync(string caller, string procedureName, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre générique hor int/string.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument (ex : numéro de série).</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre de façon sûre (paramétrée), sans concaténation SQL dangereuse.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via paramètres EF Core.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <exception cref="Exception">Reclassifiée via Ex_Classifier.</exception>
        Task ExecuteArg1Async(string caller, string procedureName, object arg1, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre de type <c>string</c>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument alphanumérique de type string.</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre métier/technique à une procédure stockée tout en conservant la traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Recevoir la CallChain amont via <paramref name="caller"/>.</description></item>
        /// <item><description>Exécuter la procédure stockée identifiée par <paramref name="procedureName"/>.</description></item>
        /// <item><description>Fournir la valeur du paramètre via <paramref name="arg1"/>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont (origine de l’appel).</param>
        /// <param name="procedureName">Nom SQL (schema.proc) de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteArg1StringAsync(string caller, string procedureName, string arg1, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute une procédure stockée avec 1 paramètre de type <c>int</c>.</para>
        /// <para>Contexte</para>
        /// <para>Utilisée pour les procédures attendues avec un identifiant numérique (ex : IdSerialNumber).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une exécution typée, sûre et explicite.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Valider la valeur <paramref name="arg1"/>.</description></item>
        /// <item><description>Exécuter la procédure via paramètre EF Core.</description></item>
        /// </list>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// </summary>
        Task ExecuteArg1IntAsync(string caller, string procedureName, int arg1, CancellationToken ct = default);


        #endregion
    }
}
