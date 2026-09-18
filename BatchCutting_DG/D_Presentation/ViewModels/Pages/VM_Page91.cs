using System.Windows;
using System.Windows.Input;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page91 : VM_Page_Generic
    {

        private readonly IS_Settings _settings;
        private readonly IS_Navigation _navigation;
        private readonly IS_Language _language;
        private readonly IS_Dictionary _dictionary;
        private bool _isLanguage1Selected;
        private bool _isLanguage2Selected;
        private bool _isLanguage3Selected;
        private bool _isLanguage4Selected;
        private bool _isLanguage5Selected;
        private bool _isLanguage6Selected;

        public ICommand ChangeLanguageCommand { get; }

        public bool IsLanguage1Selected
        {
            get => _isLanguage1Selected;
            set
            {
                if (_isLanguage1Selected != value)
                {
                    _isLanguage1Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLanguage2Selected
        {
            get => _isLanguage2Selected;
            set
            {
                if (_isLanguage2Selected != value)
                {
                    _isLanguage2Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLanguage3Selected
        {
            get => _isLanguage3Selected;
            set
            {
                if (_isLanguage3Selected != value)
                {
                    _isLanguage3Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLanguage4Selected
        {
            get => _isLanguage4Selected;
            set
            {
                if (_isLanguage4Selected != value)
                {
                    _isLanguage4Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLanguage5Selected
        {
            get => _isLanguage5Selected;
            set
            {
                if (_isLanguage5Selected != value)
                {
                    _isLanguage5Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLanguage6Selected
        {
            get => _isLanguage6Selected;
            set
            {
                if (_isLanguage6Selected != value)
                {
                    _isLanguage6Selected = value;
                    OnPropertyChanged();
                }
            }
        }

        public VM_Page91(IS_Settings settings, IS_Navigation navigation, 
                                IS_Language language, IS_Dictionary dictionary)
        {
            _settings = settings;
            _navigation = navigation;
            _language = language;
            _dictionary = dictionary;

            ChangeLanguageCommand = new UT_RelayCommandArg1<string>(SetLanguageCode);

            LoadDataAsync();
        }

        private void LoadDataAsync()
        {
            // Définir la langue actuellement utilisée.
            GetLanguageCode();
        }

        private void GetLanguageCode()
        {
            string cultureCode = _settings.GetAppCultureCode();
            int index = cultureCode.LastIndexOf('-');
            string countryCode = index >= 0 ? cultureCode.Substring(index + 1).ToUpper() : cultureCode.ToUpper();
            string languageCode = index >= 0 ? cultureCode.Substring(0, index).ToUpperInvariant() : cultureCode.ToUpperInvariant();

            // Réinitialiser toutes les sélections
            IsLanguage1Selected = false;
            IsLanguage2Selected = false;
            IsLanguage3Selected = false;
            IsLanguage4Selected = false;
            IsLanguage5Selected = false;
            IsLanguage6Selected = false;

            // Activer la bonne langue
            switch (languageCode)
            {
                case "FR":
                    IsLanguage1Selected = true;
                    break;
                case "EN":
                    IsLanguage2Selected = true;
                    break;
                case "DE":
                    IsLanguage3Selected = true;
                    break;
                case "ES":
                    IsLanguage4Selected = true;
                    break;
                case "IT":
                    IsLanguage5Selected = true;
                    break;
                case "PT":
                    IsLanguage6Selected = true;
                    break;
                default:
                    // Optionnel : définir un comportement par défaut si besoin
                    break;
            }
        }

        private void SetLanguageCode( string languageCode) 
        {
            string cultureCode = languageCode switch
            {
                "FR" => "fr-FR",
                "EN" => "en-US",
                "DE" => "de-DE",
                "ES" => "es-ES",
                "IT" => "it-IT",
                "PT" => "pt-PT",
                _ => "fr-FR" // Valeur par défaut (français si code inconnu)
            };

            // Mettre à jour la Langue
            _language.Execute(cultureCode);
            ((MainWindow)Application.Current.MainWindow).RefreshLanguageIcon();

            // Mettre à jour le titre de l'application
            _settings.SetApplicationTitle(_dictionary.GetText("App_Ti_00"));
            ((MainWindow)Application.Current.MainWindow).CommonBackgroundPad.Title = _settings.GetApplicationTitle();

            // Afficher la Page 10
            _navigation.NavigateToNewPage("Page10");

        }
    }
}