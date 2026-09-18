using System.Windows;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page20 : VM_Page_Generic
    {
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IQ_DecoupeBarreWithCut _qhDecoupeBarreWithCut;
        private readonly IS_Settings _settings;
        private readonly IS_ReferenceVue _vueSettings;
        private readonly IS_Navigation _navigation;
        private readonly IS_Notification _notification;
        private readonly IS_Decoupe _decoupe;

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

        public VM_Page20(IQ_DecoupeDetail qhDecoupeDetail, IQ_DecoupeBarreWithCut qhDecoupeBarreWithCut,
                               IS_Settings settings, IS_ReferenceVue vueSettings, 
                               IS_Navigation navigation, IS_Notification notification, IS_Decoupe decoupe)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _qhDecoupeBarreWithCut = qhDecoupeBarreWithCut;
            _settings = settings;
            _vueSettings = vueSettings;
            _navigation = navigation;
            _notification = notification;
            _decoupe = decoupe;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Récupérer le premier détail de découpe à traiter
                var decoupeDetail = await _qhDecoupeDetail.HandleGetFirstToCutAsync();
                if (decoupeDetail == null)
                {
                    // Terminer le Lot
                    await _decoupe.DecoupeLotCompletedAsync();
                }
                else
                {
                    // Mettre à jour DecoupeDetail et DecoupeBarre dans les settings
                    _settings.SetDecoupeDetailId(decoupeDetail.Id);
                    _settings.SetDecoupeDetailReferenceVue(decoupeDetail.ReferenceVue);
                    _settings.SetDecoupeBarreId(decoupeDetail.IdDecoupeBarre);

                    // Charger les infos de la barre associée via DTO
                    _settings.SetDecoupeBarreWithCut(await _qhDecoupeBarreWithCut.HandleGetFirstAsync());
                    DecoupeBarreWithCut = _settings.GetDecoupeBarreWithCut();

                    // Charger l'image de la découpe associée
                    ReferenceVueUri = _vueSettings.GetVueUri(_settings.GetDecoupeDetailReferenceVue());
                }
            }
            catch (Exception ex)
            {
                _notification.Error("No_Er_09", $"Page 22 - {ex.Message}");
                _navigation.NavigateToNewPage("Page10");
            }
        }
    }
}