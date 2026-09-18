using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page10 — Menu horizontal de la page d’accueil.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché sur la Page10, qui
    /// présente la liste des lots à approvisionner. Il permet d’accéder
    /// aux différentes étapes du processus d’approvisionnement.</para>
    ///
    /// <para><b>Objectif :</b> Offrir un accès rapide aux actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Return :</b> Raffraichissement de la page.</description></item>
    ///   <item><description><b>MH_Return :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Integration :</b> Accès à la page d’intégration des chutes.</description></item>
    ///   <item><description><b>MH_Force :</b> Accès à la page de forçage des ruptures.</description></item>
    ///   <item><description><b>MH_Admin :</b> Accès à la page d’administration des utilisateurs.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page10"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier de la stylisation et du cycle de vie communs.
    /// Les règles de sécurité sont redéfinies pour contrôler la
    /// visibilité des boutons selon les droits utilisateur.</para>
    /// </summary>
    public partial class MH_Page10 : MH_Page_Generic
    {
        public MH_Page10()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page10>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page10.
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();

            // Retour
            if (FindName("MH_Return") is Button returnButton)
                returnButton.Visibility = _navigation.CanNavigate("Page21") ? Visibility.Visible : Visibility.Collapsed;

            // Intégration
            if (FindName("MH_Integration") is Button integrationButton)
                integrationButton.Visibility = _navigation.CanNavigate("Page22") ? Visibility.Visible : Visibility.Collapsed;

            // Forçage
            if (FindName("MH_Force") is Button forceButton)
                forceButton.Visibility = _navigation.CanNavigate("Page23") ? Visibility.Visible : Visibility.Collapsed;

            // Administration
            if (FindName("MH_Admin") is Button adminButton)
                adminButton.Visibility = _navigation.CanNavigate("Page97") ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Applique les icônes spécifiques aux boutons de la Page10.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();

            var icons = new (string name, System.Uri source)[]
            {
                ("MH_Return", _icons.GetMH_ReturnBack_Source()),
                ("MH_Integration", _icons.GetMH_Save_Source()),
                ("MH_Force", _icons.GetMH_WarningTriangleOrange_Source()),
                ("MH_Admin", _icons.GetMH_Admin_Source())
            };

            foreach (var (name, icon) in icons)
            {
                if (FindName(name) is Button btn)
                    _controlStyler.StyleButton(btn, icon);
            }
        }
    }
}