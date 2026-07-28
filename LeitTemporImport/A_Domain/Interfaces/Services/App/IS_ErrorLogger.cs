
namespace LeitTemporImport.A_Domain.Interfaces.Services.App
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de service de journalisation des erreurs pour l’application console.
    /// Enregistre les erreurs dans un fichier CSV structuré et tente, si possible, un enregistrement en base.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// L’application ne dispose pas de mécanisme de notification (pas de <c>IS_LogAndNotify</c>).
    /// La stratégie retenue est donc exclusivement la journalisation (fichier + base).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la journalisation des exceptions avec traçabilité (CallChain) et robustesse.
    /// Un logger ne doit jamais faire échouer le traitement principal : best effort.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et Services applicatifs du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Normaliser l’exception (Business / Infrastructure / Unknown).</description></item>
    /// <item><description>Écrire une ligne CSV dans le fichier de log.</description></item>
    /// <item><description>Tenter l’enregistrement en base.</description></item>
    /// </list>
    /// </summary>
    public interface IS_ErrorLogger
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Journalise une exception de manière robuste (CSV + tentative base).</para>
        /// <para>Contexte</para>
        /// <para>Appelée dans les blocs catch des UseCases et Services.</para>
        /// <para>Objectif</para>
        /// <para>Persister l’erreur avec CallChain afin de permettre le diagnostic et la traçabilité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Récupérer le contexte applicatif (AppId, user, device).</description></item>
        /// <item><description>Normaliser les informations d’erreur.</description></item>
        /// <item><description>Écrire dans le fichier CSV.</description></item>
        /// <item><description>Tenter un enregistrement en base si disponible.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="ex">Exception à journaliser.</param>
        /// <param name="ct">Token d’annulation.</param>
        Task ExecuteAsync(string caller, Exception ex, CancellationToken ct = default);

        #endregion
    }
}
