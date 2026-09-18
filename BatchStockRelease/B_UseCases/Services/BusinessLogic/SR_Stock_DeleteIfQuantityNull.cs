using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de supprimer un enregistrement de stock si sa quantité est égale à zéro.
    /// </summary>
    public class SR_Stock_DeleteIfQuantityNull : IS_Stock_DeleteIfQuantityNull
    {
        private readonly string _callee;
        private readonly IC_Stock _chStock;
        private readonly IQ_Stock _qhStock;

        public SR_Stock_DeleteIfQuantityNull(IC_Stock chStock, IQ_Stock qhStock)
        {
            _callee = GetType().Name;
            _chStock = chStock;
            _qhStock = qhStock;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int stockId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (stockId <= 0)
                    throw new Ex_Business(callChain,"STK_09", "No_Er_Bu_26");

                var stockRecord = await _qhStock.HandleGetByIdAsync(stockId);

                if (stockRecord == null)
                    throw new Ex_Infrastructure(callChain, "STK_10", "No_Er_Bu_27");

                if (stockRecord.Quantite == 0)
                {
                    await _chStock.HandleDeleteAsync(stockId, callChain);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}