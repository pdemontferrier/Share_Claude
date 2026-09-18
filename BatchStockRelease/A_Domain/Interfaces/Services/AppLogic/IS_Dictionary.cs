using System.Windows;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    public interface IS_Dictionary
    {
        ResourceDictionary GetLanguageDictionary();
        void SetLanguageDictionary(ResourceDictionary dictionary);
        string GetText(string key);
    }
}