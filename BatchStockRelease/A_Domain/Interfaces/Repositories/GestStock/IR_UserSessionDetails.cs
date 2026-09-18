using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_UserSessionDetails
    {
        Task<List<DTO_UserSessionDetails>> GetUserSessionDetailsAsync(int appId);
    }
}
