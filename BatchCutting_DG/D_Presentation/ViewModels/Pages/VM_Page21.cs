using System.Collections.ObjectModel;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page21 : VM_Page_Generic
    {
        private readonly IS_Settings _settings;
        private readonly IS_ReferenceVue _vueSettings;
        private readonly IS_Dictionary _dictionary;

        private DTO_DecoupeBarreWithCut? _decoupeBarreWithCut;
        public DTO_DecoupeBarreWithCut? DecoupeBarreWithCut
        {
            get => _decoupeBarreWithCut;
            set
            {
                _decoupeBarreWithCut = value;
                OnPropertyChanged();
            }
        }

        private Uri? _referenceVueUri;
        public Uri? ReferenceVueUri
        {
            get => _referenceVueUri;
            set
            {
                _referenceVueUri = value;
                OnPropertyChanged();
            }
        }

        // Liste des motifs de refus
        public ObservableCollection<string> RefusMotifs { get; }

        private string? _selectedRefusMotif;
        public string? SelectedRefusMotif
        {
            get => _selectedRefusMotif;
            set
            {
                _selectedRefusMotif = value;
                _settings.SetDecoupeBarreDecoupeCommentaires(value);
                OnPropertyChanged();
            }
        }

        public VM_Page21(IS_Settings settings, IS_ReferenceVue vueSettings, IS_Dictionary dictionary)
        {
            _settings = settings;
            _vueSettings = vueSettings;
            _dictionary = dictionary;

            RefusMotifs = new ObservableCollection<string>
            {
                _dictionary.GetText("P21_10"),
                _dictionary.GetText("P21_11"),
                _dictionary.GetText("P21_12"),
                _dictionary.GetText("P21_13")
            };

            LoadData();
        }

        private void LoadData()
        {
            // Mettre la valeur de DecoupeBarreDecoupeCommentaires à empty 
            _settings.SetDecoupeBarreDecoupeCommentaires(string.Empty);

            // Charger les infos de la barre associée via DTO
            DecoupeBarreWithCut = _settings.GetDecoupeBarreWithCut();

            // Charger l'image de la découpe associée
            ReferenceVueUri = _vueSettings.GetVueUri(_settings.GetDecoupeDetailReferenceVue());
        }
    }
}