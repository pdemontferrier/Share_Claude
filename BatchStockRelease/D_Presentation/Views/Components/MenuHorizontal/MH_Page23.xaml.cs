using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page23 — Menu horizontal de la Page23.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché lors du processus
    /// de forçage des barres neuves en rupture de stock. Il permet de
    /// valider le forçage du lot complet ou uniquement des barres en rupture.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement des données affichées.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    ///   <item><description><b>MH_ForceLot :</b> Forcer la découpe pour tout le lot.</description></item>
    ///   <item><description><b>MH_ForceLotBarre :</b> Forcer uniquement les barres en rupture.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page23"/></para>
    ///
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style et du comportement commun. Les règles de sécurité
    /// déterminent la visibilité des boutons de forçage en fonction des droits
    /// utilisateur sur la Page23.</para>
    /// </summary>
    public partial class MH_Page23 : MH_Page_Generic
    {
        public MH_Page23()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page23>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page23.
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();

            bool canUpdate = _navigation.CanUpdate("Page23");

            if (FindName("MH_ForceLot") is Button forceLotButton)
                forceLotButton.Visibility = canUpdate ? Visibility.Visible : Visibility.Collapsed;

            if (FindName("MH_ForceLotBarre") is Button forceLotBarreButton)
                forceLotBarreButton.Visibility = canUpdate ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Applique les icônes spécifiques au menu de la Page23.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();

            if (FindName("MH_ForceLot") is Button forceLotButton)
                _controlStyler.StyleButton(forceLotButton, _icons.GetMH_WarningTriangleOrange_Source());

            if (FindName("MH_ForceLotBarre") is Button forceLotBarreButton)
                _controlStyler.StyleButton(forceLotBarreButton, _icons.GetMH_WarningTriangleRed_Source());
        }
    }
}