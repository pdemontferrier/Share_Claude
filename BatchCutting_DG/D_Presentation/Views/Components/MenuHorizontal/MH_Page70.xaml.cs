using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal;

namespace BatchCutting_DG.D_Presentation.Views.Components.MenuHorizontal
{
    public partial class MH_Page70 : UserControl
    {
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Icons _icons;
        private readonly IS_Window _window;
        private readonly IS_Navigation _navigation;

        public MH_Page70()
        {
            InitializeComponent();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _icons = App.ServiceProvider.GetRequiredService<IS_Icons>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            _navigation = App.ServiceProvider.GetRequiredService<IS_Navigation>();
            this.DataContext = App.ServiceProvider.GetRequiredService<VM_MH_Page70>();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            StyleControls();

            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.SizeChanged += OnWindowSizeChanged;
            }
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            StyleControls();
        }

        private void StyleControls()
        {
            // MH_Grid
            _controlStyler.StyleHorizontalMenuGrid(MH_Grid, MH_Grid_C1, MH_Grid_C2, MH_Border, _window.GetMainWindowWidth());

            // Initialiser les boutons
            _controlStyler.StyleButton(MH_Menu, _icons.GetMH_Menu_Source());
            _controlStyler.StyleButton(MH_Refresh, _icons.GetMH_Refresh_Source());
            _controlStyler.StyleButton(MH_Previous, _icons.GetMH_Previous_Source());
            _controlStyler.StyleButton(MH_Home, _icons.GetMH_Home_Source());

            // Vérifier l'accès à la Page_10
            if (_navigation.CanNavigate("Page10"))
            {
                MH_Home.Visibility = Visibility.Visible;
            }
        }
    }
}