namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Flag
    {
        Uri GetAppFlagUri();
        void SetAppFlagUri(string countryCode);
        Uri GetFlagUri(string languageCode);
    }
}
