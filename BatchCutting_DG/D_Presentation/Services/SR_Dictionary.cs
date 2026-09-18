using System.Windows;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Dictionary : IS_Dictionary
    {
        public ResourceDictionary GetLanguageDictionary() => UT_Dictionary.Language_Dic;
        public void SetLanguageDictionary(ResourceDictionary dictionary) => UT_Dictionary.Language_Dic = dictionary;

        public string GetText(string key)
        {
            // Vérifie si la clé existe dans le dictionnaire et retourne la valeur associée
            return UT_Dictionary.Language_Dic[key] as string ?? "Not found";
        }
    }
}