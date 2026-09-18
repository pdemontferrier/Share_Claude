using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page20 — Interface d’affectation d’un chariot à un lot à approvisionner.
    /// </summary>
    /// <para><b>Contexte :</b> Cette page est affichée lorsqu’un lot sélectionné
    /// dans la Page10 n’a pas encore de chariot attribué. Elle constitue une
    /// étape intermédiaire du processus BatchStockRelease avant la sortie de stock.</para>
    ///
    /// <para><b>Objectif :</b> Permettre à l’utilisateur de sélectionner un
    /// chariot disponible et de l’affecter au lot courant.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>ChariotComboBox :</b> liste des chariots disponibles.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers chargés de l’approvisionnement des barres.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page20"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour bénéficier
    /// du système de stylisation et du cycle de vie commun (Loaded, SizeChanged).</para>
    /// <para>Toutes les actions de validation sont exécutées via le Menu Horizontal,
    /// afin de prévenir les clics accidentels sur tablette.</para>
    public partial class Page20 : Page_Generic
    {
        private readonly VM_Page20 _viewModel;

        public Page20()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page20>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Exécution du comportement générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Applique les styles et met en forme la page.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Exécution de la logique générique de Page_Generic

            // Page et titres
            _controlStyler.StylePage(PageGrid);
            _controlStyler.StyleTextBlockTitle(ChariotTitle);

            // Composants
            _controlStyler.StyleComboBox(ChariotComboBox, 170.0);
        }
    }
}