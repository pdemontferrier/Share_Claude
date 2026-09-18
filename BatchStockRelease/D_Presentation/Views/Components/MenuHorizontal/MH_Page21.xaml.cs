using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page21 — Menu horizontal de la Page21.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché lors du processus
    /// de réintégration en stock des barres neuves non consommées.
    /// Il permet la navigation et la validation de l’opération.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement des données affichées.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    ///   <item><description><b>MH_Validate :</b> Validation du retour en stock des barres.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page21"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style et du comportement commun. La visibilité du bouton
    /// de validation dépend des droits utilisateur sur la Page21.</para>
    /// </summary>
    public partial class MH_Page21 : MH_Page_Generic
    {
        public MH_Page21()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page21>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page21.
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();

            if (FindName("MH_Validate") is Button validateButton)
            {
                validateButton.Visibility = _navigation.CanUpdate("Page21")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Applique les icônes spécifiques au menu de la Page21.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();

            if (FindName("MH_Validate") is Button validateButton)
                _controlStyler.StyleButton(validateButton, _icons.GetMH_Validate_Source());
        }
    }
}