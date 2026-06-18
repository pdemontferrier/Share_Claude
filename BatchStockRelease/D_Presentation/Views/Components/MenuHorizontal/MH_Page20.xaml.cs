using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal;
using BatchStockRelease.D_Presentation.Views.Generic;

namespace BatchStockRelease.D_Presentation.Views.Components.MenuHorizontal
{
    /// <summary>
    /// MH_Page20 — Menu horizontal spécifique à la page 20.
    ///
    /// <para><b>Contexte :</b> Ce menu permet aux magasiniers de valider
    /// le chariot sélectionné pour un lot donné avant la sortie de stock.</para>
    ///
    /// <para><b>Objectif :</b> Fournir les fonctions de navigation standard
    /// (Menu, Refresh, Previous, Home) ainsi qu’un bouton de validation
    /// spécifique au processus d’approvisionnement.</para>
    ///
    /// <list type="bullet">
    ///   <item><description><b>MH_Validate :</b> Bouton permettant de valider le chariot sélectionné.</description></item>
    /// </list>
    ///
    /// <para><b>Vue modèle associée :</b> <see cref="VM_MH_Page20"/></para>
    /// <para><b>Spécificités techniques :</b> Hérite de <see cref="MH_Page_Generic"/>
    /// pour bénéficier de la structure, du style et du cycle de vie communs.
    /// Seules les règles de sécurité sont redéfinies pour afficher le bouton
    /// de validation si l’utilisateur possède les droits sur la Page20.</para>
    /// </summary>
    public partial class MH_Page20 : MH_Page_Generic
    {
        public MH_Page20()
        {
            InitializeComponent();
            DataContext = App._serviceProvider.GetRequiredService<VM_MH_Page20>();
        }

        /// <summary>
        /// Règle d’affichage du bouton de validation selon les droits.
        /// </summary>
        protected override void ApplySecurityRules()
        {
            base.ApplySecurityRules();

            if (FindName("MH_Validate") is Button validateButton)
            {
                validateButton.Visibility = _navigation.CanUpdate("Page20")
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Applique les styles spécifiques du menu horizontal.
        /// </summary>
        protected override void ApplyNavigationRules()
        {
            base.ApplyNavigationRules();

            if (FindName("MH_Validate") is Button validateButton)
                _controlStyler.StyleButton(validateButton, _icons.GetMH_Validate_Source());
        }
    }
}