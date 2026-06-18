using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page96 — Menu horizontal de la messagerie inter-applications.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché sur la Page96,
    /// dédiée à la messagerie entre applications. Il permet de naviguer,
    /// d’actualiser la liste des messages reçus et envoyés, et de revenir
    /// à la page d’accueil.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Actualisation de la liste des messages.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page96"/></para>
    ///
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style, des icônes et du comportement communs.
    /// Aucun bouton supplémentaire n’est ajouté sur cette page.</para>
    /// </summary>
    public partial class MH_Page96 : MH_Page_Generic
    {
        public MH_Page96()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page96>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page96.
        /// (Aucune restriction particulière ici.)
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
