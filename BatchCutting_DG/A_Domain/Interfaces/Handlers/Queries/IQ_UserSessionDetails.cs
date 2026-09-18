using BatchCutting_DG.A_Domain.GestStock.DTOs;

namespace BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_UserSessionDetails
    {
        // Requête spécifique :
        Task<List<DTO_UserSessionDetails>> HandleAsync(int appId);
    }
}
