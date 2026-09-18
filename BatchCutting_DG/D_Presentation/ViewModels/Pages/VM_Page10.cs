using System.Collections.ObjectModel;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page10 : VM_Page_Generic
    {
        private readonly string ServiceName;
        private readonly IC_DecoupeLot _chDecoupeLot;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IQ_DecoupeLotWithCut _qhDecoupeLotWithCut;
        private readonly IS_Settings _settings;
        private readonly IS_Navigation _navigation;
        private readonly IS_DataBase _dataBase;
        private readonly IS_Decoupe _decoupe;

        public ObservableCollection<DTO_DecoupeLotWithCut> LotsADecouper { get; } = new();

        private DTO_DecoupeLotWithCut? _selectedLot;
        public DTO_DecoupeLotWithCut? SelectedLot
        {
            get => _selectedLot;
            set
            {
                _selectedLot = value;
                OnPropertyChanged();

                if (_selectedLot != null)
                    _ = HandleSelectedLotAsync(_selectedLot.Id); // fire and forget
            }
        }

        public VM_Page10(IC_DecoupeLot chDecoupeLot, IQ_DecoupeLot qhDecoupeLot, IQ_DecoupeLotWithCut qhDecoupeLotWithCut,
                               IS_Settings settings, IS_Navigation navigation,
                               IS_DataBase dataBase, IS_Decoupe decoupe)
        {
            ServiceName = GetType().Name;
            _chDecoupeLot = chDecoupeLot;
            _qhDecoupeLot = qhDecoupeLot;
            _qhDecoupeLotWithCut = qhDecoupeLotWithCut;
            _settings = settings;
            _navigation = navigation;
            _dataBase = dataBase;
            _decoupe = decoupe;
        }

        public async Task LoadDataAsync()
        {
            _settings.SetDecoupeLotWithCut(await _qhDecoupeLotWithCut.HandleAsync());
            LotsADecouper.Clear();
            foreach (var lot in _settings.GetDecoupeLotWithCut())
                LotsADecouper.Add(lot);

            // Mettre à jour le lot encour de coupe si il y a
            int lotId = _settings.GetDecoupeLotId();
            if (lotId > 0)
            {
                await HandleBatchNotInProgressAsync(lotId);
            }
        }

        private async Task HandleSelectedLotAsync(int lotId)
        {
            // Mettre à jour le lot sélectionné
            _settings.SetDecoupeLotId(lotId);

            // Mettre à jour DecoupeLot
            await _decoupe.DecoupeLotStartAsync();

            // Lancer la procédure stockée SprDecoupeBarreUpdateEmpSCAsync(int appID)
            await _dataBase.SprDecoupeBarreUpdateEmpSCAsync(lotId);

            // Mettre à jour le lot sélectionné
            await HandleBatchInProgressAsync(lotId);

            // Afficher la Page20
            _navigation.NavigateToNewPage("Page20");
        }

        private async Task HandleBatchInProgressAsync(int lotId)
        {
            var lot = await _qhDecoupeLot.HandleGetByIdAsync(lotId);
            lot.DecoupeDgEncours = true;
            await _chDecoupeLot.HandleUpdateAsync(lot, ServiceName, nameof(HandleBatchNotInProgressAsync));
        }

        private async Task HandleBatchNotInProgressAsync(int lotId)
        {
            var lot = await _qhDecoupeLot.HandleGetByIdAsync(lotId);
            lot.DecoupeDgEncours = false;
            await _chDecoupeLot.HandleUpdateAsync(lot, ServiceName, nameof(HandleBatchNotInProgressAsync));
            _settings.SetDecoupeLotId(0);
        }
    }
}