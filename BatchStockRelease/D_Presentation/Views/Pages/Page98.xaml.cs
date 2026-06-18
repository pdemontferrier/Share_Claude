using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page98 — Page de présentation générale de l’application BatchStockRelease.
    ///
    /// <para><b>Contexte :</b> Cette page présente les informations générales de
    /// l’application, son objectif, son architecture logicielle et le numéro de version
    /// actuellement déployé.</para>
    ///
    /// <para><b>Objectif :</b> Fournir aux utilisateurs une vue d’ensemble sur :</para>
    /// <list type="bullet">
    ///   <item><description><b>Le contexte :</b> Digitalisation de l’atelier de fabrication.</description></item>
    ///   <item><description><b>L’objectif :</b> Automatiser les flux de stock et d’approvisionnement.</description></item>
    ///   <item><description><b>L’architecture :</b> Application WPF .NET 8 basée sur Clean Architecture et MVVM.</description></item>
    ///   <item><description><b>La version :</b> Numéro de build affiché dynamiquement via le ViewModel.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs de l’application, ainsi
    /// que les administrateurs souhaitant identifier la version installée.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page98"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier de la stylisation générique, de la gestion du cycle de vie et du
    /// chargement automatique des ressources multilingues.</para>
    /// </summary>
    public partial class Page98 : Page_Generic
    {
        private readonly VM_Page98 _viewModel;

        public Page98()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page98>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les informations lors de l’ouverture de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Comportement générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Applique la mise en page et le style du document de présentation.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel du comportement générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // AppInfoDoc
            _controlStyler.StyleAppInfoDoc(AppInfoDoc);
        }
    }
}