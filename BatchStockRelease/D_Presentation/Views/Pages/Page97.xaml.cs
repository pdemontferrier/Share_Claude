using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page97 — Interface d’administration des sessions utilisateurs.
    ///
    /// <para><b>Contexte :</b> Cette page permet aux administrateurs
    /// de superviser les connexions en cours et de gérer la disponibilité
    /// de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Offrir trois fonctions principales :</para>
    /// <list type="bullet">
    ///   <item><description><b>Verrouillage de l’application :</b> rend temporairement l’application inaccessible.</description></item>
    ///   <item><description><b>Déconnexion ciblée :</b> force la déconnexion d’un utilisateur sélectionné.</description></item>
    ///   <item><description><b>Déconnexion globale :</b> force la déconnexion de tous les utilisateurs connectés.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Administrateurs et responsables système.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page97"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier du cycle de vie générique, du style commun et de la gestion
    /// automatique des redimensionnements. Les actions de gestion sont exécutées à partir de boutons sur la page et
    /// via le Menu Horizontal (<c>MH_Page97</c>).</para>
    /// </summary>
    public partial class Page97 : Page_Generic
    {
        private readonly VM_Page97 _viewModel;

        public Page97()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page97>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Chargement des données à l’ouverture de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded();

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Réagit au redimensionnement de la fenêtre principale.
        /// </summary>
        protected override void OnPageResized()
        {
            base.OnPageResized();
            AdjustScrollViewerHeight();
        }

        /// <summary>
        /// Applique la mise en page et le style général.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout();

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // ScrollViewers
            _controlStyler.StyleScrollViewer(UserSessionScrollViewer, UserSessionTitle, UserSessionBorder, UserSessionHeader);

            // ListViews
            _controlStyler.StyleListView(UserSessionListView);

            // Ajuster la hauteur des ScrollViewer
            AdjustScrollViewerHeight();
        }

        /// <summary>
        /// Ajuste dynamiquement la hauteur du ScrollViewer.
        /// </summary>
        private void AdjustScrollViewerHeight()
        {
            double scrollViewerHeight = _window.GetMainWindowHeight() - 348;
            UserSessionScrollViewer.Height = scrollViewerHeight;
        }

    }
}
