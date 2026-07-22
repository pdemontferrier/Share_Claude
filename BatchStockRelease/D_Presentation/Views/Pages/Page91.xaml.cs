using Microsoft.Extensions.DependencyInjection;
using System.Windows.Media.Imaging;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page91 — Interface de sélection de la langue de l’application.
    ///
    /// <para><b>Contexte :</b> Cette page permet à l’utilisateur de modifier
    /// la langue d’affichage de l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Proposer six langues au choix, illustrées par
    /// leur drapeau et un bouton de sélection unique :</para>
    /// <list type="bullet">
    ///   <item><description><b>FR :</b> Français</description></item>
    ///   <item><description><b>EN :</b> Anglais</description></item>
    ///   <item><description><b>DE :</b> Allemand</description></item>
    ///   <item><description><b>ES :</b> Espagnol</description></item>
    ///   <item><description><b>IT :</b> Italien</description></item>
    ///   <item><description><b>PT :</b> Portugais</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs connectés
    /// souhaitant changer la langue de l’interface.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page91"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/>
    /// pour bénéficier du cycle de vie générique et de la stylisation centralisée.
    /// La validation du changement de langue est effectuée via le Menu Horizontal
    /// (<c>MH_Page91</c>).</para>
    /// </summary>
    public partial class Page91 : Page_Generic
    {
        private readonly VM_Page91 _viewModel;
        private readonly IS_Flags _flag;

        public Page91()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page91>();
            _flag = App._serviceProvider.GetRequiredService<IS_Flags>();
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
        /// Applique les styles et charge les drapeaux pour chaque langue.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel de la logique générique

            _controlStyler.StylePage(PageGrid);
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            var buttonMapping = new[]
            {
                (Language1Button, Language1Image, Language1RadioButton, "FR"),
                (Language2Button, Language2Image, Language2RadioButton, "EN"),
                (Language3Button, Language3Image, Language3RadioButton, "DE"),
                (Language4Button, Language4Image, Language4RadioButton, "ES"),
                (Language5Button, Language5Image, Language5RadioButton, "IT"),
                (Language6Button, Language6Image, Language6RadioButton, "PT")
            };

            foreach (var (button, image, radio, code) in buttonMapping)
            {
                _controlStyler.StyleLanguageButton(button, image, radio, 350);
                image.Source = new BitmapImage(_flag.GetFlagUriFromLanguageCode(code));
            }
        }
    }
}