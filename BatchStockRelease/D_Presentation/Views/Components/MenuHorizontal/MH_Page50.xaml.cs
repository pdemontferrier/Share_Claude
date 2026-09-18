using Microsoft.Extensions.DependencyInjection;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    public partial class MH_Page50 : MH_Page_Generic
    {
        /// <summary>
        /// MH_Page50 — Menu horizontal de la page 50.
        ///
        /// <para><b>Contexte :</b> Ce menu est affiché sur la Page50. 
        /// Il permet de naviguer, d’actualiser et de revenir à la page d’accueil.</para>
        ///
        /// <para><b>Objectif :</b> Fournir un accès rapide aux fonctions
        /// de navigation et de rafraîchissement de la page sans action
        /// de validation directe.</para>
        ///
        /// <list type="bullet">
        ///   <item><description><b>MH_Menu :</b> Accès au menu principal.</description></item>
        ///   <item><description><b>MH_Refresh :</b> Actualisation des données affichées.</description></item>
        ///   <item><description><b>MH_Previous :</b> Retour à la page précédente.</description></item>
        ///   <item><description><b>MH_Home :</b> Retour à la page d’accueil.</description></item>
        /// </list>
        ///
        /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page50"/></para>
        /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
        /// pour bénéficier du style, des icônes et du comportement communs.
        /// Aucune règle de sécurité supplémentaire n’est appliquée. </para>
        /// </summary>
        public MH_Page50()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page50>();
        }
    }
}