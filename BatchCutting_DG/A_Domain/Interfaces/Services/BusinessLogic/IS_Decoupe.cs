namespace BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic
{
    public interface IS_Decoupe
    {
        Task DecoupeLotStartAsync();
        Task DecoupeLotCompletedAsync();
        Task BarValidationAsync();
        Task UpdateDecoupeBarreAtCutValidationAsync();
        Task CutValidationAsync();
        Task BarDropValidationAsync();
        Task LoadDecoupeDetailAsync(int decoupeDetailId, string decoupeDetailMessageElumatec);
        Task DecoupeBarreRefuseAsync();
        Task DecoupeDetailRefusalAsync();
    }
}