using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase permettant d’effectuer la sortie de stock de barres neuves dans le cadre de la préparation d’un lot de découpe.
    /// L'opération est transactionnelle et comprend la mise à jour du stock, la traçabilité de la sortie,
    /// la mise à jour de l’état de découpe, la consommation article interne, et la suppression éventuelle du stock.
    /// </summary>
    public class UC_BarNewStockRelease : IU_BarNewStockRelease
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IQ_DecoupeBarreWithBarNewToRelease _qhDecoupeBarreDetails;
        private readonly IS_AIConsommation_Add _aiConsommation_AddEntry;
        private readonly IS_DecoupeBarre_UpdateStockReleaseState _decoupeBarre_UpdateStockReleaseState;
        private readonly IS_Stock_UpdateQuantity _stock_UpdateQuantity;
        private readonly IS_Stock_DeleteIfQuantityNull _stock_DeleteIfQuantityNull;
        private readonly IS_StockModif_Add _stock_AddModification;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Navigation _navigation;
        private readonly IS_Settings_UseCase _settings;
        private readonly IU_DecoupeLot_UpdateAtStockRelease _ucDecoupeLot_UpdateAtStockRelease;
        private readonly IQ_AppContext _appContext;

        public UC_BarNewStockRelease(
            IQ_DecoupeBarre qhDecoupeBarre,
            IQ_DecoupeLot qhDecoupeLot,
            IQ_DecoupeBarreWithBarNewToRelease qhDecoupeBarreDetails,
            IS_AIConsommation_Add articleInterneConsommation_AddEntry,
            IS_DecoupeBarre_UpdateStockReleaseState decoupeBarre_UpdateStockReleaseState,
            IS_Stock_UpdateQuantity stock_UpdateQuantity,
            IS_Stock_DeleteIfQuantityNull stock_DeleteIfQuantityNull,
            IS_StockModif_Add stock_AddModification,
            IS_LogAndNotify logAndNotify,
            IS_Navigation navigation,
            IS_Settings_UseCase settings,
            IU_DecoupeLot_UpdateAtStockRelease ucDecoupeLot_UpdateAtStockRelease,
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;

            _qhDecoupeBarre = qhDecoupeBarre;
            _qhDecoupeLot = qhDecoupeLot;
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
            _aiConsommation_AddEntry = articleInterneConsommation_AddEntry;
            _decoupeBarre_UpdateStockReleaseState = decoupeBarre_UpdateStockReleaseState;
            _stock_UpdateQuantity = stock_UpdateQuantity;
            _stock_DeleteIfQuantityNull = stock_DeleteIfQuantityNull;
            _stock_AddModification = stock_AddModification;
            _logAndNotify = logAndNotify;
            _navigation = navigation;
            _settings = settings;
            _ucDecoupeLot_UpdateAtStockRelease = ucDecoupeLot_UpdateAtStockRelease;
            _appContext = appContext;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, int decoupeBarreIdStock, int quantiteSortie, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Eléments d'identification de l'application'
                DTO_AppContext appCtx = _appContext.GetAppContext();

                // Étape 1 : Identifier les barres à sortir du stock
                var BarNewList = await _qhDecoupeBarreDetails.HandleGetAsync(decoupeLotId);
                var BarNewToRelease = BarNewList.Where(x => x.IdStock == decoupeBarreIdStock).FirstOrDefault(); ;

                // Étape 2 : Mettre à jour la table Stock
                DTO_StockQuantity result = await _stock_UpdateQuantity.ExecuteAsync(decoupeBarreIdStock, quantiteSortie, true, callChain);

                // Étape 3 : Ajouter une entrée dans la table StockModif
                await _stock_AddModification.ExecuteAsync(BarNewToRelease.IdArticleInterne, true, BarNewToRelease.ApproConteneur, result, appCtx.AppDateTime, appCtx.AppUserId, callChain);

                // Étape 4 : Ajouter une entrée dans la table ArticleInterneConsommation
                await _aiConsommation_AddEntry.ExecuteAsync(decoupeLotId, BarNewToRelease.IdArticleInterne, true, BarNewToRelease.ApproConteneur, BarNewToRelease.ApproAdresseDesignation, result, appCtx.AppDateTime, appCtx.AppUserId, callChain);

                // Étape 5 : Mettre à jour la table DecoupeBarre
                await _decoupeBarre_UpdateStockReleaseState.ExecuteAsync(decoupeLotId, result, appCtx, callChain);

                // Étape 6 : Supprimer l'enregistrement de la table Stock si la quantité = 0
                if (result.QuantiteApres == 0)
                {
                    await _stock_DeleteIfQuantityNull.ExecuteAsync(decoupeBarreIdStock, callChain);
                }

                // Etape 7 : Vérifier si il y a des Barres neuves à approvisionner
                bool hasBarNewToRelease = await _qhDecoupeBarre.HandleCheckBarNewToReleaseAsync(decoupeLotId);
                if (hasBarNewToRelease)
                {
                    // Étape 71 : Appeler la méthode de rafraîchissement dans MainWindow
                    _navigation.RefreshCurrentPage();
                }
                else
                {
                    // Étape 71 : Mettre à jour DecoupeLot
                    await _ucDecoupeLot_UpdateAtStockRelease.ExecuteAsync(decoupeLotId, false, callChain);

                    // Étape 72 : Naviguer vers Page10
                    _navigation.NavigateToNewPage("Page10");
                }
            }
            catch (Ex_Business bex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", bex);
            }
            catch (Ex_Infrastructure iex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", iex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }
    }
}