using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Commands
{
    public interface IC_UserAppErrorLog
    {
        Task HandleAddAsync(UserAppErrorLog entity);
    }
}