using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant l’allocation des barres neuves depuis le stock par article interne.
    /// </summary>
    public class UC_BarNewStockAllocation : IU_BarNewStockAllocation
    {
        private readonly string _callee;
        private readonly IS_DecoupeBarre_ClearOutOfStockStatus _clearOutOfStock;
        private readonly IS_DecoupeBarre_ProcessAllocation _processArticle;
        private readonly IS_DecoupeBarre_UpdateOutOfStockStatus _updateOutOfStockStatus;
        private readonly IS_DecoupeLot_UpdateOptimBarNew _updateOptimBarNew;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_LogAndNotify _logAndNotify;

        public UC_BarNewStockAllocation(
            IS_DecoupeBarre_ClearOutOfStockStatus clearOutOfStock,
            IS_DecoupeBarre_ProcessAllocation processArticle,
            IS_DecoupeBarre_UpdateOutOfStockStatus updateOutOfStockStatus,
            IS_DecoupeLot_UpdateOptimBarNew updateOptimBarNew,
            IQ_DecoupeBarre qhDecoupeBarre,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;
            _clearOutOfStock = clearOutOfStock;
            _processArticle = processArticle;
            _updateOutOfStockStatus = updateOutOfStockStatus;
            _updateOptimBarNew = updateOptimBarNew;
            _qhDecoupeBarre = qhDecoupeBarre;
            _logAndNotify = logAndNotify;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : Remettre à zéro les ruptures de stock
                await _clearOutOfStock.ExecuteAsync(decoupeLotId, callChain);

                // Étape 2 : Récupérer la liste des IdArticleInterne à traiter
                var articleInterneIds = await _qhDecoupeBarre.HandleGetDistinctArticleInterneIdAsync(decoupeLotId);

                // Identifier les cas de nullité et d'erreur pour articleInterneIds


                // Étape 3 : Parcourir la liste des IdArticleInterne
                foreach (var articleInterneId in articleInterneIds)
                {
                    // Appeler ProcessArticleGroup
                    await _processArticle.ExecuteAsync(decoupeLotId, articleInterneId, callChain);
                }

                // Étape 4 : Mettre à jour les informations des ruptures de stock
                await _updateOutOfStockStatus.ExecuteAsync(decoupeLotId, callChain);

                // Étape 5 : Mettre à jour DecoupeLot
                await _updateOptimBarNew.ExecuteAsync(decoupeLotId, callChain);

            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }
    }
}