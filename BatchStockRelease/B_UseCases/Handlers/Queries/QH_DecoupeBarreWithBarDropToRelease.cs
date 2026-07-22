using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarreWithBarDropToRelease : IQ_DecoupeBarreWithBarDropToRelease
    {
        private readonly IR_DecoupeBarreWithBarDropToRelease _repository;

        public QH_DecoupeBarreWithBarDropToRelease(IR_DecoupeBarreWithBarDropToRelease repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retourne la liste enrichie des enregistrements DecoupeBarre pour un lot donné.
        /// </summary>
        public async Task<List<DTO_DecoupeBarreWithBarDropToRelease>> HandleGetAsync(int decoupeLotId)
        {
            return await _repository.GetAsync(decoupeLotId);
        }
    }
}