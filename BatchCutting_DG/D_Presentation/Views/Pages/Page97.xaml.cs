using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page97 : Page
    {
        private readonly VM_Page97 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Window _window;

        public Page97()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page97>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            this.DataContext = _viewModel;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;
        }

        // Méthodes relatives à la page
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les styles
            StyleControls();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Logique à exécuter lors du changement de taille de la page, si nécessaire
        }

        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(UserSessionScrollViewer, UserSessionTitle, UserSessionBorder, UserSessionHeader);

            // ListViews
            _controlStyler.StyleListView(UserSessionListView);

            // Ajuster la hauteur des ScrollViewer
            AdjustScrollViewerHeight();

        }

        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 348;
            //UserSessionScrollViewer.Height = scrollViewerHeight;
        }
    }
}
