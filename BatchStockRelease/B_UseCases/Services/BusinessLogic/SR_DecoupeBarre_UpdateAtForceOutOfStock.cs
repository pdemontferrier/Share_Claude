using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de marquer toutes les barres neuves en rupture de stock comme sorties forcées,
    /// en mettant à jour les champs de traçabilité associés.
    /// </summary>
    public class SR_DecoupeBarre_UpdateAtForceOutOfStock : IS_DecoupeBarre_UpdateAtForceOutOfStock
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;

        public SR_DecoupeBarre_UpdateAtForceOutOfStock(
            IQ_DecoupeBarre qhDecoupeBarre, 
            IC_DecoupeBarre chDecoupeBarre)
        {
            _callee = GetType().Name;
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeBarre = chDecoupeBarre;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, DTO_AppContext appCtx, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0)
                    throw new Ex_Business(callChain, "DBU_07", "No_Er_Bu_41");

                var decoupeBarreList = await _qhDecoupeBarre.HandleGetBarNewOutOfStockAsync(decoupeLotId);
                var decoupeBarres = decoupeBarreList
                                        .Where(db => db.ApproOrigine == "neuf" && db.ApproRupture == true && db.ApproSortieForce == false)
                                        .ToList();

                if (decoupeBarres.Any() == false)
                    throw new Ex_Business(callChain, $"DBU_08 - decoupeLotId : {decoupeLotId}", "No_Er_Bu_34");

                foreach ( var decoupeBarre in decoupeBarres )
                {
                    decoupeBarre.ApproRupture = false;
                    decoupeBarre.ApproAllocation = true;
                    decoupeBarre.ApproSortieFaite = true;
                    decoupeBarre.ApproSortieForce = true;
                    decoupeBarre.ApproDateDebut = appCtx.AppDateTime;
                    decoupeBarre.ApproDateFin = appCtx.AppDateTime;
                    decoupeBarre.ApproUtilisateurErp = appCtx.AppUserId;
                    decoupeBarre.ApproUtilisateurPoste = appCtx.AppDeviceUser;
                    decoupeBarre.ApproPosteMachineId = appCtx.AppDeviceId;
                    decoupeBarre.ApproPosteMachineIp = appCtx.AppDeviceIP;

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