using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page98 — Menu horizontal de la page de présentation de l’application.
    ///
    /// <para><b>Contexte :</b> Ce menu accompagne la Page98, qui présente
    /// le contexte, les objectifs et les informations techniques de
    /// l’application BatchStockRelease.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement de la page d’information.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page98"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour appliquer automatiquement le style, les icônes et les règles
    /// de navigation standard. Aucun bouton spécifique n’est ajouté ici.</para>
    /// </summary>
    public partial class MH_Page98 : MH_Page_Generic
    {
        public MH_Page98()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page98>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page98.
        /// (Aucune restriction particulière.)
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();
        }

        /// <summary>
        /// Applique les icônes standard du menu horizontal.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();
        }
    }
}