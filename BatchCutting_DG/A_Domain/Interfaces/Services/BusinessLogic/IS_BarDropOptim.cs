namespace BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_BarDropOptim
    {
        Task ExecuteAsync(int decoupeLotId);
    }
}
