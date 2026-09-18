using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.B_UseCases.Services.Business
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
    public class SR_TemporImportFileImporter : IS_TemporImportFileImporter
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_MdbReader _mdbReader;
        private readonly IS_TemporRowTransformer _rowTransformer;
        private readonly IC_TemporImport _temporImportCommand;
        private readonly IC_ProductionSeries _productionSeriesCommand;
        private readonly IS_FileDelete _fileDelete;
        private readonly IS_ErrorLogger _errorLog;
        private readonly IS_FileMoveToFailed _fileMoveToFailed;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le service d’import MDB → SQL.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires à l’import, au tag applicatif et à la suppression du fichier.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// <item><description>Valider les dépendances.</description></item>
        /// </list>
        /// </summary>
        public SR_TemporImportFileImporter(
            IS_MdbReader mdbReader,
            IS_TemporRowTransformer rowTransformer,
            IC_TemporImport temporImportCommand,
            IC_ProductionSeries productionSeriesCommand,
            IS_FileDelete fileDelete,
            IS_ErrorLogger errorLog,
            IS_FileMoveToFailed fileMoveToFailed)
        {
            _callee = GetType().Name;

            _mdbReader = mdbReader ?? throw new ArgumentNullException(nameof(mdbReader));
            _rowTransformer = rowTransformer ?? throw new ArgumentNullException(nameof(rowTransformer));
            _temporImportCommand = temporImportCommand ?? throw new ArgumentNullException(nameof(temporImportCommand));
            _productionSeriesCommand = productionSeriesCommand ?? throw new ArgumentNullException(nameof(productionSeriesCommand));
            _fileDelete = fileDelete ?? throw new ArgumentNullException(nameof(fileDelete));
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
            _fileMoveToFailed = fileMoveToFailed;
        }

        #endregion

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
        public async Task ExecuteAsync(string caller, string filePath, string failedDir, int serialNumberId, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("filePath is required.", nameof(filePath));

                if (serialNumberId <= 0)
                    throw new ArgumentOutOfRangeException(nameof(serialNumberId), "serialNumberId must be > 0.");

                bool hasErrors = false;
                int okCount = 0;
                int koCount = 0;

                // Étape 1-3 : lecture streaming + transformation + insert (par ligne)
                foreach (var row in _mdbReader.StreamTableRows(callChain, filePath, "Tempor"))
                {
                    ct.ThrowIfCancellationRequested();

                    row.TryGetValue("SerieNr", out string? serieNrRaw);
                    string? feld_10_036 = TryGetFeld10_036(row);

                    try
                    {
                        var entity = _rowTransformer.Transform(row);
                        await _temporImportCommand.HandleAddAsync(callChain, entity, false);

                        okCount++;
                    }
                    catch (Exception ex)
                    {
                        hasErrors = true;
                        koCount++;

                        string dbDetails = BuildDbErrorDetails(ex);

                        var logEx = new Ex_Infrastructure(
                            $"Row import failed. File='{filePath}', SerieNr='{serieNrRaw}', Feld_10_036='{feld_10_036}'. DB={dbDetails}",
                            ex);

                        await _errorLog.ExecuteAsync(callChain, logEx, ct);

                        continue;
                    }
                }

                // Étape 4-5 : uniquement si 100% OK
                if (!hasErrors)
                {
                    await _productionSeriesCommand.HandleSetIsImportedTrueAsync(callChain, serialNumberId, ct);
                    await _fileDelete.ExecuteAsync(callChain, filePath, ct);
                }
                else
                {
                    // On garde le fichier (pas de delete) et on ne tag pas la série
                    await _errorLog.ExecuteAsync(
                        callChain,
                        new Ex_Infrastructure($"Import finished with errors. File kept. ok={okCount}, ko={koCount}, File='{filePath}'."),
                        ct);

                    // ACE/OleDb peut conserver un lock un court instant après Dispose()
                    // (surtout sur .mdb). Petite pause pour éviter le sharing violation.
                    await Task.Delay(150, ct);

                    // Déplacement pour éviter le retraitement en boucle
                    await _fileMoveToFailed.ExecuteAsync(callChain, filePath, failedDir, reason: "ImportRowError", ct);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        private static string? TryGetFeld10_036(IDictionary<string, string> row)
        {
            if (!row.TryGetValue("Feld_10", out var raw) || string.IsNullOrEmpty(raw))
                return null;

            // Feld_10_036 => zone 36 => index 35
            const int targetIndex = 35;

            var parts = raw.Split('|');
            if (targetIndex < 0 || targetIndex >= parts.Length)
                return null;

            return parts[targetIndex];
        }

        private static string BuildDbErrorDetails(Exception ex)
        {
            var root = ex.GetBaseException();

            if (ex is DbUpdateException dbu && dbu.InnerException != null)
                root = dbu.InnerException.GetBaseException();

            if (root is SqlException sql)
                return $"SqlException(Number={sql.Number}, State={sql.State}, Line={sql.LineNumber}, Proc={sql.Procedure}) : {sql.Message}";

            return $"{root.GetType().Name} : {root.Message}";
        }

        #endregion
    }
}
