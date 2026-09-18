using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page10 : Page
    {
        private readonly VM_Page10 _viewModel;
        private readonly IS_Window _window;
        private readonly IS_ControlStyler _controlStyler;

        public Page10()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page10>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            this.DataContext = _viewModel;
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.SizeChanged += OnSizeChanged;
        }

        // Méthodes relatives à la page
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Appliquer les styles
            StyleControls();

            // Charger les données du View Model
            await _viewModel.LoadDataAsync();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lors du déchargement de la page, si nécessaire
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Ajuster la hauteur des ScrollViewer
            AdjustScrollViewerHeight();
        }

        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(ListScrollViewer, ListTitle, ListTitleBorder, ListTitleHeader);

            // ListViews
            _controlStyler.StyleListView(ListView);

            // Ajuster la hauteur des ScrollViewer
            AdjustScrollViewerHeight();
        }

        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 305;
            ListScrollViewer.Height = scrollViewerHeight;
        }
    }
}