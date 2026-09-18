using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page96 — Interface de messagerie interne entre utilisateurs et applications.
    ///
    /// <para><b>Contexte :</b> Cette page centralise l’affichage des messages reçus
    /// et envoyés par l’utilisateur au sein du système BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Offrir un espace de consultation et de lecture
    /// des messages, structuré en trois onglets :</para>
    /// <list type="bullet">
    ///   <item><description><b>Messages reçus :</b> Liste des messages entrants, triés par date.</description></item>
    ///   <item><description><b>Messages envoyés :</b> Liste des messages sortants.</description></item>
    ///   <item><description><b>Détail du message :</b> Contenu complet du message sélectionné.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs disposant d’un compte,
    /// notamment les opérateurs et responsables.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page96"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// gérer le cycle de vie, la stylisation et les ajustements dynamiques
    /// des contrôles. La page est purement informative, sans action directe :
    /// la messagerie est manipulée via le Menu Horizontal (<c>MH_Page96</c>).</para>
    /// </summary>
    public partial class Page96 : Page_Generic
    {
        private readonly VM_Page96 _viewModel;
        private bool _isInitializing = true;

        public Page96()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page96>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données de messagerie lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Logique générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Ajuste la hauteur des contrôles en fonction de la taille de la fenêtre principale.
        /// </summary>
        protected override void OnPageResized()
        {
            base.OnPageResized();

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();
        }

        /// <summary>
        /// Applique les styles et prépare l’affichage des onglets de messagerie.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel du comportement générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // Appliquer les styles aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // TabControl
            _controlStyler.StyleTabControl(MainTabControl);

            // Style UserDetailsBorder
            _controlStyler.StyleBorder(MessageBorder);

            // TabItems
            ApplyLayoutTabItem();

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();

            // ScrollViewers
            ApplyLayoutScrollViewer();

            // ListViews
            ApplyLayoutListView();

            // Fin de l'initialisation
            _isInitializing = false;
        }

        /// <summary>
        /// Applique le style des composants ScrollViewer.
        /// </summary>
        private void ApplyLayoutScrollViewer()
        {
            _controlStyler.StyleScrollViewer(MessagesReceivedScrollViewer, null, MessagesReceivedBorder, MessagesReceivedHeader1, MessagesReceivedHeader2, MessagesReceivedHeader3);
            _controlStyler.StyleScrollViewer(MessagesSentScrollViewer, null, MessagesSentBorder, MessagesSentHeader1, MessagesSentHeader2, MessagesSentHeader3);
        }

        /// <summary>
        /// Applique le style des composants ListView.
        /// </summary>
        private void ApplyLayoutListView()
        {
            _controlStyler.StyleListView(MessagesReceivedListView);
            _controlStyler.StyleListView(MessagesSentListView);
        }

        /// <summary>
        /// Applique le style des composants TabItem.
        /// </summary>
        private void ApplyLayoutTabItem()
        {
            var MessagesReceivedTab = new TextBlock();
            _controlStyler.StyleTabItem(MessagesReceivedTabItem, MessagesReceivedTab, _dictionary.GetText("P96_01"), 250);
            var MessagesSentTab = new TextBlock();
            _controlStyler.StyleTabItem(MessagesSentTabItem, MessagesSentTab, _dictionary.GetText("P96_02"), 250);
            var MessageTab = new TextBlock();
            _controlStyler.StyleTabItem(MessageTabItem, MessageTab, _dictionary.GetText("P96_03"), 250);
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