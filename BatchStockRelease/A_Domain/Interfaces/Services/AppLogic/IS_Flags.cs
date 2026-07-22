using System.ComponentModel;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Flags
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        Uri GetAppFlagUri();
        void SetAppFlagUri(Uri flagUri);
        Uri GetFlagUriFromLanguageCode(string languageCode);
    }
}
