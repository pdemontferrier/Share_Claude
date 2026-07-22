using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using System.Windows.Input;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    public class VM_MH_Reduce
    {
        protected readonly IS_Navigation _navigation;
        public ICommand MenuCommand { get; }

        public VM_MH_Reduce(IS_Navigation navigation)
        {
            _navigation = navigation;

            MenuCommand = new UT_RelayCommandArg0(ExpendHorizontalMenu);
        }

        private void ExpendHorizontalMenu()
        {
            _navigation.ExpendHorizontalMenu();
        }
    }
}