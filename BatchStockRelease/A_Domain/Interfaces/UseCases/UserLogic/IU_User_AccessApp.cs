
namespace BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic
{
    public interface IU_User_AccessApp
    {
        Task<bool> ExecuteAsync(string caller);
    }
}
