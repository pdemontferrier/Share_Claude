using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page99 — Interface d’avertissement pour accès non autorisé.
    ///
    /// <para><b>Contexte :</b> Cette page s’affiche lorsqu’un utilisateur
    /// tente d’accéder à une page pour laquelle il ne possède pas
    /// les droits nécessaires. Elle fait partie du mécanisme de
    /// double sécurité du système de navigation.</para>
    ///
    /// <para><b>Objectif :</b> Informer l’utilisateur que l’accès
    /// est refusé tout en maintenant la stabilité de l’application.
    /// Cette page peut aussi être affichée si un jeton de session
    /// ou un droit a expiré.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>PageTitleMain :</b> Titre principal signalant la restriction.</description></item>
    ///   <item><description><b>Message :</b> Texte d’information complémentaire.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs,
    /// notamment en cas de perte de session ou d’anomalie d’accès.</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page99"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/>
    /// pour bénéficier du cycle de vie standard et de la stylisation homogène.
    /// La page est statique et non interactive.</para>
    /// </summary>
    public partial class Page99 : Page_Generic
    {
        private readonly VM_Page99 _viewModel;

        public Page99()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page99>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Chargement du ViewModel à l’ouverture.
        /// </summary>
        protected override async void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Appel de la logique générique

            // Charger les données du ViewModel
            await _viewModel.LoadDataAsync();
        }

        /// <summary>
        /// Applique les styles généraux à la page et au titre.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Logique générique

            // Page Grid
            _controlStyler.StylePage(PageGrid);

            // PageTitle
            _controlStyler.StyleTextBlockPageTitle(PageTitleMain);
        }
    }
}