using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.AppLogic
{
    public class UC_Page10Dispatch : IU_Page10Dispatch
    {
        private readonly string _callee;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IU_BarNewStockAllocation _barNewStockAllocation;
        private readonly IS_DecoupeLot_SetInfo _decoupeLotSetInfo;
        private readonly IS_Notification _notification;
        private readonly IS_Dictionary _dictionary;

        public UC_Page10Dispatch(
            IC_DecoupeBarre chDecoupeBarre,
            IQ_DecoupeBarre qhDecoupeBarre,
            IU_BarNewStockAllocation barNewStockAllocation,
            IS_DecoupeLot_SetInfo decoupeLotSetInfo,
            IS_Notification notification,
            IS_Dictionary dictionary)
        {
            _callee = GetType().Name;
            _chDecoupeBarre = chDecoupeBarre;
            _qhDecoupeBarre = qhDecoupeBarre;
            _barNewStockAllocation = barNewStockAllocation;
            _decoupeLotSetInfo = decoupeLotSetInfo;
            _notification = notification;
            _dictionary = dictionary;
        }

        public async Task<string> ExecuteBarDropAsync(int decoupeLotId, int idChariot, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteBarDropAsync)}";

            // Mettre à jour les informations du lot
            await _decoupeLotSetInfo.ExecuteAsync(decoupeLotId, callChain);

            if (idChariot == 0)
                return "Page20";
            else
                return "Page31";
        }

        public async Task<string> ExecuteBarNewAsync(int decoupeLotId, int chariotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteBarNewAsync)}";

            // Mettre à jour les informations du lot
            await _decoupeLotSetInfo.ExecuteAsync(decoupeLotId, callChain);

            if (chariotId == 0)
                return "Page20";

            // Rechercher si il y a des barres allouées dont IdStock n'existe plus
            var broken = await _qhDecoupeBarre.HandleCheckBarNewAllocatedToReallocateAsync(decoupeLotId);
            if (broken.Count > 0)
            {
                // Mettre à jour un ensemble d'enregistrement de DecoupeBarre pour annuler l'allocation
                await _chDecoupeBarre.HandleDisableAllocationForBarreAsync(broken, callChain);
            }

            // Rechercher si il y a de barres non allouées
            var nonAllocatedBar = await _qhDecoupeBarre.HandleGetBarNewNotAllocatedAsync(decoupeLotId);
            if (nonAllocatedBar.Any())
            {
                // Afficher une fenêtre d'attente
                _notification.OpenDialogWindow("No_Ti_09", "No_In_06");

                // Lancer le service d'allocation
                await _barNewStockAllocation.ExecuteAsync(decoupeLotId, callChain);

                // Fermer DialogWindow
                _notification.CloseDialogWindow();
            }

            // Navigation
            if (await _qhDecoupeBarre.HandleCheckBarNewToReleaseAsync(decoupeLotId))
                return "Page32";

            if (await _qhDecoupeBarre.HandleCheckBarNewOutOfStock(decoupeLotId))
                return "Page30";

            return "Page10";
        }
    }
}