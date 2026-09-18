
namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Application
    {
        void ShutdownApplicationForceClose(string? warningText = null);
        void ShutdownApplication();
    }
}
