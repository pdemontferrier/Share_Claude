using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page91 — Menu horizontal de la Page91.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché dans la page de
    /// sélection de la langue. Il permet de naviguer, actualiser
    /// la liste des langues disponibles, et revenir à l’accueil. </para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Actualisation de la liste des langues.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page91"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style et du comportement commun. Aucun bouton
    /// spécifique n’est ajouté à cette page.</para>
    /// </summary>
    public partial class MH_Page91 : MH_Page_Generic
    {
        public MH_Page91()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page91>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page91.
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
