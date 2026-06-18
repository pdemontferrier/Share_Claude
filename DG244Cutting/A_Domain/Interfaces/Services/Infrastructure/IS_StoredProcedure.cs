using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// Contrat d'un service technique transverse d'exécution de procédures stockées SQL Server,
    /// résidant en <c>A_Domain/Interfaces/Services/Infrastructure/</c> et implémenté en
    /// <c>C_Infrastructure/Services/</c>.
    /// </summary>
    /// <remarks>
    /// <para>Contexte</para>
    /// <para>
    /// Ce contrat est consommé par les UseCases orchestrateurs d'import et de synchronisation
    /// lorsque certaines étapes de traitement sont implémentées côté base via des procédures
    /// stockées (consolidation, synchronisation, post-traitements). Son implémentation concrète
    /// est <see cref="DG244Cutting.C_Infrastructure.Services.SR_StoredProcedure"/> en <c>C_Infrastructure/Services/</c>, résolue par
    /// injection de dépendances.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une API stable et homogène pour déclencher des procédures stockées SQL Server,
    /// avec ou sans paramètre, sans exposer les détails techniques d'accès aux données aux
    /// couches supérieures et en garantissant la traçabilité de bout en bout via la CallChain.
    /// </para>
    /// <para>Responsabilités</para>
    /// <list type="bullet">
    /// <item><description>Exécuter une procédure stockée whitelistée, sans paramètre ou avec un paramètre paramétré (object, string, int).</description></item>
    /// <item><description>Propager la CallChain reçue à chaque appel et la transmettre au pipeline de requalification d'exceptions.</description></item>
    /// <item><description>Lever <see cref="Ex_Business"/> typée en cas de violation des préconditions structurelles (nom de procédure null/vide/non whitelisté, argument null/vide/hors plage).</description></item>
    /// <item><description>Laisser remonter sans interception <see cref="OperationCanceledException"/> lorsque l'annulation coopérative est signalée.</description></item>
    /// </list>
    /// <para>Non-responsabilités</para>
    /// <list type="bullet">
    /// <item><description>Ne pilote aucune transaction (ni ouverture, ni validation, ni annulation) — la transaction relève exclusivement du UseCase orchestrateur.</description></item>
    /// <item><description>N'orchestre aucun scénario par appel d'un autre Service applicatif.</description></item>
    /// <item><description>N'invoque aucun Repository ni Command Handler en aval — l'exécution SQL passe par EF Core via <c>ExecuteSqlRawAsync</c>.</description></item>
    /// <item><description>Ne journalise pas et ne notifie pas directement — la requalification d'exception relève du pipeline de classifier.</description></item>
    /// <item><description>N'exécute aucune procédure hors whitelist contrôlée par le composant d'implémentation.</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.C_Infrastructure.Services.SR_StoredProcedure"/>
    public interface IS_StoredProcedure
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// Exécute une procédure stockée sans paramètre.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Appelée par un UseCase orchestrateur d'import ou de synchronisation.</para>
        /// <para>Objectif</para>
        /// <para>Déclencher un traitement SQL côté serveur en garantissant sécurité (whitelist) et traçabilité (CallChain).</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Enrichir la CallChain reçue via <paramref name="caller"/>.</description></item>
        /// <item><description>Valider le nom de procédure via whitelist.</description></item>
        /// <item><description>Exécuter la procédure stockée identifiée par <paramref name="procedureName"/>.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>N'initie aucune transaction.</description></item>
        /// <item><description>Ne journalise ni ne notifie directement.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité amont (origine de l'appel).</param>
        /// <param name="procedureName">Nom SQL (schema.proc) de la procédure stockée whitelistée.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_01</c> si <paramref name="procedureName"/> est null, vide ou non autorisé par la whitelist.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée en cas de défaillance technique de l'aval EF Core / SQL Server après requalification par le classifier.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteAsync(string caller, string procedureName, CancellationToken ct = default);

        /// <summary>
        /// Exécute une procédure stockée avec un paramètre générique (hors int et string).
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument typé hors des deux surcharges spécialisées <c>ExecuteArg1StringAsync</c> / <c>ExecuteArg1IntAsync</c>.</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre de façon sûre (paramétrée), sans concaténation SQL.</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Enrichir la CallChain reçue via <paramref name="caller"/>.</description></item>
        /// <item><description>Valider le nom de procédure via whitelist et la non-nullité de <paramref name="arg1"/>.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via les paramètres EF Core.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>N'initie aucune transaction.</description></item>
        /// <item><description>Ne journalise ni ne notifie directement.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée whitelistée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_01</c> si <paramref name="procedureName"/> est null/vide/non whitelisté, ou si <paramref name="arg1"/> est null.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée en cas de défaillance technique de l'aval EF Core / SQL Server après requalification par le classifier.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteArg1Async(string caller, string procedureName, object arg1, CancellationToken ct = default);

        /// <summary>
        /// Exécute une procédure stockée avec un paramètre de type <c>string</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Utilisée lorsque la procédure attend un argument alphanumérique de type chaîne.</para>
        /// <para>Objectif</para>
        /// <para>Passer un paramètre métier ou technique à une procédure stockée de façon sûre (paramétrée) tout en conservant la traçabilité.</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Enrichir la CallChain reçue via <paramref name="caller"/>.</description></item>
        /// <item><description>Valider le nom de procédure via whitelist et la non-vacuité de <paramref name="arg1"/>.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via les paramètres EF Core.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>N'initie aucune transaction.</description></item>
        /// <item><description>Ne journalise ni ne notifie directement.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée whitelistée.</param>
        /// <param name="arg1">Valeur du premier paramètre.</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_01</c> si <paramref name="procedureName"/> est null/vide/non whitelisté, ou si <paramref name="arg1"/> est null ou vide.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée en cas de défaillance technique de l'aval EF Core / SQL Server après requalification par le classifier.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteArg1StringAsync(string caller, string procedureName, string arg1, CancellationToken ct = default);

        /// <summary>
        /// Exécute une procédure stockée avec un paramètre de type <c>int</c>.
        /// </summary>
        /// <remarks>
        /// <para>Contexte</para>
        /// <para>Utilisée pour les procédures attendues avec un identifiant numérique strictement positif (ex. <c>IdSerialNumber</c>).</para>
        /// <para>Objectif</para>
        /// <para>Fournir une exécution typée, sûre et explicite.</para>
        /// <para>Responsabilités</para>
        /// <list type="bullet">
        /// <item><description>Enrichir la CallChain reçue via <paramref name="caller"/>.</description></item>
        /// <item><description>Valider le nom de procédure via whitelist et la stricte positivité de <paramref name="arg1"/>.</description></item>
        /// <item><description>Exécuter <c>EXEC Proc @p0</c> via les paramètres EF Core.</description></item>
        /// </list>
        /// <para>Non-responsabilités</para>
        /// <list type="bullet">
        /// <item><description>N'initie aucune transaction.</description></item>
        /// <item><description>Ne journalise ni ne notifie directement.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="procedureName">Nom de la procédure stockée whitelistée.</param>
        /// <param name="arg1">Valeur du premier paramètre (identifiant numérique strictement positif).</param>
        /// <param name="ct">Token d'annulation coopérative.</param>
        /// <returns>Tâche asynchrone.</returns>
        /// <exception cref="Ex_Business">
        /// Code <c>BU_ER_01</c> si <paramref name="procedureName"/> est null/vide/non whitelisté.
        /// Code <c>BU_ER_02</c> si <paramref name="arg1"/> est inférieur ou égal à zéro.
        /// </exception>
        /// <exception cref="Ex_Infrastructure">Levée en cas de défaillance technique de l'aval EF Core / SQL Server après requalification par le classifier.</exception>
        /// <exception cref="OperationCanceledException">Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.</exception>
        Task ExecuteArg1IntAsync(string caller, string procedureName, int arg1, CancellationToken ct = default);

        #endregion
    }
}