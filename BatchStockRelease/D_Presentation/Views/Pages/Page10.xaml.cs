using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page10 — Page d’accueil affichant la liste des lots à approvisionner.
    /// </summary>
    /// <para><b>Contexte :</b> Cette page fait partie du processus global BatchStockRelease
    /// et constitue le point de départ des opérations d’approvisionnement. Elle s’affiche
    /// après la connexion réussie de l’utilisateur.</para>
    ///
    /// <para><b>Objectif :</b> Présenter deux listes distinctes :</para>
    /// <list type="bullet">
    ///   <item><description><b>Chutes :</b> lots à approvisionner en barres de chutes.</description></item>
    ///   <item><description><b>Barres neuves :</b> lots à approvisionner en barres neuves.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers et opérateurs responsables de la
    /// préparation des barres avant découpe.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page10"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour bénéficier
    /// du système générique de stylisation et de gestion du cycle de vie (Loaded, SizeChanged).</para>
    /// <para>Toutes les actions de validation sont exécutées via le Menu Horizontal,
    /// afin de prévenir les clics accidentels sur tablette.</para>
    public partial class Page10 : Page_Generic
    {
        private readonly VM_Page10 _viewModel;

        public Page10()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page10>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Exécute la logique générique de Page_Generic

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
        /// Applique les styles et ajuste la mise en page spécifique à la Page10.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Exécute la logique générique de Page_Generic

            // Mise en forme de la page
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(BarDropScrollViewer, BarDropTitle, BarDropBorder, BarDropHeader);
            _controlStyler.StyleScrollViewer(BarNewScrollViewer, BarNewTitle, BarNewBorder, BarNewHeader);

            // ListViews
            _controlStyler.StyleListView(BarDropListView);
            _controlStyler.StyleListView(BarNewListView);

            // Ajustement dynamique des hauteurs
            AdjustScrollViewerHeight();
        }

        /// <summary>
        /// Ajuste dynamiquement la hauteur des ScrollViewers selon la taille de la fenêtre.
        /// </summary>
        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 305;
            BarDropScrollViewer.Height = scrollViewerHeight;
            BarNewScrollViewer.Height = scrollViewerHeight;
        }
    }
}