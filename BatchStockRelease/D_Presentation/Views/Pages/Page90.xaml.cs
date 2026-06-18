using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;
using System.Windows.Controls;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page90 — Interface d’affichage des informations relatives à l’utilisateur connecté.
    ///
    /// <para><b>Contexte :</b> Cette page affiche les informations personnelles
    /// de l’utilisateur ainsi que ses droits d’accès aux différentes pages
    /// de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Fournir une vue centralisée sur les données
    /// de l’utilisateur connecté :</para>
    /// <list type="bullet">
    ///   <item><description><b>Onglet 1 :</b> Informations personnelles (nom, rôle, adresse e-mail, date de dernière connexion).</description></item>
    ///   <item><description><b>Onglet 2 :</b> Droits d’accès (lecture, écriture, suppression) pour chaque page de l’application.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs connectés,
    /// notamment les administrateurs et responsables d’équipe.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page90"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier du cycle de vie générique (Loaded, SizeChanged) et du
    /// système de stylisation centralisé. Cette page est purement informative,
    /// sans action de validation directe : les modifications éventuelles
    /// sont gérées via le menu horizontal (MH_Page90).</para>
    /// </summary>
    public partial class Page90 : Page_Generic
    {
        private readonly VM_Page90 _viewModel;

        public Page90()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page90>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les informations de l’utilisateur lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Exécute la logique générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Réagit au redimensionnement de la fenêtre principale.
        /// </summary>
        protected override void OnPageResized()
        {
            base.OnPageResized(); // 🔹 Exécute la logique générique

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();
        }

        /// <summary>
        /// Applique les styles et la mise en page de la Page90.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel de la logique générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // Appliquer les styles aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // TabControl
            ApplyLayoutTabContro();

            // Style UserDetailsBorder
            ApplyLayouteBorder();

            // ScrollViewers
            ApplyLayoutScrollViewer();

            // ListViews
            ApplyLayoutListView();

            // TabItems
            ApplyLayoutTabItem();

            // Ajuster la hauteur des TabControl
            AdjustControlsHeight();
        }

        /// <summary>
        /// Applique le style des composants TabContro.
        /// </summary>
        private void ApplyLayoutTabContro()
        {
            _controlStyler.StyleTabControl(MainTabControl);
        }

        /// <summary>
        /// Applique le style des composants ScrollViewer.
        /// </summary>
        private void ApplyLayouteBorder()
        {
            _controlStyler.StyleBorder(UserDetailsBorder);
        }

        /// <summary>
        /// Applique le style des composants ScrollViewer.
        /// </summary>
        private void ApplyLayoutScrollViewer()
        {
            _controlStyler.StyleScrollViewer(UserAccessScrollViewer, null, UserAccessBorder, UserAccessHeader01, UserAccessHeader02,
                UserAccessHeader03, UserAccessHeader04, UserAccessHeader05, UserAccessHeader06, UserAccessHeader07, UserAccessHeader08,
                UserAccessHeader09, UserAccessHeader10, UserAccessHeader11);
        }

        /// <summary>
        /// Applique le style des composants ListView.
        /// </summary>
        private void ApplyLayoutListView()
        {
            _controlStyler.StyleListView(UserAccessListView);
        }

        /// <summary>
        /// Applique le style des composants TabItem.
        /// </summary>
        private void ApplyLayoutTabItem()
        {
            var UserDetailsTab = new TextBlock();
            _controlStyler.StyleTabItem(UserDetailsTabItem, UserDetailsTab, _dictionary.GetText("P90_10"), 150);
            var UUserAccessTab = new TextBlock();
            _controlStyler.StyleTabItem(UserAccessTabItem, UUserAccessTab, _dictionary.GetText("P90_11"), 150);
        }

        /// <summary>
        /// Ajuste dynamiquement la hauteur des ScrollViewer en fonction de la taille de la fenêtre principale.
        /// </summary>
        private void AdjustControlsHeight()
        {
            double tabControlHeight = _window.GetMainWindowHeight() - 220;
            MainTabControl.Height = tabControlHeight;

            double scrollViewerHeight = tabControlHeight - 93;
            UserAccessScrollViewer.Height = scrollViewerHeight;
        }
    }
}