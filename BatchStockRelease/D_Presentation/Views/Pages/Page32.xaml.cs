using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page32 — Interface de sortie de stock des barres neuves.
    ///
    /// <para><b>Contexte :</b> Cette page permet de préparer la sortie des barres
    /// neuves à approvisionner pour un lot donné. L’ordre de traitement suit
    /// la logique de déplacement optimisée dans les zones de stockage.</para>
    ///
    /// <para><b>Objectif :</b> Afficher les barres neuves à sortir, leur profil,
    /// et permettre l’ajustement visuel des quantités avant validation.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>BarNewListView :</b> Liste des barres neuves à sortir.</description></item>
    ///   <item><description><b>ImageProfil :</b> Représentation visuelle du profil de chaque barre.</description></item>
    ///   <item><description><b>Plus/Minus :</b> Ajustement de la quantité sortie.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers préparant les barres neuves avant découpe.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page32"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour
    /// bénéficier du système générique de stylisation et du cycle de vie
    /// (Loaded, SizeChanged). Les actions de sortie et validation sont
    /// exécutées via le Menu Horizontal (<c>MH_Page32</c>).</para>
    /// </summary>
    public partial class Page32 : Page_Generic
    {
        private readonly VM_Page32 _viewModel;

        public Page32()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page32>();
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
        /// Applique les styles à la page et à ses composants.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel de la logique générique

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