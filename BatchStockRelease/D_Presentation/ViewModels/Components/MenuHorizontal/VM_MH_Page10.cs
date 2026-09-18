using System.Windows.Input;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page10 – Page d’accueil de l’application BatchStockRelease.</b></para>
    /// <para>Ce composant gère la navigation vers les pages principales du processus métier :</para>
    /// <list type="bullet">
    /// <item><description><b>Page21</b> – Retour en stock des barres neuves</description></item>
    /// <item><description><b>Page22</b> – Page d’intégration des barres de chutes</description></item>
    /// <item><description><b>Page23</b> – Mode “Force” pour l’intégration manuelle ou reprise de lot</description></item>
    /// <item><description><b>Page97</b> – Accès à la page d’administration</description></item>
    /// </list>
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home).</para>
    /// </summary>
    public class VM_MH_Page10 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        // Ajouter ici les propriétés spécifiques.
        #endregion

        #region === Propriétés publiques ===
        /// <summary>Commande pour revenir à la Page21.</summary>
        public ICommand ReturnCommand { get; }

        /// <summary>Commande pour naviguer vers la Page22 (intégration des chutes et barres neuves).</summary>
        public ICommand IntegrationCommand { get; }

        /// <summary>Commande pour naviguer vers la Page23 (mode “Force”).</summary>
        public ICommand ForceCommand { get; }

        /// <summary>Commande pour accéder à la page d’administration (Page97).</summary>
        public ICommand AdminCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page10 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page10()
        {
            // Commandes spécifiques à la Page10
            ReturnCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage21);
            IntegrationCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage22);
            ForceCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage23);
            AdminCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage97);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// Navigation vers la Page21 – Retour en stock des barres neuves.
        /// </summary>
        private void ExecuteNavigateToPage21()
        {
            _navigation.NavigateToNewPage("Page21");
        }

        /// <summary>
        /// Navigation vers la Page22 – Intégration des barres de chutes.
        /// </summary>
        private void ExecuteNavigateToPage22()
        {
            _navigation.NavigateToNewPage("Page22");
        }

        /// <summary>
        /// Navigation vers la Page23 – Forçage des rupture de stock.
        /// </summary>
        private void ExecuteNavigateToPage23()
        {
            _navigation.NavigateToNewPage("Page23");
        }

        /// <summary>
        /// Navigation vers la Page97 – Administration de l’application.
        /// </summary>
        private void ExecuteNavigateToPage97()
        {
            _navigation.NavigateToNewPage("Page97");
        }
        #endregion
    }
}