using System.Windows;

namespace BatchCutting_DG.A_Domain.Interfaces.Services.App
{
    public interface IS_Dictionary
    {
        ResourceDictionary GetLanguageDictionary();
        void SetLanguageDictionary(ResourceDictionary dictionary);
        string GetText(string key);
    }
}