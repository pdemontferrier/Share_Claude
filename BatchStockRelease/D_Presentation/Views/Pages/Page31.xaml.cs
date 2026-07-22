using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page31 — Interface de sortie de stock des barres de chutes par référence.
    ///
    /// <para><b>Contexte :</b> Cette page guide le magasinier dans la sortie
    /// de stock des barres de chute, dans un ordre optimisé en fonction des
    /// zones de stockage, afin de réduire les déplacements. Les barre de chute
    /// sont sorties une à une.</para>
    ///
    /// <para><b>Objectif :</b> Afficher les barres à sortir, leur ordre de
    /// traitement et leur image de profil pour une identification rapide.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>BarListView :</b> Liste des barres à sortir.</description></item>
    ///   <item><description><b>ImageProfil :</b> Représentation visuelle du profil de la barre.</description></item>
    ///   <item><description><b>Plus/Minus :</b> Ajustement de la quantité sotie.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers préparant les barres neuves
    /// avant acheminement vers la découpe.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page31"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier du système générique de stylisation et du cycle de vie
    /// (Loaded, SizeChanged). Toutes les actions de validation sont déclenchées
    /// via le Menu Horizontal (MH_Page31).</para>
    /// </summary>
    public partial class Page31 : Page_Generic
    {
        private readonly VM_Page31 _viewModel;

        public Page31()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page31>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel lors du chargement de la page.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Appel du comportement générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Applique les styles à la page et à ses composants.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Exécute la logique générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // Appliquer les styles génériques aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // Appliquer les styles des boutons + et -
            _controlStyler.StylePlusMinusButton(PlusData, PlusText);
            _controlStyler.StylePlusMinusButton(MinusData, MinusText);
        }
    }
}