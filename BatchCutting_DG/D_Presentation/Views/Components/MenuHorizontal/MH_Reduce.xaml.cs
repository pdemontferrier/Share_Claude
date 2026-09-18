using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal;

namespace BatchCutting_DG.D_Presentation.Views.Components.MenuHorizontal
{
    public partial class MH_Reduce : UserControl
    {
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Icons _icons;

        public MH_Reduce()
        {
            InitializeComponent();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _icons = App.ServiceProvider.GetRequiredService<IS_Icons>();
            this.DataContext = App.ServiceProvider.GetRequiredService<VM_MH_Reduce>();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StyleControls();
        }

        private void StyleControls()
        {
            // MH_Grid
            _controlStyler.StyleMH_Reduce(MH_Border, MH_Menu, _icons.GetMH_Menu_Source());
        }
    }
}