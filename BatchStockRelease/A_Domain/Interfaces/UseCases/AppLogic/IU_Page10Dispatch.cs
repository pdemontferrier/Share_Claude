namespace BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic
{
    public interface IU_Page10Dispatch
    {
        Task<string> ExecuteBarDropAsync(int decoupeLotId, int idChariot, string caller);
        Task<string> ExecuteBarNewAsync(int decoupeLotId, int chariotId, string caller);
    }
}
