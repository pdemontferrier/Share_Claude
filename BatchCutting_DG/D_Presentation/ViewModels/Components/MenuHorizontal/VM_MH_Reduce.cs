using System.Windows.Input;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;

namespace BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal
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