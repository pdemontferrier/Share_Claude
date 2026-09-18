using System.ComponentModel;
using CommonResources.Settings;

namespace BatchStockRelease.D_Presentation.Settings
{
    /// <summary>
    /// Paramètres statiques liés aux drapeaux de langue de l’application.
    /// <para>
    /// Fournit l’URI du drapeau courant (<see cref="AppFlagUri"/>) et le dictionnaire
    /// des drapeaux disponibles via <see cref="ReferenceFlag"/>.
    /// </para>
    /// </summary>
    public static class SE_Flags
    {

        #region === Champs et propriétés ===

        /// <summary>
        /// Drapeau par défaut (utilisé si aucun code de langue n’est reconnu).
        /// </summary>
        public static readonly Uri DefaultFlagUri =
            new Uri("pack://application:,,,/BatchStockRelease;component/B_UseCases/Ressources/Flags/united_kingdom.png", UriKind.Absolute);

        private static Uri _appFlagUri = DefaultFlagUri;

        /// <summary>
        /// Drapeau actuellement affiché dans l’application.
        /// </summary>
        public static Uri AppFlagUri
        {
            get => _appFlagUri;
            set
            {
                if (_appFlagUri != value)
                {
                    _appFlagUri = value;
                    OnPropertyChanged(nameof(AppFlagUri));
                }
            }
        }

        /// <summary>
        /// Dictionnaire des drapeaux disponibles (clé = code de langue ISO, valeur = URI).
        /// </summary>
        public static Dictionary<string, Uri> ReferenceFlag => CR_FlagsSettings.ReferenceFlag;

        #endregion

        #region === INotifyPropertyChanged statique ===

        public static event PropertyChangedEventHandler? PropertyChanged;
        internal static void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}