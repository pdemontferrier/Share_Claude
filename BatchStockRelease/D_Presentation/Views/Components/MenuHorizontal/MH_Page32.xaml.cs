using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page32 — Menu horizontal de la Page32.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché lors de la sortie
    /// des barres neuves. L’utilisateur peut valider la sortie du stock
    /// et afficher le détail des barres à prélever.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement des données affichées.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    ///   <item><description><b>MH_Details :</b> Affichage du détail de la barre sélectionnée.</description></item>
    ///   <item><description><b>MH_Validate :</b> Validation de la sortie de stock des barres neuves.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page32"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style et du comportement commun. La visibilité des boutons
    /// dépend des droits utilisateur sur la Page32.</para>
    /// </summary>
    public partial class MH_Page32 : MH_Page_Generic
    {
        public MH_Page32()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page32>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page32.
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();

            bool canUpdate = _navigation.CanUpdate("Page32");

            if (FindName("MH_Validate") is Button validateButton)
                validateButton.Visibility = canUpdate ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Applique les icônes spécifiques au menu de la Page32.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();

            if (FindName("MH_Details") is Button detailsButton)
                _controlStyler.StyleButton(detailsButton, _icons.GetMH_Details_Source());

            if (FindName("MH_Validate") is Button validateButton)
                _controlStyler.StyleButton(validateButton, _icons.GetMH_Validate_Source());
        }
    }
}