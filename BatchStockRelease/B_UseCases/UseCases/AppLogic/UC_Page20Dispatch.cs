using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.AppLogic
{
    /// <summary>
    /// UseCase orchestrant la logique de navigation à partir de la Page20 en fonction de l’état d’approvisionnement.
    /// </summary>
    public class UC_Page20Dispatch : IU_Page20Dispatch
    {
        private readonly string _callee;
        private readonly IS_Settings_UseCase _settings;
        private readonly IS_Notification _notification;
        private readonly IS_DecoupeBarre_UpdateChariot _decoupeBarre_UpdateChariot;
        private readonly IS_DecoupeLot_UpdateChariot _decoupeLot_UpdateChariot;
        private readonly IU_BarNewStockAllocation _ucBarNewStockAllocation;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_Navigation _navigation;
        private readonly IS_LogAndNotify _logAndNotify;


        public UC_Page20Dispatch(
            IS_Settings_UseCase settings,
            IS_Notification notification,
            IS_DecoupeBarre_UpdateChariot decoupeBarre_UpdateChariot,
            IS_DecoupeLot_UpdateChariot decoupeLot_UpdateChariot,
            IU_BarNewStockAllocation ucBarNewStockAllocation,
            IQ_DecoupeLot qhDecoupeLot,
            IQ_DecoupeBarre qhDecoupeBarre,
            IS_Navigation navigation,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;
            _settings = settings;
            _notification = notification;
            _decoupeBarre_UpdateChariot = decoupeBarre_UpdateChariot;
            _decoupeLot_UpdateChariot = decoupeLot_UpdateChariot;
            _ucBarNewStockAllocation = ucBarNewStockAllocation;
            _qhDecoupeLot = qhDecoupeLot;
            _qhDecoupeBarre = qhDecoupeBarre;
            _navigation = navigation;
            _logAndNotify = logAndNotify;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                int chariotId = _settings.GetDecoupeBarreIdChariot();
                string? chariotDesignation = _settings.GetDecoupeBarreChariotDesignation();

                if (decoupeLotId <= 0 || string.IsNullOrEmpty(chariotDesignation))
                {
                    _navigation.NavigateToNewPage("Page10");
                    return;
                }

                // Étape 1 : Mettre à jour les informations du chariot
                await _decoupeLot_UpdateChariot.ExecuteAsync(decoupeLotId, chariotId, chariotDesignation, callChain);
                await _decoupeBarre_UpdateChariot.ExecuteAsync(decoupeLotId, chariotId, chariotDesignation, callChain);
                
                // Étape 2 : Vérifier l’état d’approvisionnement
                bool approChute = await _qhDecoupeLot.HandleCheckApproChuteAsync(decoupeLotId);
                if (!approChute)
                {
                    _navigation.NavigateToNewPage("Page31");
                    return;
                }

                bool approNeuf = await _qhDecoupeLot.HandleCheckApproNeufAsync(decoupeLotId);
                if (!approNeuf)
                {
                    // Étape 2.1 : Lancer allocation si des barres sont en attente
                    var notAllocatedList = await _qhDecoupeBarre.HandleGetBarNewNotAllocatedAsync(decoupeLotId);
                    if (notAllocatedList?.Any() == true)
                    {
                        // Afficher une fenêtre d'attente
                        _notification.OpenDialogWindow("No_Ti_09", "No_In_06");

                        // Lancer le usecase d'allocation des barres neuves
                        await _ucBarNewStockAllocation.ExecuteAsync(decoupeLotId, callChain);

                        // Fermer DialogWindow
                        _notification.CloseDialogWindow();
                    }

                    // Étape 2.2 : Vérifier les barres à sortir
                    if (await _qhDecoupeBarre.HandleCheckBarNewToReleaseAsync(decoupeLotId))
                    {
                        _navigation.NavigateToNewPage("Page32");
                        return;
                    }

                    // Étape 2.3 : Vérifier les ruptures
                    if (await _qhDecoupeBarre.HandleCheckBarNewOutOfStock(decoupeLotId))
                    {
                        _navigation.NavigateToNewPage("Page30");
                        return;
                    }

                    // Aucune barre à traiter ni rupture
                    _navigation.NavigateToNewPage("Page10");
                    return;
                }

                // Si tout est déjà approvisionné
                _navigation.NavigateToNewPage("Page10");

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