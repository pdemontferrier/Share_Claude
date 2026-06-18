using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page22 — Interface d’intégration en stock des barres de chutes générées à la découpe.
    ///
    /// <para><b>Contexte :</b> Cette page intervient en fin de cycle de découpe.
    /// Elle permet à l’opérateur de scanner les barres de chutes pour les réintégrer
    /// dans le stock et les rendre à nouveau disponibles pour de futurs lots.</para>
    ///
    /// <para><b>Objectif :</b> Identifier et marquer comme “en stock” les chutes valides
    /// scannées via leur QR Code, en mettant à jour les tables de gestion de stock.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>QrCodeInput :</b> Champ de saisie pour le scan du QR Code.</description></item>
    ///   <item><description><b>DoneImage :</b> Indicateur visuel pour une intégration réussie.</description></item>
    ///   <item><description><b>NotFoundImage :</b> Indicateur visuel pour un QR Code inconnu.</description></item>
    ///   <item><description><b>NotDoneImage :</b> Indicateur visuel pour un QR Code déjà traité.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Opérateurs de découpe et magasiniers en charge du rangement.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page22"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour bénéficier
    /// du système générique de stylisation et du cycle de vie (Loaded, SizeChanged).
    /// Toutes les validations sont déclenchées via le Menu Horizontal (MH_Page22) pour éviter
    /// les interactions accidentelles sur tablette.</para>
    /// </summary>
    public partial class Page22 : Page_Generic
    {
        private readonly VM_Page22 _viewModel;
        private readonly IS_Icons _icons;

        public Page22()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page22>();
            _icons = App._serviceProvider.GetRequiredService<IS_Icons>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Charge les données du ViewModel au démarrage.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Exécute la logique générique

            // Charger les données du View Model
            await _viewModel.LoadDataAsync();

            // Focus initial sur la zone de scan
            QrCodeInput.Focus();
        }

        /// <summary>
        /// Applique les styles visuels à la page et initialise les icônes.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Exécute la logique générique de Page_Generic

            _controlStyler.StylePage(PageGrid);
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // Icônes
            DoneImage.Source = new BitmapImage(_icons.GetMH_Validate_Source());
            NotFoundImage.Source = new BitmapImage(_icons.GetMH_WarningTriangleRed_Source());
            NotDoneImage.Source = new BitmapImage(_icons.GetMH_WarningTriangleOrange_Source());
        }

        /// <summary>
        /// Événement déclenché lors de la validation du QR Code (touche Entrée).
        /// </summary>
        private async void QrCodeInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string qrCode = QrCodeInput.Text.Trim();

                // Vide la zone après lecture
                QrCodeInput.Clear();

                // Remet le focus
                QrCodeInput.Focus();

                // Lance le traitement du QR Code
                await _viewModel.ProcessQrCodeAsync(qrCode);
            }
        }
    }
}