using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour l’état d’un lot de découpe à la fin de l’approvisionnement.
    /// </summary>
    public class SR_DecoupeLot_UpdateAtStockRelease : IS_DecoupeLot_UpdateAtStockRelease
    {
        private readonly string _callee;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IC_DecoupeLot _chDecoupeLot;

        public SR_DecoupeLot_UpdateAtStockRelease(
            IQ_DecoupeLot qhDecoupeLot,
            IC_DecoupeLot chDecoupeLot)
        {
            _callee = GetType().Name;
            _qhDecoupeLot = qhDecoupeLot;
            _chDecoupeLot = chDecoupeLot;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, bool isChute, DTO_AppContext appCtx, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0)
                    throw new Ex_Business(callChain, $"DLU_01 - decoupeLotId : {decoupeLotId}", "No_Er_Bu_41");

                var decoupeLot = await _qhDecoupeLot.HandleGetByIdAsync(decoupeLotId);
                if (decoupeLot == null)
                    throw new Ex_Business(callChain,"DLU_02", "No_Er_Bu_30");

                if (isChute)
                {
                    decoupeLot.ApproChute = true;
                    decoupeLot.ApproChuteUtilisateurErp = appCtx.AppUserId;
                    decoupeLot.ApproChuteUtilisateurPoste = appCtx.AppDeviceUser;
                    decoupeLot.ApproChutePosteId = appCtx.AppDeviceId;
                    decoupeLot.ApproChutePosteIp = appCtx.AppDeviceIP;
                    decoupeLot.ApproChuteDate = appCtx.AppDateTime;
                    decoupeLot.DecoupeDg = false;
                }
                else
                {
                    decoupeLot.ApproNeuf = true;
                    decoupeLot.ApproNeufUtilisateurErp = appCtx.AppUserId;
                    decoupeLot.ApproNeufUtilisateurPoste = appCtx.AppDeviceUser;
                    decoupeLot.ApproNeufPosteId = appCtx.AppDeviceId;
                    decoupeLot.ApproNeufPosteIp = appCtx.AppDeviceIP;
                    decoupeLot.ApproNeufDate = appCtx.AppDateTime;
                    decoupeLot.DecoupeDg = false;
                }

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