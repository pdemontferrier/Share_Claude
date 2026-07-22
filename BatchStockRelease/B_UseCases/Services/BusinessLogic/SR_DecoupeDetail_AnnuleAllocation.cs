using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour les détails de l'allocation associés à une barre.
    /// </summary>
    public class SR_DecoupeDetail_AnnuleAllocation : IS_DecoupeDetail_AnnuleAllocation
    {
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IC_DecoupeDetail _chDecoupeDetail;
        private readonly string _callee;

        public SR_DecoupeDetail_AnnuleAllocation(IQ_DecoupeDetail qhDecoupeDetail, IC_DecoupeDetail chDecoupeDetail)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _chDecoupeDetail = chDecoupeDetail;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeBarreId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeBarreId <= 0)
                    throw new Ex_Business(callChain,"DDA_01", "No_Er_Bu_35");

                // Rechercher tout les enregistrements de DecoupeDetail pour une barre donnée
                var decoupeDetails = await _qhDecoupeDetail.HandleGetAllByDecoupeBarreIdAsync(decoupeBarreId);

                if (decoupeDetails == null || !decoupeDetails.Any())
                    throw new Ex_Business(callChain, $"DDA_02 - BarreID: {decoupeBarreId}", "No_Er_Bu_36");

                foreach (DecoupeDetail decoupeDetail in decoupeDetails)
                {
                    decoupeDetail.IdDecoupeBarre = 0;
                    decoupeDetail.ApproOptimBarreChute = false;
                    decoupeDetail.ApproOptimBarreNeuve = false;
                    decoupeDetail.DecoupeBarreIndex = 0;
                    decoupeDetail.DecoupeLongueurReste = 0;

                    await _chDecoupeDetail.HandleUpdateAsync(decoupeDetail, callChain);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}