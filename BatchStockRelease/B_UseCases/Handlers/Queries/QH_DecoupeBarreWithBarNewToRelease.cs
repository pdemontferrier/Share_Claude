using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarreWithBarNewToRelease : IQ_DecoupeBarreWithBarNewToRelease
    {
        private readonly IR_DecoupeBarreWithBarNewToRelease _repository;

        public QH_DecoupeBarreWithBarNewToRelease(IR_DecoupeBarreWithBarNewToRelease repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retourne la liste enrichie des enregistrements DecoupeBarre pour un lot donné.
        /// </summary>
        public async Task<List<DTO_DecoupeBarreWithBarNewToRelease>> HandleGetAsync(int decoupeLotId)
        {
            return await _repository.GetAsync(decoupeLotId);
        }
    }
}