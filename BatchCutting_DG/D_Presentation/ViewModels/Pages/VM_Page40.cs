using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page40 : VM_Page_Generic
    {
        private readonly IQ_DecoupeBarreWithCut _qhDecoupeBarreWithCut;
        private readonly IQ_DecoupeDetailWithCut _qhDecoupeDetailWithCut;
        private readonly IS_Settings _settings;

        private List<DTO_DecoupeDetailWithCut> _cuttingDetails = new();
        public List<DTO_DecoupeDetailWithCut> CuttingDetails
        {
            get => _cuttingDetails;
            set
            {
                _cuttingDetails = value;
                OnPropertyChanged();
            }
        }

        private List<DTO_DecoupeBarreWithCut> _cuttingBarres = new();
        public List<DTO_DecoupeBarreWithCut> CuttingBarres
        {
            get => _cuttingBarres;
            set
            {
                _cuttingBarres = value;
                OnPropertyChanged();
            }
        }

        private List<DTO_DecoupeBarreWithCut> _dropBarres = new();
        public List<DTO_DecoupeBarreWithCut> DropBarres
        {
            get => _dropBarres;
            set
            {
                _dropBarres = value;
                OnPropertyChanged();
            }
        }

        public VM_Page40(IQ_DecoupeBarreWithCut qhDecoupeBarreWithCut, IQ_DecoupeDetailWithCut qhDecoupeDetailWithCut,
                               IS_Settings settings)
        {
            _qhDecoupeBarreWithCut = qhDecoupeBarreWithCut;
            _qhDecoupeDetailWithCut = qhDecoupeDetailWithCut;
            _settings = settings;
        }

        public async Task LoadDataAsync()
        {
            // Récupérer les arguments de sélection
            int decoupeLotId = _settings.GetDecoupeLotId();
            string machineId = _settings.GetCuttingMachine();

            // Mettre à jour les listes
            CuttingDetails = await _qhDecoupeDetailWithCut.HandleGetAllByLotAndMachineAsync(decoupeLotId, machineId);
            CuttingBarres = await _qhDecoupeBarreWithCut.HandleGetAllByLotAndMachineAsync(decoupeLotId, machineId);
            DropBarres = await _qhDecoupeBarreWithCut.HandleGetAllDropBarByLotAndMachineAsync(decoupeLotId, machineId);
        }
    }
}