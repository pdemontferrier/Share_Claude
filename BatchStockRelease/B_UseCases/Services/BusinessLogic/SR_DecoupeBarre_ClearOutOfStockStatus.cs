using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de réinitialiser les statuts de rupture de stock sur les barres d’un lot.
    /// </summary>
    public class SR_DecoupeBarre_ClearOutOfStockStatus : IS_DecoupeBarre_ClearOutOfStockStatus
    {
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly string _callee;

        public SR_DecoupeBarre_ClearOutOfStockStatus(
            IQ_DecoupeBarre qhDecoupeBarre,
            IC_DecoupeBarre chDecoupeBarre)
        {
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeBarre = chDecoupeBarre;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0)
                    throw new Ex_Business(callChain,"DBU_10", "No_Er_Bu_39");

                var barresEnRupture = await _qhDecoupeBarre.HandleGetBarNewOutOfStockAsync(decoupeLotId);

                foreach (DecoupeBarre decoupeBarre in barresEnRupture)
                {
                    decoupeBarre.ApproRupture = false;
                    await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}