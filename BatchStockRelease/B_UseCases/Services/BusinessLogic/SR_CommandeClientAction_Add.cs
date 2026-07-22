using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;


namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’ajouter une action client à une commande spécifiée.
    /// </summary>
    public class SR_CommandeClientAction_Add : IS_CommandeClientAction_Add
    {
        private readonly string _callee;
        private readonly IC_CommandeClientAction _chCommandeClientAction;

        public SR_CommandeClientAction_Add(IC_CommandeClientAction chCommandeClientAction)
        {
            _callee = GetType().Name;
            _chCommandeClientAction = chCommandeClientAction;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            CommandeClient commande, 
            int idAction,
            DateTime appDateTime,
            int appUserId,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                var action = new CommandeClientAction
                {
                    IdCmdClient = commande.Id,
                    IdUser = appUserId,
                    IdAction = idAction,
                    DateAction = appDateTime,
                    TempsEstime = 0,
                    BonusMalus = 1
                };

                await _chCommandeClientAction.HandleAddAsync(action, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}