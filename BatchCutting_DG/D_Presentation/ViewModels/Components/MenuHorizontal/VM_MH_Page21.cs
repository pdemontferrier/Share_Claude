using System.Windows.Input;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal
{
    public class VM_MH_Page21 : VM_MH_Generic
    {
        private readonly IS_Settings _settings;
        private readonly IS_Decoupe _decoupe;
        private readonly IS_Notification _notification;
        private bool _isProcessing = false;
        public bool IsProcessing
        {
            get => _isProcessing;
            set
            {
                _isProcessing = value;
                OnPropertyChanged();
            }
        }
        public ICommand ValidateRefusal { get; }

        public VM_MH_Page21(IS_Settings settings, IS_Decoupe decoupe, IS_Notification notification)
        {
            _settings = settings;
            _decoupe = decoupe;
            _notification = notification;

            ValidateRefusal = new UT_RelayCommandArg0(RefusalValidation);
        }

        private async void RefusalValidation()
        {
            // Ne rien faire si déjà en cours
            if (IsProcessing) return;

            try
            {
                // Signaler qu'un process est encours
                IsProcessing = true;

                // Changer l'apparence de la souris
                Mouse.OverrideCursor = Cursors.Wait;

                // Appeller le service
                // Tester si un motif de refus est sélectionné
                if (string.IsNullOrEmpty(_settings.GetDecoupeBarreDecoupeCommentaires()))
                {
                    // Afficher une notification pour demander de sélectionner un motif de refus
                    _notification.Warning("No_Se_06");
                }
                else
                {
                    // Mettre à jour la barre validée
                    await _decoupe.DecoupeBarreRefuseAsync();
                }
            }
            finally
            {
                // Changer l'apparence de la souris
                Mouse.OverrideCursor = null;

                // Mettre à jour le staut du process
                IsProcessing = false;
            }

        }
    }
}