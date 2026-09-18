using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase chargé d’optimiser et d’allouer les barres de chute à découper pour un lot donné.
    /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
    /// </summary>
    public class UC_BarDropOptim : IU_BarDropOptim
    {
        private readonly string _callee;
        private readonly IS_DecoupeDetail_MachineList _machineListService;
        private readonly IS_DecoupeBarre_OptimBarDrop _optimBarService;
        private readonly IS_DecoupeDetail_UpdateIndice2 _updateIndice2Service;
        private readonly IS_LogAndNotify _logAndNotify;

        public UC_BarDropOptim(
            IS_DecoupeDetail_MachineList machineListService,
            IS_DecoupeBarre_OptimBarDrop optimBarService,
            IS_DecoupeDetail_UpdateIndice2 updateIndice2Service,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;
            _machineListService = machineListService;
            _optimBarService = optimBarService;
            _updateIndice2Service = updateIndice2Service;
            _logAndNotify = logAndNotify;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : Obtenir la liste des machines à approvisionner
                List<string> machineList = await _machineListService.ExecuteAsync(decoupeLotId, callChain);

                if (machineList == null || !machineList.Any())
                    throw new Ex_Business(callChain, "OPT_01", "No_Er_Bu_42");

                // Étape 2 : Optimisation par machine
                foreach (var machineId in machineList)
                {
                    await _optimBarService.ExecuteAsync(decoupeLotId, machineId, callChain);
                }

                // Étape 3 : Mise à jour des lignes indice 2
                await _updateIndice2Service.ExecuteAsync(decoupeLotId, callChain);
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