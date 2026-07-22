
namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Shutdown
    {
        void ForceShutdown(string caller, string? warningText = null);
        void Shutdown(string caller);
        Task ShutdownWithDelayAsync(int delaySeconds, string titleKey, string contentKey, string caller);
    }
}
