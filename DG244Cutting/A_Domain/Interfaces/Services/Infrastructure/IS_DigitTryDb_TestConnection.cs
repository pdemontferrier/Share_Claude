namespace DG244Cutting.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat d'un service de diagnostic de connectivité à la base de données SQL Server
    /// portée par le DbContext applicatif, dans le cadre de l'informatique de production
    /// du projet DG244Cutting.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Cette interface est implémentée par <c>SR_DigitTryDb_TestConnection</c> dans la couche
    /// <c>C_Infrastructure</c>. Elle est consommée par injection de dépendances par les
    /// UseCases orchestrateurs portant un test de connectivité (typiquement le UseCase
    /// orchestrateur du Jalon 3 de la séquence de démarrage applicatif - §3.10 du 0230 -
    /// ainsi que le UseCase orchestrateur du re-test périodique invoqué via
    /// <c>SR_UseCaseInvoker</c> par un event MainWindow - EA-11).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Exposer, sous une forme abstraite et stable, le besoin d'un test unitaire et
    /// réutilisable de connectivité à la base SQL, sans portage d'information enrichie
    /// (pas de DTO, pas de latence, pas de message d'erreur) ; le résultat est strictement
    /// binaire.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases orchestrateurs (chaîne (1) au sens de §4.14.9).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Déclarer l'opération unitaire de test de connectivité à la base.</description></item>
    /// <item><description>Imposer la propagation de la CallChain via le paramètre <c>caller</c> contractuel.</description></item>
    /// <item><description>Imposer le support de l'annulation coopérative via un <c>CancellationToken</c>.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Déclarer la frontière de retour <see langword="bool"/> matérialisant le résultat binaire du diagnostic.</description></item>
    /// <item><description>Documenter les exceptions techniques susceptibles d'être levées par requalification terminale de l'implémentation.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    /// <item><description>Ne porte aucune logique d'orchestration ni de transaction : ces rôles relèvent du UseCase appelant.</description></item>
    /// <item><description>Ne notifie pas l'état de connexion à <c>IS_AppContext</c> ni à aucun autre composant : cette responsabilité relève du UseCase orchestrateur.</description></item>
    /// <item><description>Ne porte aucune information enrichie (pas de latence, pas de message d'erreur, pas de DTO).</description></item>
    /// </list>
    /// </remarks>
    /// <seealso cref="DG244Cutting.C_Infrastructure.Services.SR_DigitTryDb_TestConnection"/>
    public interface IS_DigitTryDb_TestConnection
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute un test unitaire de connectivité à la base de données SQL Server.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Appelée par un UseCase orchestrateur (typiquement <c>UC_Application_OnStart</c> au
        /// Jalon 3 de la séquence de démarrage applicatif, ou un UseCase de re-test périodique
        /// invoqué via <c>SR_UseCaseInvoker</c>). L'implémentation utilise un DbContext de
        /// courte durée produit par <c>IDbContextFactory&lt;DigitTryDbContext&gt;</c>,
        /// indépendant de tout scope UseCase.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Retourner un résultat binaire : <see langword="true"/> si la connexion à la base
        /// est établie, <see langword="false"/> en cas d'indisponibilité non exceptionnelle
        /// (retour négatif de la primitive EF Core <c>Database.CanConnectAsync</c>). Toute
        /// défaillance technique non prévue (exception EF Core ou tierce) est requalifiée
        /// et propagée à l'appelant.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Enrichir la <c>CallChain</c> reçue en première position.</description></item>
        /// <item><description>Tester la connectivité via la primitive EF Core <c>Database.CanConnectAsync</c>.</description></item>
        /// <item><description>Propager le résultat binaire à l'appelant, ou requalifier toute exception non prévue.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">
        /// Chaîne d'appel reçue de l'appelant, propagée selon le format normatif
        /// <c>{caller} &gt; {_callee} &gt; {nameof(ExecuteAsync)}</c> (§4.5 du 0230).
        /// </param>
        /// <param name="ct">Jeton d'annulation coopérative. Par défaut <see langword="default"/>.</param>
        /// <returns>
        /// <see langword="true"/> si la connexion à la base SQL est établie ;
        /// <see langword="false"/> si elle ne l'est pas et que la primitive EF Core retourne
        /// négativement sans lever d'exception.
        /// </returns>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Infrastructure">
        /// Levée par requalification terminale via <c>IS_ExClassifier</c> en cas de défaillance
        /// technique identifiable (échec de connexion EF Core, SqlException, DbException...).
        /// </exception>
        /// <exception cref="DG244Cutting.A_Domain.Common.Exceptions.Ex_Unclassified">
        /// Levée par requalification terminale via <c>IS_ExClassifier</c> en cas d'exception non
        /// prévue ne relevant pas d'une défaillance technique classifiable.
        /// </exception>
        /// <exception cref="System.OperationCanceledException">
        /// Levée si <paramref name="ct"/> est annulé avant ou pendant l'exécution. Aucune
        /// requalification : remontée stricte à l'appelant conformément à la priorité d'annulation
        /// coopérative (R-4.6.13 du 0231).
        /// </exception>
        Task<bool> ExecuteAsync(string caller, CancellationToken ct = default);

        #endregion
    }
}