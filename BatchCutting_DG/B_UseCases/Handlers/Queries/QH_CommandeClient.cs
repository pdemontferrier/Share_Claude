using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_CommandeClient : QH_Generic<CommandeClient>, IQ_CommandeClient
    {
        private readonly IR_CommandesClient _repositorySpecifique;

        public QH_CommandeClient(IR_CommandesClient repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Obtenir la liste des Commande Client pour un lot donné
        public async Task<List<CommandeClient>> HandleGetByIdDecoupeLotAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetByDecoupeLotIdAsync(decoupeLotId);
        }
    }
}