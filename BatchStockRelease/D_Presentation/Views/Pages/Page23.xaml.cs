using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page23 — Interface de forçage des barres neuves en rupture de stock.
    ///
    /// <para><b>Contexte :</b> Cette page intervient lorsque certaines barres
    /// neuves d’un lot ne peuvent pas être approvisionnées. Elle permet au
    /// magasinier de décider de forcer l’approvisionnement partiel ou total.</para>
    ///
    /// <para><b>Objectif :</b> Offrir deux actions possibles via le Menu Horizontal :</para>
    /// <list type="bullet">
    ///   <item><description><b>Forçage du lot :</b> Autorise uniquement la découpe
    ///   des barres déjà disponibles.</description></item>
    ///   <item><description><b>Forçage complet :</b> Force l’approvisionnement des barres
    ///   en rupture et les rend visibles aux postes de découpe.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers et responsables d’atelier.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page23"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier du système générique de stylisation et du cycle de vie
    /// (Loaded, SizeChanged). Toutes les actions de validation sont exécutées
    /// via le Menu Horizontal (MH_Page23).</para>
    /// </summary>
    public partial class Page23 : Page_Generic
    {
        private readonly VM_Page23 _viewModel;

        public Page23()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page23>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Appel de la logique générique

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
        }

        /// <summary>
        /// Applique les styles à la page et aux composants.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Logique générique exécutée

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(BatchScrollViewer, BatchTitle, BatchBorder, BatchHeader);
            _controlStyler.StyleScrollViewer(BarNewScrollViewer, BarNewTitle, BarNewBorder, BarNewHeader);

            // ListViews
            _controlStyler.StyleListView(BatchListView);
            _controlStyler.StyleListView(BarNewListView);

            AdjustScrollViewerHeight();
        }

        /// <summary>
        /// Ajuste dynamiquement la hauteur des zones de défilement selon la fenêtre principale.
        /// </summary>
        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 305;
            BatchScrollViewer.Height = scrollViewerHeight;
            BarNewScrollViewer.Height = scrollViewerHeight;
        }
    }
}


/*


using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    public partial class Page23 : Page
    {
        private readonly VM_Page23 _viewModel;
        private readonly IS_ControlStyler _controlStyler;
        private readonly IS_Window _window;

        public Page23()
        {
            InitializeComponent();
            _viewModel = App.ServiceProvider.GetRequiredService<VM_Page23>();
            _controlStyler = App.ServiceProvider.GetRequiredService<IS_ControlStyler>();
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
            AdjustScrollViewerHeight();
        }

        // Méthodes relatives aux composants de la page
        private void StyleControls()
        {
            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(BatchScrollViewer, BatchTitle, BatchBorder, BatchHeader);
            _controlStyler.StyleScrollViewer(BarNewScrollViewer, BarNewTitle, BarNewBorder, BarNewHeader);

            // ListViews
            _controlStyler.StyleListView(BatchListView);
            _controlStyler.StyleListView(BarNewListView);

            // Ajuster la hauteur des ScrollViewer
            AdjustScrollViewerHeight();
        }


        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 305;
            BatchScrollViewer.Height = scrollViewerHeight;
            BarNewScrollViewer.Height = scrollViewerHeight;
        }
    }
}*/
