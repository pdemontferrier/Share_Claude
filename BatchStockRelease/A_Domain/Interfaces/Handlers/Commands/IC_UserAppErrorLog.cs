using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserAppErrorLog
    {
        Task HandleAddAsync(UserAppErrorLog entity);
    }
}