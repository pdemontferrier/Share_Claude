namespace BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_BarNewOptim
    {
        Task ExecuteAsync(int decoupeLotId);
    }
}
