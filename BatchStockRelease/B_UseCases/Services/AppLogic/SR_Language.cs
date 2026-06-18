using System.Windows;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    public class SR_Language : IS_Language
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_Dictionary _dictionary;
        private readonly IS_Settings_Language _settingsLanguage;
        private readonly IS_Flags _flag;

        #endregion

        #region === Constructeur ===

        public SR_Language(IS_Dictionary dictionary, IS_Settings_Language settingsLanguage, IS_Flags flag)
        {
            _callee = GetType().Name;

            _dictionary = dictionary;
            _settingsLanguage = settingsLanguage;
            _flag = flag;
        }

        #endregion

        #region === Méthodes publiques ===

        public void Execute(string cultureCode)
        {
            // Mettre à jour la Langue
            _settingsLanguage.SetAppCultureCode(cultureCode);

            // Mettre à jour le dictionnaire
            SetDictionary(cultureCode);

            // Mettre à jour le drapeau
            SetFlag(cultureCode);
        }

        #endregion

        #region === Méthodes privées ===

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

        private void SetFlag(string cultureCode)
        {
            // Extraire le code du pays, soit les deux derniers caractères après le tiret ou retourner tout le code
            int index = cultureCode.LastIndexOf('-');
            string countryCode = index >= 0 ? cultureCode.Substring(index + 1).ToUpper() : cultureCode.ToUpper();
            string languageCode = index >= 0 ? cultureCode.Substring(0, index).ToUpperInvariant() : cultureCode.ToUpperInvariant();

            // Assigner l'URI du drapeau à LanguageIcon_Source
            var flagUri = _flag.GetFlagUriFromLanguageCode(languageCode);
            _flag.SetAppFlagUri(flagUri);
        }

        private Uri GetDictionarySource(string cultureCode)
        {
            // Renvoie la source appropriée du dictionnaire en fonction du code culturel
            return cultureCode switch
            {
                string code when code.StartsWith("us", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicEn(),
                string code when code.StartsWith("en", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicEn(),
                string code when code.StartsWith("fr", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicFr(),
                string code when code.StartsWith("de", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicDe(),
                string code when code.StartsWith("es", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicEs(),
                string code when code.StartsWith("it", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicIt(),
                string code when code.StartsWith("pt", StringComparison.OrdinalIgnoreCase) => _settingsLanguage.GetDicPt(),
                _ => _settingsLanguage.GetDicEn() // Valeur par défaut : Anglais
            };
        }

        #endregion
    }
}