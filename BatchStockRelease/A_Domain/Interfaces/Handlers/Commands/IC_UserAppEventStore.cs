namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserAppEventStore
    {
        Task HandleAddAsync(string tableDesignation, int tableId, string data, string appCallChain, string appHandlerCommand, string appCommandMethod);
    }
}
