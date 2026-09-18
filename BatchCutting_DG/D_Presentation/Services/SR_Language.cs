using System.Globalization;
using System.Windows;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;

namespace BatchCutting_DG.D_Presentation.Services
{
    public class SR_Language : IS_Language
    {
        private readonly IS_Dictionary _dictionary;
        private readonly IS_Settings _settings;
        private readonly IS_Flag _flag;

        public SR_Language(IS_Dictionary dictionary, IS_Settings settings, IS_Flag flag) 
        {
            _dictionary = dictionary;
            _settings = settings;
            _flag = flag;
        }

        public void Execute(string cultureCode)
        {
            // Mettre à jour la Langue
            _settings.SetAppCultureCode(cultureCode);

            // A utiliser pour test
            string cultureCodeNew = "it-IT";
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureCodeNew);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultureCodeNew);

            // Mettre à jour le dictionnaire
            SetDictionary(cultureCode);

            // Mettre à jour le drapeau
            SetFlagUri(cultureCode);
        }


        private void SetDictionary(string cultureCode)
        {
            // Appliquer la culture du poste à l'application
            var dictionary = new ResourceDictionary();

            // Déterminer la source du dictionnaire en fonction du code culturel
            dictionary.Source = GetDictionarySource(cultureCode);

            // Appliquer les modifications au dictionnaire de ressources
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dictionary);

            // Mettre à jour la référence du dictionnaire
            _dictionary.SetLanguageDictionary(dictionary);
        }

        private void SetFlagUri(string cultureCode)
        {
            // Extraire le code du pays, soit les deux derniers caractères après le tiret ou retourner tout le code
            int index = cultureCode.LastIndexOf('-');
            string countryCode = index >= 0 ? cultureCode.Substring(index + 1).ToUpper() : cultureCode.ToUpper();
            string languageCode = index >= 0 ? cultureCode.Substring(0, index).ToUpperInvariant() : cultureCode.ToUpperInvariant();

            // Assigner l'URI du drapeau à LanguageIcon_Source
            _flag.SetAppFlagUri(languageCode);
        }

        private Uri GetDictionarySource(string cultureCode)
        {
            // Renvoie la source appropriée du dictionnaire en fonction du code culturel
            return cultureCode switch
            {
                string code when code.StartsWith("us", StringComparison.OrdinalIgnoreCase) => _settings.GetDicEn(),
                string code when code.StartsWith("en", StringComparison.OrdinalIgnoreCase) => _settings.GetDicEn(),
                string code when code.StartsWith("fr", StringComparison.OrdinalIgnoreCase) => _settings.GetDicFr(),
                string code when code.StartsWith("de", StringComparison.OrdinalIgnoreCase) => _settings.GetDicDe(),
                string code when code.StartsWith("es", StringComparison.OrdinalIgnoreCase) => _settings.GetDicEs(),
                string code when code.StartsWith("it", StringComparison.OrdinalIgnoreCase) => _settings.GetDicIt(),
                string code when code.StartsWith("pt", StringComparison.OrdinalIgnoreCase) => _settings.GetDicPt(),
                _ => _settings.GetDicEn() // Valeur par défaut : Anglais
            };
        }
    }
}