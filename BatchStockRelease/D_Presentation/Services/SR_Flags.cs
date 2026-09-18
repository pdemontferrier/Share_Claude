using System.ComponentModel;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Settings;

namespace BatchStockRelease.D_Presentation.Services
{
    /// <summary>
    /// Service de gestion des drapeaux de langue.
    /// <para>
    /// Fournit l’URI et l’image WPF du drapeau courant, et relaie les notifications
    /// de changement de <see cref="SE_Flags"/> pour permettre la mise à jour automatique
    /// dans les ViewModels.
    /// </para>
    /// </summary>
    public class SR_Flags : IS_Flags, INotifyPropertyChanged
    {
        /// <summary>
        /// Événement déclenché lorsqu'une propriété est modifiée.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructeur : relaye automatiquement les changements de <see cref="SE_Flags"/>
        /// vers le service pour informer les ViewModels abonnés.
        /// </summary>
        public SR_Flags()
        {
            // Relais automatique des PropertyChanged provenant de SE_Flags
            SE_Flags.PropertyChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, e);
            };
        }

        #region === Propriétés exposées ===

        /// <summary>
        /// Retourne l’URI du drapeau actuellement affiché.
        /// </summary>
        public Uri GetAppFlagUri() => SE_Flags.AppFlagUri;

        #endregion

        #region === Méthodes de mise à jour ===

        /// <summary>
        /// Définit le drapeau courant à partir du code de langue (ex: "FR", "EN", "DE").
        /// </summary>
        public void SetAppFlagUri(Uri flagUri)
        {
            if (SE_Flags.AppFlagUri != flagUri)
            {
                SE_Flags.AppFlagUri = flagUri;
                SE_Flags.OnPropertyChanged(nameof(SE_Flags.AppFlagUri));
            }
        }

        /// <summary>
        /// Retourne l’URI du drapeau correspondant au code de langue donné.
        /// </summary>
        public Uri GetFlagUriFromLanguageCode(string languageCode)
        {
            if (!string.IsNullOrWhiteSpace(languageCode) &&
                SE_Flags.ReferenceFlag.TryGetValue(languageCode.ToUpperInvariant(), out var uri))
            {
                return uri;
            }

            return SE_Flags.DefaultFlagUri;
        }

        #endregion
    }
}