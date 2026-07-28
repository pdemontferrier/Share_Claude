namespace LeitTemporImport.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// UseCase orchestrateur chargé de parcourir un répertoire de travail contenant des fichiers MDB
    /// (ex: Leitxxxx.mdb) et de déclencher le traitement métier pour chaque fichier via
    /// <c>IU_TemporImport_ProcessFile</c>.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Exécuté en mode batch (console) à intervalles réguliers (ex : toutes les 15 minutes) afin de
    /// traiter les nouveaux fichiers présents dans le répertoire configuré.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir un traitement robuste : un fichier en erreur ne doit pas empêcher le traitement des suivants.
    /// Les erreurs sont journalisées via <c>IS_ErrorLogger</c>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Traitements batch / console d’import MDB → SQL Server 2019 (projet 104).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Scanner le répertoire pour trouver les fichiers MDB correspondants.</description></item>
    /// <item><description>Ordonner la liste pour un traitement déterministe.</description></item>
    /// <item><description>Traiter chaque fichier via <c>UC_TemporImport_ProcessFile</c>.</description></item>
    /// <item><description>Logger les erreurs et poursuivre sur le fichier suivant.</description></item>
    /// </list>
    /// </summary>
    public interface IU_TemporImport
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute l’orchestration batch de traitement de tous les fichiers MDB détectés.</para>
        /// <para>Contexte</para>
        /// <para>Déclenché périodiquement par l’application console.</para>
        /// <para>Objectif</para>
        /// <para>Traiter chaque fichier de manière indépendante : erreur sur un fichier ⇒ log ⇒ continuer.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Scanner le répertoire de travail.</description></item>
        /// <item><description>Traiter les fichiers un par un via <c>IU_TemporImport_ProcessFile</c>.</description></item>
        /// <item><description>Logger les exceptions sans interrompre la boucle.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteAsync(string caller, CancellationToken ct = default);
    }
}
