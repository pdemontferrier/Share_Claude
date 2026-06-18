using System.Windows;
using System.Windows.Input;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Pages
{
    /// <summary>
    /// ViewModel de la Page91 — Page de sélection de la langue de l’application.
    ///
    /// <para><b>Contexte :</b> Cette page permet à l’utilisateur de modifier
    /// la langue d’affichage de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Permettre à l’utilisateur de choisir la langue d’affichage
    /// parmi les six langues supportées par l’application :</para>
    /// <list type="bullet">
    ///   <item><description><b>FR :</b> Français</description></item>
    ///   <item><description><b>EN :</b> Anglais</description></item>
    ///   <item><description><b>DE :</b> Allemand</description></item>
    ///   <item><description><b>ES :</b> Espagnol</description></item>
    ///   <item><description><b>IT :</b> Italien</description></item>
    ///   <item><description><b>PT :</b> Portugais</description></item>
    /// </list>
    ///
    /// <para><b>Vue associée :</b> <c>Page91.xaml</c></para>
    ///
    /// <para><b>Spécificités techniques :</b>
    /// - Utilise les services <see cref="IS_Language"/>, <see cref="IS_Settings_Language"/> et <see cref="IS_Settings_App"/>.  
    /// - Met à jour dynamiquement la langue, le titre de l’application et l’icône correspondante.  
    /// - Gère les erreurs via <see cref="Ex_Classifier"/> et <see cref="IS_LogAndNotify"/>.</para>
    /// </summary>
    public class VM_Page91 : VM_Page_Generic
    {
        #region === Dépendances privées ===

        private readonly IS_Language _language;
        private readonly IS_Settings_Language _settingsLanguage;
        private readonly IS_Settings_App _settingsApp;

        #endregion

        #region === Commandes ===

        /// <summary>Commande de changement de langue de l’application.</summary>
        public ICommand ChangeLanguageCommand { get; }

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// Initialise le ViewModel de la Page91 avec les services nécessaires.
        /// </summary>
        public VM_Page91(
            IS_Language language,
            IS_Settings_Language settingsLanguage,
            IS_Settings_App settingsApp,
            IS_Settings_UseCase settingsUseCase,
            IS_Navigation navigation,
            IS_Dictionary dictionary,
            IS_LogAndNotify logAndNotify)
            : base(settingsUseCase, navigation, dictionary, logAndNotify)
        {
            _language = language;
            _settingsLanguage = settingsLanguage;
            _settingsApp = settingsApp;

            ChangeLanguageCommand = new UT_RelayCommandArg1<string>(SetLanguageCode);
        }

        #endregion

        #region === Propriétés liées à la vue ===

        private bool _isLanguage1Selected;
        private bool _isLanguage2Selected;
        private bool _isLanguage3Selected;
        private bool _isLanguage4Selected;
        private bool _isLanguage5Selected;
        private bool _isLanguage6Selected;

        /// <summary>Indique si la langue française est sélectionnée.</summary>
        public bool IsLanguage1Selected
        {
            get => _isLanguage1Selected;
            set => SetProperty(ref _isLanguage1Selected, value);
        }

        /// <summary>Indique si la langue anglaise est sélectionnée.</summary>
        public bool IsLanguage2Selected
        {
            get => _isLanguage2Selected;
            set => SetProperty(ref _isLanguage2Selected, value);
        }

        /// <summary>Indique si la langue allemande est sélectionnée.</summary>
        public bool IsLanguage3Selected
        {
            get => _isLanguage3Selected;
            set => SetProperty(ref _isLanguage3Selected, value);
        }

        /// <summary>Indique si la langue espagnole est sélectionnée.</summary>
        public bool IsLanguage4Selected
        {
            get => _isLanguage4Selected;
            set => SetProperty(ref _isLanguage4Selected, value);
        }

        /// <summary>Indique si la langue italienne est sélectionnée.</summary>
        public bool IsLanguage5Selected
        {
            get => _isLanguage5Selected;
            set => SetProperty(ref _isLanguage5Selected, value);
        }

        /// <summary>Indique si la langue portugaise est sélectionnée.</summary>
        public bool IsLanguage6Selected
        {
            get => _isLanguage6Selected;
            set => SetProperty(ref _isLanguage6Selected, value);
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// Charge la langue actuellement utilisée et met à jour l’état de sélection dans la vue.
        /// </summary>
        public async Task LoadDataAsync()
        {
            string callChain = BuildFirstCallChain(nameof(LoadDataAsync));

            await ExecuteSafeAsync(async () =>
            {
                try
                {
                    string languageCode = GetLanguageCodeFromCulture();
                    ApplyLanguageSelection(languageCode);
                    await Task.CompletedTask;
                }
                catch (Ex_Infrastructure)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw Ex_Classifier.Execute(callChain, ex);
                }
            });
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// Extrait le code langue (FR, EN, DE, etc.) à partir du code culture complet.
        /// </summary>
        /// <returns>Code langue au format ISO (ex. : FR, EN, DE, ES, IT, PT).</returns>
        private string GetLanguageCodeFromCulture()
        {
            string cultureCode = _settingsLanguage.GetAppCultureCode();
            int index = cultureCode.LastIndexOf('-');
            return index >= 0
                ? cultureCode.Substring(0, index).ToUpperInvariant()
                : cultureCode.ToUpperInvariant();
        }

        /// <summary>
        /// Met à jour les propriétés de sélection visuelle en fonction du code de langue.
        /// </summary>
        /// <param name="languageCode">Code langue ISO à activer.</param>
        private void ApplyLanguageSelection(string languageCode)
        {
            // Réinitialiser toutes les sélections
            IsLanguage1Selected = false;
            IsLanguage2Selected = false;
            IsLanguage3Selected = false;
            IsLanguage4Selected = false;
            IsLanguage5Selected = false;
            IsLanguage6Selected = false;

            // Activer la langue correspondante
            switch (languageCode)
            {
                case "FR": IsLanguage1Selected = true; break;
                case "EN": IsLanguage2Selected = true; break;
                case "DE": IsLanguage3Selected = true; break;
                case "ES": IsLanguage4Selected = true; break;
                case "IT": IsLanguage5Selected = true; break;
                case "PT": IsLanguage6Selected = true; break;
                default: IsLanguage1Selected = true; break; // français par défaut
            }
        }

        /// <summary>
        /// Change la langue de l’application selon le code sélectionné.
        /// </summary>
        /// <param name="languageCode">Code de langue ISO (FR, EN, DE, ES, IT, PT).</param>
        private void SetLanguageCode(string languageCode)
        {
            string cultureCode = GetCultureCodeFromLanguage(languageCode);
            ApplyLanguageChange(cultureCode);
            RefreshApplicationAfterLanguageChange();
        }

        /// <summary>
        /// Convertit un code langue ISO (FR, EN, etc.) en code culture complet (fr-FR, en-US...).
        /// </summary>
        /// <param name="languageCode">Code de langue ISO.</param>
        /// <returns>Code culture complet au format RFC (ex. : fr-FR).</returns>
        private string GetCultureCodeFromLanguage(string languageCode)
        {
            return languageCode switch
            {
                "FR" => "fr-FR",
                "EN" => "en-US",
                "DE" => "de-DE",
                "ES" => "es-ES",
                "IT" => "it-IT",
                "PT" => "pt-PT",
                _ => "fr-FR"
            };
        }

        /// <summary>
        /// Applique la langue sélectionnée et met à jour les ressources de l’application.
        /// </summary>
        /// <param name="cultureCode">Code culture complet à appliquer (ex. : fr-FR).</param>
        private void ApplyLanguageChange(string cultureCode)
        {
            _language.Execute(cultureCode);
        }

        /// <summary>
        /// Met à jour le titre de l’application et redirige l’utilisateur vers la page d’accueil.
        /// </summary>
        private void RefreshApplicationAfterLanguageChange()
        {
            _settingsApp.SetApplicationTitle(_dictionary.GetText("App_Ti_00"));
            ((MainWindow)Application.Current.MainWindow).CommonBackgroundPad.Title = _settingsApp.GetApplicationTitle();
            _navigation.NavigateToNewPage("Page10");
        }

        #endregion
    }
}