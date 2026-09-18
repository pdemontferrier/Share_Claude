using System.Windows.Input;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal
{
    public class VM_MH_Page20 : VM_MH_Generic
    {
        private readonly IS_Decoupe _decoupe;
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
        public ICommand ValidateBar { get; }
        public ICommand RefusePage { get; }
        public ICommand DetailBatch { get; }

        public VM_MH_Page20(IS_Decoupe decoupe)
        {
            _decoupe = decoupe;

            ValidateBar = new UT_RelayCommandArg0(BarValidation);
            RefusePage = new UT_RelayCommandArg0(NavigateToPage21);
            DetailBatch = new UT_RelayCommandArg0(NavigateToPage40);
        }

        private async void BarValidation()
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
                await _decoupe.BarValidationAsync();
            }
            finally
            {
                // Changer l'apparence de la souris
                Mouse.OverrideCursor = null;

                // Mettre à jour le staut du process
                IsProcessing = false;
            }
        }

        private void NavigateToPage21()
        {
            _navigation.NavigateToNewPage("Page21");
        }

        private void NavigateToPage40()
        {
            _navigation.NavigateToNewPage("Page40");
        }
    }
}