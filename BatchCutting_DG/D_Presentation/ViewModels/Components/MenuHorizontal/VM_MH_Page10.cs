using System.Windows.Input;
using BatchCutting_DG.D_Presentation.Utilities.RelayCommands;
using BatchCutting_DG.D_Presentation.ViewModels.Generic;

namespace BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal
{
    public class VM_MH_Page10 : VM_MH_Generic
    {

        public VM_MH_Page10()
        {
            // Ajoutez ici les propriétés ou commandes spécifiques.
        }

        public ICommand AdminPage => new UT_RelayCommandArg0(NavigateToPage97);

        private void NavigateToPage97()
        {
            _navigation.NavigateToNewPage("Page97");
        }

    }
}