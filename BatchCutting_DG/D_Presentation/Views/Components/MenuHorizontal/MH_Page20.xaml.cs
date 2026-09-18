using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.D_Presentation.ViewModels.Components.MenuHorizontal;

namespace BatchCutting_DG.D_Presentation.Views.Components.MenuHorizontal
{
    public partial class MH_Page20 : UserControl
    {
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Icons _icons;
        private readonly IS_Window _window;
        private readonly IS_Navigation _navigation;
        private readonly IQ_DecoupeDetail _decoupeDetail;

        public MH_Page20()
        {
            InitializeComponent();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _icons = App.ServiceProvider.GetRequiredService<IS_Icons>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            _navigation = App.ServiceProvider.GetRequiredService<IS_Navigation>();
            _decoupeDetail = App.ServiceProvider.GetRequiredService<IQ_DecoupeDetail>();
            this.DataContext = App.ServiceProvider.GetRequiredService<VM_MH_Page20>();
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

        private async void StyleControls()
        {
            // MH_Grid
            _controlStyler.StyleHorizontalMenuGrid(MH_Grid, MH_Grid_C1, MH_Grid_C2, MH_Border, _window.GetMainWindowWidth());

            // Initialiser les boutons
            _controlStyler.StyleButton(MH_Menu, _icons.GetMH_Menu_Source());
            _controlStyler.StyleButton(MH_Validate, _icons.GetMH_Validate_Source());
            _controlStyler.StyleButton(MH_Refuse, _icons.GetMH_Delete_Source());
            _controlStyler.StyleButton(MH_Details, _icons.GetMH_Details_Source());
            _controlStyler.StyleButton(MH_Refresh, _icons.GetMH_Refresh_Source());
            _controlStyler.StyleButton(MH_Previous, _icons.GetMH_Previous_Source());
            _controlStyler.StyleButton(MH_Home, _icons.GetMH_Home_Source());

            // Vérifier l'accès à la Page_10
            if (_navigation.CanNavigate("Page10"))
            {
                MH_Home.Visibility = Visibility.Visible;
            }

            // Vérifier l'accès à la Page_40
            if (_navigation.CanNavigate("Page40"))
            {
                MH_Details.Visibility = Visibility.Visible;
            }

            // Vérifier les droits de validation
            if (_navigation.CanUpdate("Page20"))
            {
                MH_Validate.Visibility = Visibility.Visible;
                MH_Refuse.Visibility = Visibility.Visible;
            }

            // Vérifier si il existe une découpe déjà réalisée.
            bool existeDecoupe = await _decoupeDetail.HandleExistsByDecoupeBarreIdAsync();
            if (existeDecoupe)
            {
                MH_Refuse.Visibility = Visibility.Collapsed;
            }
        }
    }
}