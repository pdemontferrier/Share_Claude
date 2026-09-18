using BatchStockRelease.A_Domain.App.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_AppContext
    {
        DTO_AppContext GetAppContext();
    }
}