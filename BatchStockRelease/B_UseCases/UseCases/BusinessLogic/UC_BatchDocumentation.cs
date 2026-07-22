using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant la génération des fichiers de documentation pour un lot de découpe.
    /// </summary>
    public class UC_BatchDocumentation : IU_BatchDocumentation
    {
        private readonly string _callee;
        private readonly IS_BatchDoc_SetDirectory _setDirectoryService;
        private readonly IS_BatchDoc_AddDecoupeDetailToExcel _addDetailToExcelService;
        private readonly IS_BatchDoc_AddDecoupeBarreToExcel _addBarreToExcelService;
        private readonly IS_BatchDoc_AddEluFiles _addEluFilesService;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_UseCase _settingsUseCase;

        public UC_BatchDocumentation(
            IQ_AppContext appContext,
            IS_BatchDoc_SetDirectory setDirectoryService,
            IS_BatchDoc_AddDecoupeDetailToExcel addDetailToExcelService,
            IS_BatchDoc_AddDecoupeBarreToExcel addBarreToExcelService,
            IS_BatchDoc_AddEluFiles addEluFilesService,
            IS_LogAndNotify logAndNotify,
            IS_Settings_App settingsApp,
            IS_Settings_UseCase settings)
        {
            _callee = GetType().Name;

            _setDirectoryService = setDirectoryService;
            _addDetailToExcelService = addDetailToExcelService;
            _addBarreToExcelService = addBarreToExcelService;
            _addEluFilesService = addEluFilesService;
            _logAndNotify = logAndNotify;
            _settingsApp = settingsApp;
            _settingsUseCase = settings;
        }

        /// <inheritdoc/>
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : Préparer le répertoire du lot
                await _setDirectoryService.ExecuteAsync(decoupeLotId, callChain);

                // Eléments de paramétrage
                string filePath = _settingsUseCase.GetDirectoryPathBatchFile() ?? string.Empty;
                string batchFileSheet1 = _settingsUseCase.GetBatchFileSheet1() ?? string.Empty;
                string batchFileSheet2 = _settingsUseCase.GetBatchFileSheet2() ?? string.Empty;
                string decoupeLotDesignation = _settingsUseCase.GetDecoupeLotDesignation() ?? string.Empty;
                string batchDirectoryPath = _settingsUseCase.GetDirectoryPathBatch() ?? string.Empty;
                string batchFileName = _settingsUseCase.GetBatchFileName1() ?? string.Empty;
                string directoryPathServer2 = _settingsApp.GetDirectoryPathServer2();
                string directoryPathElumatec = _settingsApp.GetDirectoryPathElumatec();
                string directoryPathElumatec1 = _settingsApp.GetDirectoryPathElumatec1();
                string directoryPathElumatec2 = _settingsApp.GetDirectoryPathElumatec2();
                string directoryPathElumatec3 = _settingsApp.GetDirectoryPathElumatec3();

                // Étape 2 : Ajouter les données de DecoupeDetail dans le fichier Excel
                await _addDetailToExcelService.ExecuteAsync(decoupeLotId, filePath, batchFileSheet1, decoupeLotDesignation, callChain);

                // Étape 3 : Ajouter les données de DecoupeBarre dans le fichier Excel
                await _addBarreToExcelService.ExecuteAsync(decoupeLotId, filePath, batchFileSheet2, decoupeLotDesignation, callChain);

                // Étape 4 : Générer les fichiers .elu pour les machines Elumatec
                await _addEluFilesService.ExecuteAsync(decoupeLotId, batchDirectoryPath, batchFileName, directoryPathServer2, directoryPathElumatec, directoryPathElumatec1, directoryPathElumatec2, directoryPathElumatec3, callChain);
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }
    }
}