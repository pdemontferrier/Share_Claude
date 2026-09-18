using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using Microsoft.Extensions.DependencyInjection;

namespace BatchCutting_DG.D_Presentation.ViewModels.Generic
{
    public abstract class VM_MH_Generic : INotifyPropertyChanged
    {
        protected readonly IS_Navigation _navigation;
        public ICommand MenuCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand HomePage { get; }


        protected VM_MH_Generic()
        {
            _navigation = App.ServiceProvider.GetRequiredService<IS_Navigation>();

            MenuCommand = new UT_RelayCommandArg0(ReduceHorizontalMenu);
            RefreshCommand = new UT_RelayCommandArg0(RefreshCurrentPage);
            PreviousCommand = new UT_RelayCommandArg0(NavigateToPreviousPage);
            HomePage = new UT_RelayCommandArg0(NavigateToPage10);
        }

        private void ReduceHorizontalMenu()
        {
            _navigation.ReduceHorizontalMenu();
        }

        private void NavigateToPage10()
        {
            _navigation.NavigateToNewPage("Page10");
        }

        private void RefreshCurrentPage()
        {
            _navigation.RefreshCurrentPage();
        }

        private void NavigateToPreviousPage()
        {
            _navigation.NavigateToPreviousPage();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

