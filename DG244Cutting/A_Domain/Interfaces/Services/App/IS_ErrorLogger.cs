using DG244Cutting.A_Domain.DTOs.App;

namespace DG244Cutting.A_Domain.Interfaces.Services.App
{
    /// <summary>
    /// Service transverse de journalisation des erreurs applicatives, opérant sur deux canaux
    /// indépendants : un fichier CSV structuré sur disque et la table <c>UserAppErrorLog</c>
    /// en base de données.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : service positionné dans B_UseCases/Services/App, implémentant
    /// <see cref="IS_ErrorLogger"/>. Il constitue le canal de persistance technique des
    /// erreurs applicatives, invocable selon deux régimes distincts.
    /// </para>
    /// <para>
    /// Régime primaire — UseCases : ce service est principalement invoqué depuis
    /// l'orchestrateur de traitement terminal des erreurs (typiquement
    /// <c>UC_LogAndNotify</c>), après que l'exception typée a remonté le pipeline complet
    /// et que le message UI traduit a été résolu via le dictionnaire multilingue.
    /// C'est le flux standard et principal de journalisation.
    /// </para>
    /// <para>
    /// Régime secondaire — Services transverses autorisés (exception architecturale
    /// documentée) : certains services transverses spécifiques dont le comportement de type
    /// best effort requiert une journalisation autonome non bloquante peuvent appeler
    /// directement ce service (ex. : <c>SR_Dictionary</c> lors d'une clé de traduction
    /// manquante). Cette exception est délibérée : elle couvre les cas où produire une trace
    /// ne doit pas interrompre le flux principal et où l'escalade vers un UseCase serait
    /// disproportionnée ou techniquement impossible.
    /// </para>
    /// <para>
    /// Exception architecturale de persistance : contrairement aux services métier standard,
    /// <c>SR_ErrorLogger</c> délègue la persistance base à <c>CH_UserAppErrorLog</c>, dont
    /// l'implémentation crée son propre DbContext de courte durée, garantissant la survie
    /// des enregistrements même en cas de rollback de la transaction principale du UseCase.
    /// </para>
    /// <para>
    /// Chemin du fichier de log : calculé à la construction par <c>BuildLogFilePath()</c>
    /// à partir des constantes internes <c>ErrorLogFolder</c> et <c>ErrorLogFileName</c>,
    /// sans dépendance envers <c>ISE_App</c>. Le nom du répertoire et du fichier de log
    /// sont des informations d'implémentation internes, non configurables, ce qui permet à
    /// <c>SR_ErrorLogger</c> d'être instancié sans accès aux interfaces de settings.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Normaliser l'exception reçue en structure interne exploitable.
    ///   </description></item>
    ///   <item><description>
    ///     Reconstruire la chaîne complète des exceptions imbriquées, bornée à
    ///     <see cref="MaxErrorDetailsLength"/> caractères.
    ///   </description></item>
    ///   <item><description>
    ///     Tenter une écriture fichier CSV en mode best effort.
    ///   </description></item>
    ///   <item><description>
    ///     Tenter une écriture en base via <see cref="IC_UserAppErrorLog"/> en mode
    ///     best effort.
    ///   </description></item>
    ///   <item><description>
    ///     Propager la CallChain à travers toutes les étapes de journalisation.
    ///   </description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Aucune traduction du message d'erreur : fourni résolu par l'appelant.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune notification à l'opérateur.
    ///   </description></item>
    ///   <item><description>
    ///     Aucune classification des exceptions brutes .NET.
    ///   </description></item>
    ///   <item><description>
    ///     Aucun calcul de chemin externe : le nom et le chemin du fichier de log sont
    ///     entièrement gérés en interne via <c>ErrorLogFileName</c> et
    ///     <c>BuildLogFilePath()</c>.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public interface IS_ErrorLogger
    {
        /// <summary>
        /// Journalise une erreur de manière robuste dans un fichier CSV structuré et,
        /// si possible, dans la base de données applicative.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Contexte : appelée soit depuis l'orchestrateur terminal des erreurs après réception
        /// d'une exception typée remontée du pipeline, soit depuis un service transverse
        /// autorisé nécessitant une journalisation autonome non bloquante. Dans le premier
        /// cas, <paramref name="errorMessage"/> contient le message principal traduit via le
        /// dictionnaire multilingue. Dans le second cas, il s'agit d'un message technique en
        /// français rédigé en dur.
        /// </para>
        /// <para>
        /// Objectif : persister une erreur de manière traçable et non bloquante, en conservant
        /// la CallChain d'origine, l'identifiant d'erreur normalisé et le détail technique
        /// complet reconstruit depuis la chaîne des exceptions imbriquées.
        /// </para>
        /// <para>
        /// Garantie best effort : cette méthode n'est jamais la source d'une interruption
        /// applicative. Toute défaillance interne est absorbée silencieusement après que
        /// <paramref name="ct"/> a été contrôlé en entrée.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour
        /// enrichissement et traçabilité.</param>
        /// <param name="appCtx">Contexte applicatif courant résolu par le composant appelant et utilisé pour
        /// enrichir l'entrée de log : application, utilisateur, poste, adresse IP et
        /// horodatage applicatif. Si <see langword="null"/>, la méthode retourne
        /// immédiatement sans erreur afin de préserver le comportement best effort.</param>
        /// <param name="errorMessage">
        /// Message principal à enregistrer dans le log. Doit être non nul et non vide.
        /// Fourni résolu (traduit ou rédigé en français) par l'appelant.
        /// </param>
        /// <param name="ex">Exception typée à journaliser. Si <see langword="null"/>, la
        /// méthode retourne immédiatement sans erreur.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière
        /// coopérative avant l'entrée dans le mode best effort interne.</param>
        /// <returns>Tâche représentant l'exécution asynchrone de la journalisation.</returns>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant l'entrée dans
        /// le mode best effort. Aucune exception n'est levée après ce point.
        /// </exception>
        Task ExecuteAsync(string caller, DTO_AppContext appCtx, string errorMessage, Exception ex, CancellationToken ct = default);
    }
}