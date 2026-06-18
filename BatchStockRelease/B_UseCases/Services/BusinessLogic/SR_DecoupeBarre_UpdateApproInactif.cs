using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de marquer une barre de découpe comme inactive.
    /// </summary>
    public class SR_DecoupeBarre_UpdateApproInactif : IS_DecoupeBarre_UpdateApproInactif
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;

        public SR_DecoupeBarre_UpdateApproInactif(
            IQ_DecoupeBarre qhDecoupeBarre, 
            IC_DecoupeBarre chDecoupeBarre)
        {
            _callee = GetType().Name;
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeBarre = chDecoupeBarre;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeBarreId, DTO_AppContext appCtx, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeBarreId <= 0)
                    throw new Ex_Business(callChain,"DBU_03", "No_Er_Bu_37");

                var decoupeBarre = await _qhDecoupeBarre.HandleGetByIdAsync(decoupeBarreId);
                if (decoupeBarre == null)
                    throw new Ex_Business(callChain, $"DBU_04 - ID: {decoupeBarreId}", "No_Er_Bu_38");

                decoupeBarre.ApproInactif = true;
                decoupeBarre.ApproDateFin = appCtx.AppDateTime;
                decoupeBarre.ApproUtilisateurErp = appCtx.AppUserId;
                decoupeBarre.ApproUtilisateurPoste = appCtx.AppDeviceUser;
                decoupeBarre.ApproPosteMachineId = appCtx.AppDeviceId;
                decoupeBarre.ApproPosteMachineIp = appCtx.AppDeviceIP;

                await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}