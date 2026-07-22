using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour un enregistrement de la table <c>ChutesMagasin</c>
    /// en marquant l'état <c>AttenteIntegration</c> à <c>false</c> pour signaler l'intégration.
    /// </summary>
    public class SR_ChutesMagasin_UpdateIntegration : IS_ChutesMagasin_UpdateIntegration
    {
        private readonly string _callee;
        private readonly IC_ChutesMagasin _chChutesMagain;

        public SR_ChutesMagasin_UpdateIntegration(IC_ChutesMagasin chChutesMagain)
        {
            _callee = GetType().Name;
            _chChutesMagain = chChutesMagain;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            ChutesMagasin chuteMagasin, 
            DateTime appDateTime,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Mettre à jour l'enregistrement
                chuteMagasin.AttenteIntegration = false;
                chuteMagasin.DateIntegration = appDateTime;

                // Mettre à jour la table ChutesMagasin
                await _chChutesMagain.HandleUpdateAsync(chuteMagasin, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}