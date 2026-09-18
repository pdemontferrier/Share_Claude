using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour les statuts d’approvisionnement neuf d’un lot.
    /// </summary>
    public class SR_DecoupeBarre_UpdateOutOfStockStatus : IS_DecoupeBarre_UpdateOutOfStockStatus
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;

        public SR_DecoupeBarre_UpdateOutOfStockStatus(IQ_DecoupeBarre qhDecoupeBarre, IC_DecoupeBarre chDecoupeBarre)
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
                    throw new Ex_Business(callChain,"DBU_11", "No_Er_Bu_41");

                var nonAllocatedBarres = await _qhDecoupeBarre.HandleGetBarNewNotAllocatedAsync(decoupeLotId);

                foreach (var barre in nonAllocatedBarres)
                {
                    barre.ApproRupture = true;
                    await _chDecoupeBarre.HandleUpdateAsync(barre, callChain);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}