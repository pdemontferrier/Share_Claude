using System.Windows;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page30 : VM_Page_Generic
    {
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IQ_DecoupeDetailWithCut _qhDecoupeDetailWithCut;
        private readonly IS_ReferenceVue _vueSettings;
        private readonly IS_Decoupe _decoupe;
        private readonly IS_Navigation _navigation;
        private readonly IS_Settings _settings;
        private readonly IS_Notification _notification;

        private DTO_DecoupeDetailWithCut? _decoupeDetailWithCut;
        public DTO_DecoupeDetailWithCut? DecoupeDetailWithCut
        {
            get => _decoupeDetailWithCut;
            set
            {
                _decoupeDetailWithCut = value;
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

        public VM_Page30(IQ_DecoupeDetail qhDecoupeDetail, IQ_DecoupeDetailWithCut qhDecoupeDetailWithCut,
                               IS_ReferenceVue vueSettings, IS_Navigation navigation,
                               IS_Decoupe decoupe, IS_Settings settings, 
                               IS_Notification notification)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _qhDecoupeDetailWithCut = qhDecoupeDetailWithCut;
            _vueSettings = vueSettings;
            _decoupe = decoupe;
            _navigation = navigation;
            _settings = settings;
            _notification = notification;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                // Récupérer le premier détail de découpe à traiter
                var decoupeDetail = await _qhDecoupeDetail.HandleGetFirstToCutAsync();
                if (decoupeDetail == null)
                {
                    // Vérifier si il reste des barres à approvisionner
                    // Si oui sortir si non


                    // Terminer le Lot
                    await _decoupe.DecoupeLotCompletedAsync();
                }
                else
                {
                    // Mettre à jour DecoupeDetail dans les settings
                    _settings.SetDecoupeDetailId(decoupeDetail.Id);
                    _settings.SetDecoupeDetailReferenceVue(decoupeDetail.ReferenceVue);

                    // Tester si detail.IdDecoupeBarre = _settings.GetDecoupeBarreId()
                    if (decoupeDetail.IdDecoupeBarre == _settings.GetDecoupeBarreId())
                    {
                        // Charger les infos de la découpe associée via DTO
                        _settings.SetDecoupeDetailWithCut(await _qhDecoupeDetailWithCut.HandleGetFirstAsync());
                        DecoupeDetailWithCut = _settings.GetDecoupeDetailWithCut();

                        // Charger l'image de la découpe associée
                        ReferenceVueUri = _vueSettings.GetVueUri(_settings.GetDecoupeDetailReferenceVue());

                        // Lancer le service Découpe pour le chargement et la mise à jour des données
                        await _decoupe.LoadDecoupeDetailAsync(DecoupeDetailWithCut.Id, DecoupeDetailWithCut.MessageElumatec.ToString());
                    }
                    else
                    {
                        // Mettre à jour DecoupeDarre
                        await _decoupe.UpdateDecoupeBarreAtCutValidationAsync();

                        // Afficher la Page20
                        _navigation.NavigateToNewPage("Page20");
                    }
                }
            }
            catch (Exception ex)
            {
                _notification.Error("No_Er_09", $"Page 30 - {ex.Message}");
                _navigation.NavigateToNewPage("Page10");
            }
        }
    }
}