using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using BatchStockRelease.D_Presentation.ViewModels.Pages;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Pages
{
    /// <summary>
    /// Page00 — Interface de connexion de l'utilisateur à l'application BatchStockRelease.
    ///
    /// <para><b>Contexte :</b> Cette page constitue le point d’entrée principal du processus
    /// BatchStockRelease. Elle permet d’établir la session utilisateur avant toute interaction
    /// avec le stock ou les lots de fabrication.</para>
    ///
    /// <para><b>Objectif :</b> Permettre la saisie et la validation des informations
    /// d'identification de l'utilisateur :</para>
    /// <list type="bullet">
    ///   <item><description><b>Login :</b> Identifiant utilisateur.</description></item>
    ///   <item><description><b>Password :</b> Mot de passe associé.</description></item>
    /// </list>
    ///
    /// <para><b>Utilisateurs cibles :</b> Tous les utilisateurs de l’ERP
    /// (magasiniers, responsables d’atelier, administrateurs).</para>
    /// <para><b>Vue modèle associée :</b> <see cref="VM_Page00"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="Page_Generic"/> pour bénéficier
    /// du système générique de stylisation et de gestion du cycle de vie (Loaded, SizeChanged).</para>
    /// </summary>
    public partial class Page00 : Page_Generic
    {
        private readonly VM_Page00 _viewModel;

        public Page00()
        {
            InitializeComponent();
            _viewModel = App._serviceProvider.GetRequiredService<VM_Page00>();
            this.DataContext = _viewModel;
        }

        /// <summary>
        /// Gère les actions à effectuer lorsque la page est chargée.
        /// </summary>
        protected override void OnPageLoaded()
        {
            base.OnPageLoaded(); // 🔹 Appel de la logique générique

            // Appliquer les styles spécifiques à la page
            ApplyLayout();

            // Focus initial sur le champ de login
            LoginInput.Focus();
        }

        /// <summary>
        /// Applique les styles et met en forme la page.
        /// </summary>
        protected override void ApplyLayout()
        {
            base.ApplyLayout(); // 🔹 Appel de la logique générique

            // Style général
            _controlStyler.StylePage(PageGrid);

            // Texte
            _controlStyler.ApplyStylesToTextBlocks(PageGrid);

            // Composants d'identification
            _controlStyler.StyleBorder(IdentificationBorder);
            _controlStyler.StylePageOOControls(LoginBorder, PasswordBorder, PasswordInput, LoginButton, LoginButtonText);

            // Alignement
            IdentificationData.HorizontalAlignment = HorizontalAlignment.Center;
        }
    }
}