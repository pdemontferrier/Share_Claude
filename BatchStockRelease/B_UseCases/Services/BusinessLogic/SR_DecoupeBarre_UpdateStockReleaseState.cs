using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.App.DTOs;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de mettre à jour les enregistrements de barres neuves dans la table DecoupeBarre
    /// suite à une sortie de stock. Marque les barres comme sorties ou les réinitialise si non sorties.
    /// </summary>
    public class SR_DecoupeBarre_UpdateStockReleaseState : IS_DecoupeBarre_UpdateStockReleaseState
    {
        private readonly string _callee;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_DecoupeBarre_UpdateApproDone _updateDecoupeBarre;

        public SR_DecoupeBarre_UpdateStockReleaseState(
            IC_DecoupeBarre chDecoupeBarre, 
            IQ_DecoupeBarre qhDecoupeBarre, 
            IS_DecoupeBarre_UpdateApproDone updateDecoupeBarre)
        {
            _callee = GetType().Name;
            _chDecoupeBarre = chDecoupeBarre;
            _qhDecoupeBarre = qhDecoupeBarre;
            _updateDecoupeBarre = updateDecoupeBarre;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, DTO_StockQuantity result, DTO_AppContext appCtx, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Récupérer tous les enregistrements de DecoupeBarre correspondant à idStock
                var decoupeBarreRecords = await _qhDecoupeBarre.HandleGetAllBarNewByIdStock(decoupeLotId, result.IdStock);
                if (decoupeBarreRecords == null)
                    throw new Ex_Infrastructure(callChain, "DBU_12", "No_Er_Bu_25");

                int quantite = Math.Min(result.QuantiteSortie, result.QuantiteAvant);
                int count = decoupeBarreRecords.Count;

                // Etape 1 : Mettre à jour les barres neuves sorties
                for (int i = 0; i < quantite && i < count; i++)
                {
                    // Mettre à jour DecoupeBarre
                    await _updateDecoupeBarre.ExecuteAsync(decoupeBarreRecords[i].Id, appCtx, callChain);
                }

                // Etape 2 : Réinitialiser les barres neuves non sorties
                for (int i = quantite; i < count; i++)
                {
                    ResetAppro(decoupeBarreRecords[i]);
                    await _chDecoupeBarre.HandleUpdateAsync(decoupeBarreRecords[i], callChain);
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// Réinitialise les champs d’approvisionnement de la barre pour permettre sa réutilisation.
        /// </summary>
        private void ResetAppro(DecoupeBarre decoupeBarre)
        {
            decoupeBarre.ApproAllocation = false;
            decoupeBarre.ApproZonePriorite = 0;
            decoupeBarre.ApproZoneDesignation = string.Empty;
            decoupeBarre.ApproAdressePriorite = 0;
            decoupeBarre.ApproAdresseDesignation = string.Empty;
            decoupeBarre.ApproConteneur = string.Empty;
            decoupeBarre.ApproTypeConteneur = string.Empty;
            decoupeBarre.ApproTypeContenant = string.Empty;
            decoupeBarre.ApproEmplacement = 0;
            decoupeBarre.ApproEmplacementDesignation = string.Empty;
        }
    }
}