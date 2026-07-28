
namespace LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat d’un service Infrastructure permettant de déplacer un fichier MDB
    /// vers le répertoire des imports en échec (<c>ImportFailedDirectoryPath</c>).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé lorsque le traitement d’un fichier ne peut pas être mené à terme
    /// (erreurs d’import ligne, série invalide/inexistante, etc.) afin d’éviter
    /// un retraitement en boucle.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Isoler les fichiers non traités dans un répertoire dédié tout en garantissant
    /// un nom de destination unique et une traçabilité via CallChain.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et services batch/console du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider les paramètres (chemin fichier, raison).</description></item>
    /// <item><description>Créer le répertoire cible si nécessaire.</description></item>
    /// <item><description>Déplacer le fichier en assurant l’unicité du nom.</description></item>
    /// </list>
    /// </summary>
    public interface IS_FileMoveToFailed
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Déplace un fichier vers le répertoire "ImportFailed".</para>
        /// <para>Contexte</para>
        /// <para>Appelé lorsqu’un fichier ne doit pas rester dans le répertoire de scan.</para>
        /// <para>Objectif</para>
        /// <para>Éviter les retraitements en boucle et faciliter l’analyse des anomalies.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="filePath">Chemin complet du fichier source.</param>
        /// <param name="failedDir">Chemin complet du répertoire si non importé.</param>
        /// <param name="reason">Raison fonctionnelle/technique du déplacement (pour nommage/traçabilité).</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteAsync(string caller, string filePath, string failedDir, string reason, CancellationToken ct = default);

        #endregion
    }
}
