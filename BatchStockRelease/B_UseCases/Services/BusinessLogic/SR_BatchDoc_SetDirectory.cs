using System.IO;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Implémentation du service de préparation du répertoire pour un lot de découpe.
    /// </summary>
    public class SR_BatchDoc_SetDirectory : IS_BatchDoc_SetDirectory
    {
        private readonly string _callee;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_UseCase _settingsUseCase;
        private readonly IQ_DecoupeLot _qhDecoupeLot;

        public SR_BatchDoc_SetDirectory(
            IS_Settings_App settingsApp,
            IS_Settings_UseCase settingsUseCase, 
            IQ_DecoupeLot qhDecoupeLot)
        {
            _settingsApp = settingsApp;
            _settingsUseCase = settingsUseCase;
            _qhDecoupeLot = qhDecoupeLot;
            _callee = GetType().Name;
        }

        // ---------------- GARDER CE SERVICE EN LOCAL ----------------------

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Récupérer les informations du lot
                var batch = await _qhDecoupeLot.HandleGetByIdAsync(decoupeLotId);
                if (batch == null)
                    throw new Ex_Business(callChain,"DOC_01", "No_Er_Bu_30");

                // Mettre à jour AppSettings.batchSelectedName
                _settingsUseCase.SetDecoupeLotId(batch.Id);
                _settingsUseCase.SetDecoupeLotDesignation(batch.Designation ?? string.Empty);
                _settingsUseCase.SetDecoupeLotEcheance(batch.IdEcheance);
                _settingsUseCase.SetDecoupeLotCouleur(batch.IdCouleur);

                string fileName1 = $"{batch.IdEcheance}_{batch.IdCouleur.Replace(" ", "")}_Lot_{batch.Id:D4}";
                _settingsUseCase.SetBatchFileName1(fileName1);

                string batchDirectoryPath = Path.Combine(
                    _settingsApp.GetDirectoryPathServer2(),
                    _settingsApp.GetDirectoryPathLotsDeFabrication(),
                    fileName1);

                // Supprimer l'ancien dossier s’il existe
                if (Directory.Exists(batchDirectoryPath))
                    await Task.Run(() => Directory.Delete(batchDirectoryPath, true));

                // Créer le nouveau dossier
                await Task.Run(() => Directory.CreateDirectory(batchDirectoryPath));
                _settingsUseCase.SetDirectoryPathBatch(batchDirectoryPath);

                // Copier le fichier modèle
                string sourceFilePath = _settingsApp.GetDirectoryPathBatchModel();
                if (!File.Exists(sourceFilePath))
                    throw new Ex_Business(callChain,"DOC_02", "No_Er_Bu_54");

                string newFileName = $"{fileName1}{_settingsUseCase.GetBatchFileName2()}{Path.GetExtension(sourceFilePath)}";
                string destFilePath = Path.Combine(batchDirectoryPath, newFileName);

                await Task.Run(() => File.Copy(sourceFilePath, destFilePath, true));
                _settingsUseCase.SetDirectoryPathBatchFile(destFilePath); // DirectoryPathBatchFile
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}