
namespace LeitTemporImport.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service d’import d’un fichier MDB : lecture de la table <c>Tempor</c>, transformation en entités
    /// <c>Tempor_Import</c>, persistance SQL via <c>IC_TemporImport</c>, puis marquage applicatif de la série
    /// comme importée et suppression du fichier en cas de succès.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé par <c>UC_TemporImport_ProcessFile</c> lorsque la série existe et n’est pas importée.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Isoler la responsabilité "importer un fichier" (MDB → SQL) en garantissant que le fichier n’est
    /// supprimé que si l’import et le marquage applicatif sont réussis.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases batch / console du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire les lignes de la table Tempor.</description></item>
    /// <item><description>Transformer chaque ligne en <c>Tempor_Import</c>.</description></item>
    /// <item><description>Persister en base via <c>IC_TemporImport</c>.</description></item>
    /// <item><description>Tagger <c>ProductionSeries.IsImported</c> à true via <c>IC_ProductionSeries</c>.</description></item>
    /// <item><description>Supprimer le fichier MDB si toutes les étapes ont réussi.</description></item>
    /// </list>
    /// </summary>
    public interface IS_TemporImportFileImporter
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Importe un fichier MDB (Tempor → Tempor_Import), marque la série comme importée (process applicatif),
        /// puis supprime le fichier si toutes les étapes ont réussi.
        /// </para>
        /// <para>Contexte</para>
        /// <para>Appelé par le UseCase de traitement d’un fichier, après validation que la série existe et n’est pas importée.</para>
        /// <para>Objectif</para>
        /// <para>Garantir un traitement atomique côté application : import + tag applicatif + suppression fichier.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Lire la table Tempor.</description></item>
        /// <item><description>Transformer en entités Tempor_Import.</description></item>
        /// <item><description>Insérer en base via IC_TemporImport.</description></item>
        /// <item><description>(Étape 4) Tagger ProductionSeries.IsImported à true via IC_ProductionSeries.</description></item>
        /// <item><description>Supprimer le fichier MDB.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour traçabilité.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <param name="failedDir">Chemin complet du répertoire si non importé.</param>
        /// <param name="serialNumberId">Identifiant numérique métier de la série (SerieNr) déjà validé.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteAsync(string caller, string filePath, string failedDir, int serialNumberId, CancellationToken ct = default);

        #endregion
    }
}
