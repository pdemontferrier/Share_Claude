using BatchCutting_DG.D_Presentation.ViewModels.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.D_Presentation.ViewModels.Pages
{
    public class VM_Page98 : VM_Page_Generic
    {
        public string VersionNumber { get; private set; }

        public VM_Page98()
        {
            VersionNumber = GetVersionNumber();
            LoadData();
        }

        private void LoadData()
        {
            // A Définir
        }


        private string GetVersionNumber()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version != null ? version.ToString() : "Version inconnue";
        }
    }
}
