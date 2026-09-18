using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page90 — Menu horizontal de la Page90.
    ///
    /// <para><b>Contexte :</b> Ce menu est affiché dans la page de
    /// consultation du profil utilisateur (Page90). Il permet à
    /// l’utilisateur de naviguer et de visualiser les informations
    /// liées à son compte et à ses droits d’accès.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les actions suivantes :</para>
    /// <list type="bullet">
    ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
    ///   <item><description><b>MH_Refresh :</b> Rafraîchissement des informations utilisateur.</description></item>
    ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
    ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page90"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier du style, des icônes et du comportement commun.
    /// Aucun bouton spécifique n’est ajouté à cette page.</para>
    /// </summary>
    public partial class MH_Page90 : MH_Page_Generic
    {
        public MH_Page90()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page90>();
        }

        /// <summary>
        /// Applique les règles de sécurité spécifiques à la Page90.
        /// (Aucune règle supplémentaire n’est nécessaire ici.)
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