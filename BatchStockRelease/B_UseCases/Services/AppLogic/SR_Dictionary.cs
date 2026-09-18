using System.Windows;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.B_UseCases.Utilities;

namespace BatchStockRelease.B_UseCases.Services.AppLogic
{
    /// <summary>
    /// Service de gestion des dictionnaires multilingues.
    /// Fournit un accès centralisé aux textes de l'application et journalise les clés manquantes.
    /// </summary>
    public class SR_Dictionary : IS_Dictionary
    {
        #region === Propriétés privées ===

        /// <summary>
        /// Nom unique du service pour la traçabilité et la journalisation.
        /// </summary>
        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        private readonly IS_FileLogger _fileLogger;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance du service de dictionnaire multilingue.
        /// </summary>
        /// <param name="fileLogger">Service de journalisation des erreurs.</param>
        public SR_Dictionary(IS_FileLogger fileLogger)
        {
            _callee = GetType().Name;

            _fileLogger = fileLogger;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Retourne le dictionnaire de langue actuellement chargé.
        /// </summary>
        public ResourceDictionary GetLanguageDictionary()
            => UT_Dictionary.Language_Dic;

        /// <summary>
        /// Définit le dictionnaire de langue actif.
        /// </summary>
        public void SetLanguageDictionary(ResourceDictionary dictionary)
            => UT_Dictionary.Language_Dic = dictionary;

        /// <summary>
        /// Récupère le texte correspondant à une clé donnée.
        /// Si la clé est absente, enregistre un log dans le fichier d’erreurs.
        /// </summary>
        /// <param name="key">Clé du texte (ex: "No_Er_06").</param>
        /// <returns>Texte traduit, ou une mention "[key] not found" si la clé est introuvable.</returns>
        public string GetText(string key)
        {
            // Conctruire la callChain
            string callChain = $"{_callee} > {nameof(GetText)}";

            try
            {
                var dictionary = UT_Dictionary.Language_Dic;

                if (dictionary == null)
                {
                    LogMissingKeyAsync(callChain, key, "Language dictionary not initialized.");
                    return $"[{key}] not found";
                }

                if (dictionary.Contains(key))
                {
                    return dictionary[key] as string ?? $"[{key}] not found";
                }

                // Clé manquante : log
                LogMissingKeyAsync(callChain, key, "Key not found in language dictionary.");
                return $"[{key}] not found";
            }
            catch (Exception ex)
            {
                // En cas d’erreur imprévue (ex: dictionnaire non chargé)
                LogMissingKeyAsync(callChain, key, $"Unexpected error: {ex.Message}");
                return $"[{key}] not found";
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Journalise les clés manquantes dans le dictionnaire.
        /// </summary>
        private async void LogMissingKeyAsync(string callContext, string key, string details)
        {
            string errorId = "DICT_01";
            string errorMessage = $"Missing translation key: {key}";
            string errorDetails = details;

            await _fileLogger.LogErrorAsync(callContext, errorId, errorMessage,  errorDetails);
        }

        #endregion
    }
}