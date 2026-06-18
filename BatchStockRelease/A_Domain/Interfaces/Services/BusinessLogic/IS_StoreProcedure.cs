
namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_StoreProcedure
    {
        Task SprApplicationAccessUpdateAsync(int appID);
        Task SprDecoupeBarreUpdateEmpSCAsync(int appID);
        Task SprCommandeClientProductionUpdateAsync();
    }
}
