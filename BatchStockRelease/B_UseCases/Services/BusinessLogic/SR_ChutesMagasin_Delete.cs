using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de supprimer une chute du stock magasin.
    /// </summary>
    public class SR_ChutesMagasin_Delete : IS_ChutesMagasin_Delete
    {
        private readonly string _callee;
        private readonly IC_ChutesMagasin _chChutesMagasin;

        public SR_ChutesMagasin_Delete(IC_ChutesMagasin chChutesMagasin)
        {
            _callee = GetType().Name;
            _chChutesMagasin = chChutesMagasin;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int chuteMagasinId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (chuteMagasinId <= 0)
                    throw new Ex_Business(callChain,"CM_01", "No_Er_Bu_32");

                await _chChutesMagasin.HandleDeleteAsync(chuteMagasinId, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}