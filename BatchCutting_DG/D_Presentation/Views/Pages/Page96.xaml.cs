using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.D_Presentation.ViewModels.Pages;
using System;

namespace BatchCutting_DG.D_Presentation.Views.Pages
{
    public partial class Page96 : Page
    {
        private readonly VM_Page96 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Window _window;
        private readonly IS_Dictionary _dictionary;
        private bool _isInitializing = true;

        public Page96()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page96>();
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
            _controlStyler.StyleBorder(MessageBorder);

            // TabItems
            var MessagesReceivedTab = new TextBlock();
            _controlStyler.StyleTabItem(MessagesReceivedTabItem, MessagesReceivedTab, _dictionary.GetText("P96_01"), 250);
            var MessagesSentTab = new TextBlock();
            _controlStyler.StyleTabItem(MessagesSentTabItem, MessagesSentTab, _dictionary.GetText("P96_02"), 250);
            var MessageTab = new TextBlock();
            _controlStyler.StyleTabItem(MessageTabItem, MessageTab, _dictionary.GetText("P96_03"), 250);

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();

            // ScrollViewers
            _controlStyler.StyleScrollViewer(MessagesReceivedScrollViewer, null, MessagesReceivedBorder, MessagesReceivedHeader1, MessagesReceivedHeader2, MessagesReceivedHeader3);
            _controlStyler.StyleScrollViewer(MessagesSentScrollViewer, null, MessagesSentBorder, MessagesSentHeader1, MessagesSentHeader2, MessagesSentHeader3);

            // ListViews
            _controlStyler.StyleListView(MessagesReceivedListView);
            _controlStyler.StyleListView(MessagesSentListView);

            DateTitle.Text = _dictionary.GetText("P96_08");

            // Fin de l'initialisation
            _isInitializing = false;
        }

        private void AdjustControlsHeight()
        {
            double tabControlHeight = _window.GetMainWindowHeight() - 220;
            MainTabControl.Height = tabControlHeight;

            double scrollViewerHeight = tabControlHeight - 93;
            MessagesReceivedScrollViewer.Height = scrollViewerHeight;
            MessagesSentScrollViewer.Height = scrollViewerHeight;
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isInitializing) return;

            if (MessagesReceivedTabItem.IsSelected && !_isInitializing)
            {
                _viewModel.RefreshPageCommand.Execute(null);
            }
            else if (MessagesSentTabItem.IsSelected)
            {
                DateTitle.Text = _dictionary.GetText("P96_09");
            }
        }
    }
}
