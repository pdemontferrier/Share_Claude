using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarre : QH_Generic<DecoupeBarre>, IQ_DecoupeBarre
    {
        private readonly IR_DecoupeBarre _repositorySpecifique;

        public QH_DecoupeBarre(IR_DecoupeBarre repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne les informations relatives au chariot utilisé pour un lot donné
        public async Task<(int chariotId, string chariotDesignation)> HandleGetChariotInfoForLotAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetChariotInfoForLotAsync(decoupeLotId);
        }
    }
}