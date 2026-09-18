using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant les opérations de sortie de stock d’une barre de chute.
    /// </summary>
    public class UC_BarDropStockRelease : IU_BarDropStockRelease
    {
        private readonly string _callee;
        private readonly IU_BarNewOptim _barNewOptim;
        private readonly IU_BarNewStockAllocation _barNewStockAllocation;
        private readonly IU_DecoupeLot_UpdateAtStockRelease _decoupeLot_UpdateAtStockRelease;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_ChutesArchive_Add _addChuteArchive;
        private readonly IS_ChutesMagasin_Delete _deleteChuteMagasin;
        private readonly IS_DecoupeBarre_UpdateApproDone _updateDecoupeBarre;
        private readonly IS_DecoupeDetail_AnnuleAllocation _updateDecoupeDetail;
        private readonly IS_DecoupeBarre_UpdateApproInactif _markBarreInactive;
        private readonly IS_Notification _notification;
        private readonly IS_Navigation _navigation;
        private readonly IS_LogAndNotify _logAndNotify;
        private readonly IS_Settings_UseCase _settings;
        private readonly IQ_AppContext _appContext;

        public UC_BarDropStockRelease(
            IU_BarNewOptim barNewOptim,
            IU_BarNewStockAllocation barNewStockAllocation,
            IU_DecoupeLot_UpdateAtStockRelease decoupeLot_UpdateAtStockRelease,
            IQ_DecoupeBarre qhDecoupeBarre,
            IS_ChutesArchive_Add addChuteArchive,
            IS_ChutesMagasin_Delete deleteChuteMagasin,
            IS_DecoupeBarre_UpdateApproDone updateDecoupeBarre,
            IS_DecoupeDetail_AnnuleAllocation updateDecoupeDetail,
            IS_DecoupeBarre_UpdateApproInactif markBarreInactive,
            IS_Notification notification,
            IS_Navigation navigation,
            IS_LogAndNotify logAndNotify, 
            IS_Settings_UseCase settings,
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;
            _barNewOptim = barNewOptim;
            _barNewStockAllocation = barNewStockAllocation;
            _decoupeLot_UpdateAtStockRelease = decoupeLot_UpdateAtStockRelease;
            _qhDecoupeBarre = qhDecoupeBarre;
            _addChuteArchive = addChuteArchive;
            _deleteChuteMagasin = deleteChuteMagasin;
            _updateDecoupeBarre = updateDecoupeBarre;
            _updateDecoupeDetail = updateDecoupeDetail;
            _markBarreInactive = markBarreInactive;
            _notification = notification;
            _navigation = navigation;
            _logAndNotify = logAndNotify;
            _settings = settings;
            _appContext = appContext;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, int decoupeBarreId, int chuteMagasinId, int quantite, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Eléments d'identification de l'application'
                DTO_AppContext appCtx = _appContext.GetAppContext();

                // Etape 1 : Ajouter un nouvel enregistrement dans ChuteArchive
                await _addChuteArchive.ExecuteAsync(chuteMagasinId, appCtx.AppDateTime, appCtx.AppUserId, callChain);

                // Etape 2 : Supprimer l'enregistrement de ChuteMagasin
                await _deleteChuteMagasin.ExecuteAsync(chuteMagasinId, callChain);

                // Etape 3 : Mettre à Jour l'enregistrement DecoupeBarre et si besoin DecoupeDetail
                if (quantite == 1)
                {
                    // Mettre à jour DecoupeBarre
                    await _updateDecoupeBarre.ExecuteAsync(decoupeBarreId, appCtx, callChain);
                }
                else if (quantite == 0)
                {
                    // Mettre à jour les enregistrement de DecoupeDetail
                    await _updateDecoupeDetail.ExecuteAsync(decoupeBarreId, callChain);

                    // Mettre à jour DecoupeBarre
                    await _markBarreInactive.ExecuteAsync(decoupeBarreId, appCtx, callChain);
                }

                // Etape 4 : Vérifier si il y a des Barres de chute à approvisionner
                bool hasBarDropToRelease = await _qhDecoupeBarre.HandleCheckBarDropToReleaseAsync(decoupeLotId);
                if (hasBarDropToRelease)
                {
                    // Appeler la méthode de rafraîchissement dans MainWindow
                    _navigation.RefreshCurrentPage();
                }
                else
                {
                    // Afficher une fenêtre d'attente
                    _notification.OpenDialogWindow("No_Ti_09", "No_In_06");

                    // Étape 41 : Mettre à jour DecoupeLot
                    await _decoupeLot_UpdateAtStockRelease.ExecuteAsync(decoupeLotId, true, callChain);

                    // Étape 42 : Lancer l'optimisation des Barres neuves
                    await _barNewOptim.ExecuteAsync(decoupeLotId, callChain);

                    // Étape 43 : Lancer l'allocation des barres
                    await _barNewStockAllocation.ExecuteAsync(decoupeLotId, callChain);

                    // Fermer DialogWindow
                    _notification.CloseDialogWindow();

                    // Étape 44 : Naviguer vers Page10
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