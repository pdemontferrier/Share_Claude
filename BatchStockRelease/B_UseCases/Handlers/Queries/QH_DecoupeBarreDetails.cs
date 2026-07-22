using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarreDetails : IQ_DecoupeBarreDetails
    {
        private readonly IR_DecoupeBarreDetails _repository;

        public QH_DecoupeBarreDetails(IR_DecoupeBarreDetails repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retourne la liste enrichie des enregistrements DecoupeBarre pour un lot donné.
        /// </summary>
        public async Task<List<DTO_DecoupeBarreDetails>> HandleGetAsync(int decoupeLotId)
        {
            return await _repository.GetAsync(decoupeLotId);
        }
    }
}