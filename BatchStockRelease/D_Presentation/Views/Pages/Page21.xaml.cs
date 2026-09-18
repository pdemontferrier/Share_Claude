using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page21 — Interface de retour en stock des barres neuves non consommées.
    ///
    /// <para><b>Contexte :</b> Cette page permet de remettre en stock les barres
    /// neuves non consommées après la découpe, en sélectionnant la référence, la couleur
    /// et le conteneur de stockage proposé.</para>
    ///
    /// <para><b>Objectif :</b> Guider l’utilisateur dans le processus de réintégration
    /// des barres non utilisées dans le stock, selon leur type et leur emplacement.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>ReferenceComboBox :</b> Sélection de la référence de la barre.</description></item>
    ///   <item><description><b>CouleurComboBox :</b> Sélection de la couleur disponible pour la référence.</description></item>
    ///   <item><description><b>ConteneurComboBox :</b> Proposition d’un emplacement de stockage.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Magasiniers chargés du rangement et de la mise à jour du stock.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page21"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour bénéficier
    /// du système générique de stylisation et du cycle de vie (Loaded, SizeChanged).
    /// Toutes les actions de validation sont exécutées via le Menu Horizontal (MH_Page21),
    /// afin de prévenir les clics accidentels sur tablette.</para>
    /// </summary>
    public partial class Page21 : Page_Generic
    {
        private readonly VM_Page21 _viewModel;

        public Page21()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page21>();
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
        /// Applique les styles et met en forme la page.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Logique générique exécutée

            // Appliquer les styles à PageGrid
            _controlStyler.StylePage(PageGrid);

            // Appliquer les styles aux TextBlocks
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // Appliquer les styles aux ComboBox
            _controlStyler.StyleComboBox(ReferenceComboBox, 250.0);
            _controlStyler.StyleComboBox(CouleurComboBox, 250.0);
            _controlStyler.StyleComboBox(ConteneurComboBox, 250.0);

            // Appliquer les styles des boutons + et -
            _controlStyler.StylePlusMinusButton(PlusData, PlusText);
            _controlStyler.StylePlusMinusButton(MinusData, MinusText);
        }

        private void ReferenceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            // Afficher la ComboBox pour la Couleur
            CouleurTitle.Visibility = Visibility.Visible;
            CouleurComboBox.Visibility = Visibility.Visible;
            CouleurComboBox.Text = string.Empty;

            // Masquer la ComboBox pour le Conteneur
            ConteneurTitle.Visibility = Visibility.Hidden;
            ConteneurComboBox.Visibility = Visibility.Hidden;

            // Masquer la Quantité
            QuantiteStack.Visibility = Visibility.Hidden;
            StockStack.Visibility = Visibility.Hidden;
            StockLane.Visibility = Visibility.Hidden;
        }

        private void CouleurComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            // Afficher la ComboBox pour la Couleur
            ConteneurTitle.Visibility = Visibility.Visible;
            ConteneurComboBox.Visibility = Visibility.Visible;
            ConteneurComboBox.Text = string.Empty;

            // Masquer la Quantité
            QuantiteStack.Visibility = Visibility.Hidden;
            StockStack.Visibility = Visibility.Hidden;
            StockLane.Visibility = Visibility.Hidden;
        }

        private void ConteneurComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;

            var selectedConteneur = e.AddedItems[0] as string;
            if (selectedConteneur == null) return;

            // Mettre à jour la propriété Reference dans le ViewModel
            _viewModel.Conteneur = selectedConteneur;

            // Afficher les Quantité
            QuantiteStack.Visibility = Visibility.Visible;
            StockStack.Visibility = Visibility.Visible;
            StockLane.Visibility = Visibility.Visible;
            _viewModel.Quantite = 1;
        }
    }
}