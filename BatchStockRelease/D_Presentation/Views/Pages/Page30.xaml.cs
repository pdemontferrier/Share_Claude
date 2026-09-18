using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page30 — Interface de gestion de la sortie des barres de stock pour un lot de fabrication.
    /// 
    /// <para><b>Contexte :</b> Cette page fait partie du processus global BatchStockRelease,
    /// utilisé par les magasiniers pour préparer et valider les approvisionnements en barres.</para>
    /// 
    /// <para><b>Objectif :</b> Permettre de visualiser et de valider les barres
    /// issues de trois sources distinctes :</para>
    /// <list type="bullet">
    ///   <item><description><b>BarDrop :</b> Barres de chutes disponibles et attribuées au lot.</description></item>
    ///   <item><description><b>BarNew :</b> Barres neuves calculées et à sortir du stock.</description></item>
    ///   <item><description><b>OutOfStock :</b> Barres en rupture nécessitant une action manuelle.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers et responsables d'atelier.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page30"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour appliquer un style homogène
    /// et gérer automatiquement les événements du cycle de vie (Loaded, SizeChanged).</para>
    /// </summary>
    public partial class Page30 : Page_Generic
    {
        private readonly VM_Page30 _viewModel;

        public Page30()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page30>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            // Exécuter d'abord la logique générique
            base.OnPageLoaded();

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Réagit au redimensionnement de la fenêtre principale.
        /// </summary>
        protected override void OnPageResized()
        {
            base.OnPageResized(); // 🔹 Exécute la logique générique

            // Ajustements dynamiques
            AdjustScrollViewerHeight();
            AdjustTabControlHeight();
        }

        /// <summary>
        /// Applique le style et ajuste la mise en page spécifique.
        /// </summary>
        protected override void ApplyLayout()
        {
            // Exécuter d'abord la logique générique
            base.ApplyLayout();

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // ScrollViewers
            ApplyLayoutScrollViewer();

            // ListViews
            ApplyLayoutListView();

            // TabControl
            _controlStyler.StyleTabControl(MainTabControl);

            // TabItems
            ApplyLayoutTabItem();

            // Ajustements dynamiques
            AdjustScrollViewerHeight();
            AdjustTabControlHeight();
        }

        /// <summary>
        /// Applique le style des composants ScrollViewer.
        /// </summary>
        private void ApplyLayoutScrollViewer()
        {
            // ScrollViewers
            _controlStyler.StyleScrollViewer(BarDropScrollViewer, null, BarDropBorder, BarDropHeader);
            _controlStyler.StyleScrollViewer(BarNewScrollViewer, null, BarNewBorder, BarNewHeader);
            _controlStyler.StyleScrollViewer(OutOfStockScrollViewer, null, OutOfStockBorder, OutOfStockHeader);
        }

        /// <summary>
        /// Applique le style des composants ListView.
        /// </summary>
        private void ApplyLayoutListView()
        {
            // ScrollViewers
            _controlStyler.StyleListView(BarDropListView);
            _controlStyler.StyleListView(BarNewListView);
            _controlStyler.StyleListView(OutOfStockListView);
        }

        /// <summary>
        /// Applique le style des composants TabItem.
        /// </summary>
        private void ApplyLayoutTabItem()
        {
            // ScrollViewers
            var barDropTab = new TextBlock();
            _controlStyler.StyleTabItem(BarDropTabItem, barDropTab, _dictionary.GetText("P30_01"), 180);

            var barNewTab = new TextBlock();
            _controlStyler.StyleTabItem(BarNewTabItem, barNewTab, _dictionary.GetText("P30_03"), 180);

            var outOfStockTab = new TextBlock();
            _controlStyler.StyleTabItem(OutOfStockTabItem, outOfStockTab, _dictionary.GetText("P30_05"), 260);
        }

        /// <summary>
        /// Calcule dynamiquement la hauteur ddu ScrollViewer en fonction
        /// de la taille de la fenêtre principale.
        /// </summary>
        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 340;
            BarDropScrollViewer.Height = scrollViewerHeight;
            BarNewScrollViewer.Height = scrollViewerHeight;
            OutOfStockScrollViewer.Height = scrollViewerHeight;
        }

        /// <summary>
        /// Calcule dynamiquement la hauteur ddu TabContro en fonction
        /// de la taille de la fenêtre principale.
        /// </summary>
        private void AdjustTabControlHeight()
        {
            double tabControlHeight = _window.GetMainWindowHeight() - 240;
            MainTabControl.Height = tabControlHeight;
        }
    }
}