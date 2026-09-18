using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant la mise à jour d’un lot de découpe lors de la fin d’un approvisionnement.
    /// </summary>
    public class UC_DecoupeLot_UpdateAtStockRelease : IU_DecoupeLot_UpdateAtStockRelease
    {
        private readonly string _callee;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IQ_AppContext _appContext;
        private readonly IS_DecoupeLot_UpdateAtStockRelease _decoupeLot_UpdateAtStockRelease;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Settings_UseCase _settings;
        private readonly IU_CommandeClientAction _ucCommandeClientAction;
        private readonly IU_BatchDocumentation _ucBatchDocumentation;

        public UC_DecoupeLot_UpdateAtStockRelease(
            IQ_DecoupeLot qhDecoupeLot,
            IQ_AppContext appContext,
            IS_DecoupeLot_UpdateAtStockRelease decoupeLot_UpdateAtStockRelease,
            IS_LogAndNotify logAndNotify,
            IS_Settings_UseCase settings,
            IU_CommandeClientAction ucCommandeClientAction,
            IU_BatchDocumentation ucBatchDocumentation)
        {
            _callee = GetType().Name;

            _qhDecoupeLot = qhDecoupeLot;
            _appContext = appContext;
            _decoupeLot_UpdateAtStockRelease = decoupeLot_UpdateAtStockRelease;
            _logAndNotify = logAndNotify;
            _settings = settings;
            _ucCommandeClientAction = ucCommandeClientAction;
            _ucBatchDocumentation = ucBatchDocumentation;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, bool isChute, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // Eléments d'identification de l'application'
            DTO_AppContext appCtx = _appContext.GetAppContext();

            try
            {
                // Étape 1 : Vérifier si le lot à déjà été approvisionné en barre neuves
                bool isApproNeuf = await _qhDecoupeLot.HandleCheckApproNeufAsync(decoupeLotId);

                // Etape 2 : Mettre à jour DecoupeLot pour decoupeLotId et indiquer si isChute
                await _decoupeLot_UpdateAtStockRelease.ExecuteAsync(decoupeLotId, isChute, appCtx, callChain);

                // Etape 3 : Ajouter une action à la commande client
                int actionId = isChute
                    ? _settings.GetActionBarDropStockRelease()
                    : _settings.GetActionBarNewStockRelease();

                await _ucCommandeClientAction.ExecuteAsync(actionId, !isApproNeuf, callChain);

                // Étape 4 : Mettre à jour la documentation
                if (!isApproNeuf)
                    await _ucBatchDocumentation.ExecuteAsync(decoupeLotId, callChain);
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