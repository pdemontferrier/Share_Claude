using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour l’état d’un lot de découpe à la fin de l’optimisation.
    /// </summary>
    public class SR_DecoupeLot_UpdateOptimBarNew : IS_DecoupeLot_UpdateOptimBarNew
    {
        private readonly string _callee;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IC_DecoupeLot _chDecoupeLot;

        public SR_DecoupeLot_UpdateOptimBarNew(
            IQ_DecoupeLot qhDecoupeLot,
            IC_DecoupeLot chDecoupeLot)
        {
            _callee = GetType().Name;
            _qhDecoupeLot = qhDecoupeLot;
            _chDecoupeLot = chDecoupeLot;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                var decoupeLot = await _qhDecoupeLot.HandleGetByIdAsync(decoupeLotId);
                if (decoupeLot == null)
                    throw new Ex_Business(callChain,"DLU_05", "No_Er_Bu_30");

                decoupeLot.OptimNeuf = true;
                await _chDecoupeLot.HandleUpdateAsync(decoupeLot, callChain);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}