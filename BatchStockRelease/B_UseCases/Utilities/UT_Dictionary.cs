using System.Windows;

namespace BatchStockRelease.B_UseCases.Utilities
{
    /// <summary>
    /// Classe technique contenant le dictionnaire de langue courant.
    /// Ce conteneur est utilisé par SR_Dictionary comme référence centrale.
    /// </summary>
    public static class UT_Dictionary
    {
        /// <summary>
        /// Dictionnaire de ressources chargé (selon la langue active).
        /// </summary>
        public static ResourceDictionary Language_Dic { get; set; } = new ResourceDictionary();
    }
}
