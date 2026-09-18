using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrateur permettant de forcer la sortie des barres neuves d’un lot de découpe en rupture de stock.
    /// Il met à jour l’état du lot, désactive les barres concernées si demandé, ajoute une ligne d’historique de modification de stock,
    /// et notifie l’utilisateur en cas d’erreur.
    /// </summary>
    public class UC_BarNewForceOutOfStock : IU_BarNewForceOutOfStock
    {
        private readonly string _callee;
        private readonly IS_DecoupeBarre_UpdateAtForceOutOfStock _decoupeBarre_UpdateAtForceOutOfStock;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Navigation _navigation;
        private readonly IS_Notification _notification;
        private readonly IS_Settings_UseCase _settings;
        private readonly IU_DecoupeLot_UpdateAtStockRelease _ucDecoupeLot_UpdateAtStockRelease;
        private readonly IQ_AppContext _appContext;

        public UC_BarNewForceOutOfStock(
            IS_DecoupeBarre_UpdateAtForceOutOfStock decoupeBarre_UpdateAtForceOutOfStock,
            IS_LogAndNotify logAndNotify,
            IS_Navigation navigation,
            IS_Notification notification,
            IS_Settings_UseCase settings,
            IU_DecoupeLot_UpdateAtStockRelease ucDecoupeLot_UpdateAtStockRelease,
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;

            _decoupeBarre_UpdateAtForceOutOfStock = decoupeBarre_UpdateAtForceOutOfStock;
            _logAndNotify = logAndNotify;
            _navigation = navigation;
            _notification = notification;
            _settings = settings;
            _ucDecoupeLot_UpdateAtStockRelease = ucDecoupeLot_UpdateAtStockRelease;
            _appContext = appContext;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, bool forceDecoupeBarre, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Eléments d'identification de l'application'
                DTO_AppContext appCtx = _appContext.GetAppContext();

                // Afficher une fenêtre d'attente
                _notification.OpenDialogWindow("No_Ti_09", "No_In_06");

                // Étape 1 : Mettre à jour la table DecoupeLot
                await _ucDecoupeLot_UpdateAtStockRelease.ExecuteAsync(decoupeLotId, false, callChain);

                // Étape 2 : Mettre à jour la table DecoupeBarre
                if (forceDecoupeBarre)
                    await _decoupeBarre_UpdateAtForceOutOfStock.ExecuteAsync(decoupeLotId, appCtx, callChain);
 
                // Fermer DialogWindow
                _notification.CloseDialogWindow();

                // Étape 3 : Raffraichir la page encours
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