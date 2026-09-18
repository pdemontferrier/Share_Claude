using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.D_Presentation.ViewModels.Generic
{
    public abstract class VM_Page_Generic : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}