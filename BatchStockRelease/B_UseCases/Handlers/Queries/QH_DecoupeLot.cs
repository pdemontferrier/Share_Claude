using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeLot : QH_Generic<DecoupeLot>, IQ_DecoupeLot
    {
        private readonly IR_DecoupeLot _repositorySpecifique;

        public QH_DecoupeLot(IR_DecoupeLot repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }

        // Requête spécifique : Vérifier si le lot a été approvisionné en barre de chute
        public async Task<bool> HandleCheckApproChuteAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckApproChuteAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si le lot a été approvisionné en barre neuve
        public async Task<bool> HandleCheckApproNeufAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckApproNeufAsync(decoupeLotId);
        }

    }
}