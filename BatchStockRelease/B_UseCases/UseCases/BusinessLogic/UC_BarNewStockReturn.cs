using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.A_Domain.App.DTOs;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase permettant d’effectuer un retour en stock de barres neuves dans le cadre de la préparation d’un lot de découpe.
    /// Mise à jour de l’état du stock, la consommation article interne, et l'historisation de la modification du stock'.
    /// </summary>
    public class UC_BarNewStockReturn : IU_BarNewStockReturn
    {
        private readonly string _callee;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IQ_DecoupeBarreWithBarNewToRelease _qhDecoupeBarreDetails;
        private readonly IS_AIConsommation_Add _aiConsommation_AddEntry;
        private readonly IS_DecoupeBarre_UpdateStockReleaseState _decoupeBarre_UpdateStockReleaseState;
        private readonly IS_DecoupeLot_UpdateAtStockRelease _decoupeLot_UpdateAtStockRelease;
        private readonly IS_Stock_UpdateQuantity _stock_UpdateQuantity;
        private readonly IS_Stock_DeleteIfQuantityNull _stock_DeleteIfQuantityNull;
        private readonly IS_StockModif_Add _stock_AddModification;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Navigation _navigation;
        private readonly IS_Settings_UseCase _settings;
        private readonly IU_BatchDocumentation _ucBatchDocumentation;
        private readonly IQ_AppContext _appContext;

        public UC_BarNewStockReturn(
            IQ_DecoupeBarre qhDecoupeBarre,
            IQ_DecoupeBarreWithBarNewToRelease qhDecoupeBarreDetails,
            IS_AIConsommation_Add articleInterneConsommation_AddEntry,
            IS_DecoupeBarre_UpdateStockReleaseState decoupeBarre_UpdateStockReleaseState,
            IS_DecoupeLot_UpdateAtStockRelease decoupeLot_UpdateAtStockRelease,
            IS_Stock_UpdateQuantity stock_UpdateQuantity,
            IS_Stock_DeleteIfQuantityNull stock_DeleteIfQuantityNull,
            IS_StockModif_Add stock_AddModification,
            IS_LogAndNotify logAndNotify,
            IS_Navigation navigation,
            IS_Settings_UseCase settings,
            IU_BatchDocumentation ucBatchDocumentation,
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;
            _qhDecoupeBarre = qhDecoupeBarre;
            _qhDecoupeBarreDetails = qhDecoupeBarreDetails;
            _aiConsommation_AddEntry = articleInterneConsommation_AddEntry;
            _decoupeBarre_UpdateStockReleaseState = decoupeBarre_UpdateStockReleaseState;
            _decoupeLot_UpdateAtStockRelease = decoupeLot_UpdateAtStockRelease;
            _stock_UpdateQuantity = stock_UpdateQuantity;
            _stock_DeleteIfQuantityNull = stock_DeleteIfQuantityNull;
            _stock_AddModification = stock_AddModification;
            _logAndNotify = logAndNotify;
            _navigation = navigation;
            _settings = settings;
            _ucBatchDocumentation = ucBatchDocumentation;
            _appContext = appContext;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(VieStockQuantiteEmplacement StockRecord, int quantiteRetour, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Eléments d'identification de l'application'
                DTO_AppContext appCtx = _appContext.GetAppContext();

                // Étape 1 : Mettre à jour la table Stock
                DTO_StockQuantity result = await _stock_UpdateQuantity.ExecuteAsync(StockRecord.Id, - quantiteRetour, false, callChain);

                // Étape 2 : Ajouter une entrée dans la table StockModif
                await _stock_AddModification.ExecuteAsync(StockRecord.IdArticleInterne, false, StockRecord.ConteneurDesignation, result, appCtx.AppDateTime, appCtx.AppUserId, callChain);

                // Étape 3 : Ajouter une entrée dans la table ArticleInterneConsommation
                await _aiConsommation_AddEntry.ExecuteAsync(9999, StockRecord.IdArticleInterne, false, StockRecord.ConteneurDesignation, StockRecord.AdresseDesignation, result, appCtx.AppDateTime, appCtx.AppUserId, callChain);

                // Appeler la méthode de rafraîchissement dans MainWindow
                _navigation.RefreshCurrentPage();
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