using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page90 : Page
    {
        private readonly VM_Page90 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Window _window;
        private readonly IS_Dictionary _dictionary;

        public Page90()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page90>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
            _window = App.ServiceProvider.GetRequiredService<IS_Window>();
            _dictionary = App.ServiceProvider.GetRequiredService<IS_Dictionary>();
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
            // Ajuster la hauteur des Control
            AdjustControlsHeight();
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

            // Style UserDetailsBorder
            _controlStyler.StyleBorder(UserDetailsBorder);

            // TabItems
            var UserDetailsTab = new TextBlock();
            _controlStyler.StyleTabItem(UserDetailsTabItem, UserDetailsTab, _dictionary.GetText("P90_10"), 150);
            var UUserAccessTab = new TextBlock();
            _controlStyler.StyleTabItem(UserAccessTabItem, UUserAccessTab, _dictionary.GetText("P90_11"), 150);
            var UserRightsTab = new TextBlock();
            _controlStyler.StyleTabItem(UserRightsTabItem, UserRightsTab, _dictionary.GetText("P90_12"), 150);

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();

            // ScrollViewers
            _controlStyler.StyleScrollViewer(UserAccessScrollViewer, null, UserAccessBorder, UserAccessHeader01, UserAccessHeader02, 
                UserAccessHeader03, UserAccessHeader04, UserAccessHeader05, UserAccessHeader06, UserAccessHeader07, UserAccessHeader08, 
                UserAccessHeader09, UserAccessHeader10, UserAccessHeader11);

            _controlStyler.StyleScrollViewer(UserRightsScrollViewer, null, UserRightsBorder, UserRightsHeader01, UserRightsHeader02,
                UserRightsHeader03, UserRightsHeader04, UserRightsHeader05, UserRightsHeader06, UserRightsHeader07, UserRightsHeader08,
                UserRightsHeader09, UserRightsHeader10, UserRightsHeader11);

            // ListViews
            _controlStyler.StyleListView(UserAccessListView);
            _controlStyler.StyleListView(UserRightsListView);
        }

        private void AdjustControlsHeight()
        {
            double tabControlHeight = _window.GetMainWindowHeight() - 220;
            MainTabControl.Height = tabControlHeight;

            double scrollViewerHeight = tabControlHeight - 93;
            UserAccessScrollViewer.Height = scrollViewerHeight;
            UserRightsScrollViewer.Height = scrollViewerHeight;
        }
    }
}