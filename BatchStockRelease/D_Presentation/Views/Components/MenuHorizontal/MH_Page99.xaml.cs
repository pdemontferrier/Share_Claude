using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page99 — Menu horizontal de la page d’accès non autorisé.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché sur la Page99 lorsque
    /// l’utilisateur tente d’accéder à une page dont il n’a pas les droits.
    /// Il permet une navigation limitée pour revenir à un état valide. </para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement de la page d’erreur.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil (Page10).</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page99"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour appliquer automatiquement le style et la logique de navigation.
    /// Aucun bouton spécifique n’est ajouté à cette page. </para>
    /// </summary>
    public partial class MH_Page99 : MH_Page_Generic
    {
        public MH_Page99()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page99>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page99.
        /// (Aucune restriction supplémentaire n’est nécessaire.)
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