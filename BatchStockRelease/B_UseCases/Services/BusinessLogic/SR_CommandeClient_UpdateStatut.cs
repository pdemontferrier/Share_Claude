using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier responsable de la mise à jour du statut d’une commande client.
    /// </summary>
    public class SR_CommandeClient_UpdateStatut : IS_CommandeClient_UpdateStatut
    {
        private readonly string _callee;
        private readonly IC_CommandeClient _chCommandeClient;

        public SR_CommandeClient_UpdateStatut(IC_CommandeClient chCommandeClient)
        {
            _callee = GetType().Name;
            _chCommandeClient = chCommandeClient;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(CommandeClient commande, int statutId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                commande.Statut = statutId;
                await _chCommandeClient.HandleUpdateAsync(commande, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}