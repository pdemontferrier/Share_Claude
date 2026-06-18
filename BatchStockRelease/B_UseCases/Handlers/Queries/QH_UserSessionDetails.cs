using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserSessionDetails : IQ_UserSessionDetails
    {
        private readonly IR_UserSessionDetails _repository;

        public QH_UserSessionDetails(IR_UserSessionDetails repository)
        {
            _repository = repository;
        }

        public async Task<List<DTO_UserSessionDetails>> HandleAsync(int appId)
        {
            return await _repository.GetUserSessionDetailsAsync(appId);
        }
    }
}