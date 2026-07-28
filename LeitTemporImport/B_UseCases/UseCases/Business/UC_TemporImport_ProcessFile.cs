using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.App;
using LeitTemporImport.A_Domain.Interfaces.Services.Business;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;
using LeitTemporImport.A_Domain.Interfaces.Settings.Business;
using LeitTemporImport.A_Domain.Interfaces.UseCases.Business;
using System.Globalization;


namespace LeitTemporImport.B_UseCases.UseCases.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// UseCase orchestrateur chargé de traiter un fichier MDB unique. Il lit la table Tempor,
    /// récupère le numéro de série (SerieNr) sur le premier enregistrement, le stocke dans SE_Business,
    /// puis consulte ProductionSeries afin de décider si le fichier doit être supprimé.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Exécuté par le UseCase parent UC_TemporImport qui itère sur les fichiers présents dans le répertoire.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Appliquer la logique métier de filtrage et de traitement : supprimer les fichiers inutiles,
    /// importer et tagger les séries non importées, puis consolider la base via procédures stockées.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Traitements batch / console d’import MDB → SQL Server.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire SerieNr dans Tempor (1ère ligne).</description></item>
    /// <item><description>Mettre à jour SE_Business.SerialNumberId (int).</description></item>
    /// <item><description>Lire ProductionSeries.IsImported pour la série.</description></item>
    /// <item><description>Supprimer le fichier si déjà importé.</description></item>
    /// </list>
    /// </summary>
    public class UC_TemporImport_ProcessFile : IU_TemporImport_ProcessFile
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly ISE_Business _business;
        private readonly IS_MdbReader _mdbReader;
        private readonly IS_StoredProcedure _storedProcedure;
        private readonly IS_ProductionSeriesReader _productionSeriesReader;
        private readonly IS_FileDelete _fileDelete;
        private readonly IS_TemporImportFileImporter _temporImportFileImporter;
        private readonly IS_PostImportSeriesUpdater _postImportSeriesUpdater;
        private readonly IS_ErrorLogger _errorLog;
        private readonly IS_FileMoveToFailed _fileMoveToFailed;
        private readonly IS_LifecycleActionAdd _lifecycleActionAdd;


        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le UseCase de traitement d’un fichier MDB.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser les dépendances nécessaires à la lecture MDB, la consultation SQL et la suppression de fichier.</para>
        /// </summary>
        public UC_TemporImport_ProcessFile(
            ISE_Business business,
            IS_MdbReader mdbReader,
            IS_StoredProcedure storedProcedure,
            IS_ProductionSeriesReader productionSeriesReader,
            IS_TemporImportFileImporter temporImportFileImporter,
            IS_FileDelete fileDelete,
            IS_PostImportSeriesUpdater postImportSeriesUpdater,
            IS_ErrorLogger errorLog,
            IS_FileMoveToFailed fileMoveToFailed,
            IS_LifecycleActionAdd lifecycleActionAdd)
        {
            _callee = GetType().Name;

            _business = business ?? throw new ArgumentNullException(nameof(business));
            _mdbReader = mdbReader ?? throw new ArgumentNullException(nameof(mdbReader));
            _storedProcedure = storedProcedure ?? throw new ArgumentNullException(nameof(storedProcedure));
            _productionSeriesReader = productionSeriesReader ?? throw new ArgumentNullException(nameof(productionSeriesReader));
            _fileDelete = fileDelete ?? throw new ArgumentNullException(nameof(fileDelete));
            _temporImportFileImporter = temporImportFileImporter ?? throw new ArgumentNullException(nameof(temporImportFileImporter));
            _postImportSeriesUpdater = postImportSeriesUpdater ?? throw new ArgumentNullException(nameof(postImportSeriesUpdater));
            _errorLog = errorLog ?? throw new ArgumentNullException(nameof(errorLog));
            _fileMoveToFailed = fileMoveToFailed ?? throw new ArgumentNullException(nameof(fileMoveToFailed));
            _lifecycleActionAdd = lifecycleActionAdd ?? throw new ArgumentNullException(nameof(lifecycleActionAdd));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Traite un fichier MDB : lit SerieNr, met à jour SE_Business, consulte ProductionSeries et supprime si déjà importé.</para>
        /// <para>Contexte</para>
        /// <para>Appelé depuis UC_TemporImport pour chaque fichier détecté.</para>
        /// <para>Objectif</para>
        /// <para>Éviter de retraiter une série déjà importée et nettoyer les fichiers obsolètes.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Lire SerieNr depuis Tempor (TOP 1).</description></item>
        /// <item><description>Convertir en int et stocker dans SE_Business.SerialNumberId.</description></item>
        /// <item><description>Lire IsImported en base SQL.</description></item>
        /// <item><description>Supprimer le fichier si IsImported = true.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="filePath">Chemin complet du fichier MDB.</param>
        /// <param name="failedDir">Chemin complet du répertoire si non importé.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        public async Task ExecuteAsync(string caller, string filePath, string failedDir, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("filePath is required.", nameof(filePath));

                // 1) Mise à jour ProductionSeries (avant vérifications)
                await _storedProcedure.ExecuteAsync(callChain, "dbo.spr_ProductionSeries_SyncFromSource", ct);

                // 2) Lire SerieNr depuis MDB (TOP 1)
                string serieNrRaw = await _mdbReader.ReadSerieNrAsync(callChain, filePath, ct);

                // 3) Si erreur technique déjà loggée => déplacer et STOP
                if (serieNrRaw == "0")
                {
                    // Déplacer le fichier dans le répertoire Import_Failed_mdb
                    await _fileMoveToFailed.ExecuteAsync(callChain, filePath, failedDir, reason: "Invalid_SerieNr", ct);
                    return;
                }

                // 4) Conversion sûre (garantie valide si != "0")
                int serialNumberId = int.Parse( serieNrRaw, NumberStyles.Integer, CultureInfo.InvariantCulture);
                _business.SerialNumberId = serialNumberId;

                // 5) Lire IsImported en SQL
                bool? isImported = await _productionSeriesReader.GetIsImportedAsync(callChain, serialNumberId, ct);

                // 6) Si pas trouvé ou Si importé => supprimer fichier
                if (isImported is null || isImported.Value)
                {
                    await _fileDelete.ExecuteAsync(callChain, filePath, ct);
                    return;
                }

                // 7) Import applicatif (Tempor -> Tempor_Import), tag IsImported=true, suppression fichier si OK
                await _temporImportFileImporter.ExecuteAsync(callChain, filePath, failedDir, serialNumberId, ct);

                // 8) Post-import DB updates (procédures stockées, séquentiel, log-only)
                bool postImportOk = await _postImportSeriesUpdater.ExecuteAsync(callChain, serialNumberId, ct);

                // Journaliser un avertissement si postImportOk == false, ici on souhaite juste tracer un état global
                if (!postImportOk)
                {
                    await _errorLog.ExecuteAsync(
                        $"{callChain} > {nameof(IS_PostImportSeriesUpdater)}",
                        new Ex_Business(
                            callChain: callChain,
                            errorId: "No_EC_PostImport_Failed",
                            errorException: $"Post-import chain failed for SerialNumberId={serialNumberId}."),
                        ct);
                    return;
                }

                // 9) Ajouter une action au cycle de vie
                await _lifecycleActionAdd.ExecuteProductionSeriesImportedAsync(callChain, serialNumberId, filePath, ct);

            }
            catch (Exception ex)
            {
                // Le logger normalise déjà Business/Infrastructure/Unknown.
                await _errorLog.ExecuteAsync(callChain, ex, ct);

                // Choix batch : ne pas interrompre l’itération globale.
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}