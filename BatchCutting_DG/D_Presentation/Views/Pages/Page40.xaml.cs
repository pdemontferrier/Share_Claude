using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page40 : Page
    {
        private readonly VM_Page40 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Dictionary _dictionary;
        private readonly IS_Window _window;

        public Page40()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page40>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _dictionary = App.ServiceProvider.GetRequiredService<IS_Dictionary>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
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
            // Logique à exécuter lors du changement de taille de la page, si nécessaire
        }

        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // Appliquer les styles aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // TabControl
            _controlStyler.StyleTabControl(MainTabControl);

            // TabItems
            var DecoupeDetailTab = new TextBlock();
            _controlStyler.StyleTabItem(DecoupeDetailTabItem, DecoupeDetailTab, _dictionary.GetText("P40_01"), 300);
            var DecoupeBarreTab = new TextBlock();
            _controlStyler.StyleTabItem(DecoupeBarreTabItem, DecoupeBarreTab, _dictionary.GetText("P40_02"), 300);
            var DropBarTab = new TextBlock();
            _controlStyler.StyleTabItem(DropBarTabItem, DropBarTab, _dictionary.GetText("P40_03"), 300);

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();

            // ScrollViewers
            _controlStyler.StyleScrollViewer(DecoupeDetailScrollViewer, null, DecoupeDetailBorder, DecoupeDetailHeader01, DecoupeDetailHeader02,
                DecoupeDetailHeader03, DecoupeDetailHeader04, DecoupeDetailHeader05, DecoupeDetailHeader06, DecoupeDetailHeader07, DecoupeDetailHeader08,
                DecoupeDetailHeader09, DecoupeDetailHeader10, DecoupeDetailHeader11);

            _controlStyler.StyleScrollViewer(DecoupeBarreScrollViewer, null, DecoupeBarreBorder, DecoupeBarreHeader01, DecoupeBarreHeader02,
                DecoupeBarreHeader03, DecoupeBarreHeader04, DecoupeBarreHeader05);

            _controlStyler.StyleScrollViewer(DropBarScrollViewer, null, DropBarBorder, DropBarHeader01, DropBarHeader02,
                DropBarHeader03, DropBarHeader04);

            // ListViews
            _controlStyler.StyleListView(DecoupeDetailListView);
            _controlStyler.StyleListView(DecoupeBarreListView);
            _controlStyler.StyleListView(DropBarListView);
        }

        private void AdjustControlsHeight()
        {
            double tabControlHeight = _window.GetMainWindowHeight() - 220;
            MainTabControl.Height = tabControlHeight;

            double scrollViewerHeight = tabControlHeight - 93;
            DecoupeDetailScrollViewer.Height = scrollViewerHeight;
            DecoupeDetailScrollViewer.Height = scrollViewerHeight;

            DecoupeBarreScrollViewer.Height = scrollViewerHeight;
            DecoupeBarreScrollViewer.Height = scrollViewerHeight;

            DropBarScrollViewer.Height = scrollViewerHeight;
            DropBarScrollViewer.Height = scrollViewerHeight;

        }
    }
}