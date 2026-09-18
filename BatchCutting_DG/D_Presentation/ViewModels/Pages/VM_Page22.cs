using System.Windows;
using System.Windows.Input;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page22 : VM_Page_Generic
    {
        private readonly IS_Settings _settings;
        private readonly IS_ReferenceVue _vueSettings;
        private int _length;

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

        public int Length
        {
            get => _length;
            set
            {
                _length = value;
                _settings.SetDecoupeBarreLongueurChuteFinale(value); 
                OnPropertyChanged();
            }
        }

        public ICommand IncrementLengthCommand { get; }
        public ICommand DecrementLengthCommand { get; }


        public VM_Page22( IS_Settings settings, IS_ReferenceVue vueSettings)
        {
            _settings = settings;
            _vueSettings = vueSettings;

            IncrementLengthCommand = new UT_RelayCommandArg0(IncrementLength);
            DecrementLengthCommand = new UT_RelayCommandArg0(DecrementLength);

            LoadData();
        }

        private void LoadData()
        {
            // Charger les infos de la barre associée via DTO
            DecoupeBarreWithCut = _settings.GetDecoupeBarreWithCut();

            // Charger l'image de la découpe associée
            ReferenceVueUri = _vueSettings.GetVueUri(_settings.GetDecoupeDetailReferenceVue());

            // Charger la longueur de la chute
            Length = DecoupeBarreWithCut.LongueurChuteFinale;
        }

        private void IncrementLength()
        {
            if (Length < DecoupeBarreWithCut.LongueurBarre)
            {
                Length += 100;
            }
        }

        private void DecrementLength()
        {
            if (Length >= 100)
            {
                Length -= 100;
            }
        }
    }
}