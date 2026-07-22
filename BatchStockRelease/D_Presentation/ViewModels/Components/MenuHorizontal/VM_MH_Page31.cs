using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page31 – Page de sortie de stock des barres de chutes par référence.</b></para>
    ///
    /// <para><b>Contexte :</b> Cette page guide le magasinier dans la sortie
    /// de stock des barres de chute, dans un ordre optimisé en fonction des
    /// zones de stockage, afin de réduire les déplacements. Les barres de chute
    /// sont sorties une à une, selon la séquence déterminée par le système.</para>
    ///
    /// <para><b>Objectif :</b> Permettre deux actions principales via le menu horizontal :</para>
    /// <list type="bullet">
    ///   <item><description><b>Validation de la sortie :</b> confirme la sortie de la barre affichée et met à jour le stock.</description></item>
    ///   <item><description><b>Affichage des détails :</b> permet de visualiser la Page30 présentant la liste complète des barres du lot sélectionné.</description></item>
    /// </list>
    ///
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home).</para>
    /// </summary>
    public class VM_MH_Page31 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        /// <summary>
        /// Service de configuration applicative permettant d’accéder au contexte courant (lot, barre, stock).
        /// </summary>
        private readonly IS_Settings_UseCase _settingsUseCase;

        /// <summary>
        /// Cas d’usage principal permettant de valider la sortie de stock d’une barre de chute.
        /// </summary>
        private readonly IU_BarDropStockRelease _ucBarDropStockRelease;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Commande permettant de naviguer vers la Page30 pour afficher le détail complet des barres du lot.
        /// </summary>
        public ICommand DetailsCommand { get; }

        /// <summary>
        /// Commande permettant de valider la sortie de la barre de chute affichée.
        /// </summary>
        public ICommand ValidateCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page31 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page31(
            IS_Settings_UseCase settingsUseCase,
            IU_BarDropStockRelease ucBarDropStockRelease)
        {
            _settingsUseCase = settingsUseCase;
            _ucBarDropStockRelease = ucBarDropStockRelease;

            DetailsCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage30);
            ValidateCommand = new UT_RelayCommandArg0(ExecuteValidateBarDrop);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// <para>Navigation vers la Page30 – affichage du détail complet des barres du lot sélectionné.</para>
        /// <para>Permet au magasinier de consulter la liste des barres déjà sorties ou restantes à traiter.</para>
        /// </summary>
        private void ExecuteNavigateToPage30()
        {
            _navigation.NavigateToNewPage("Page30");
        }

        /// <summary>
        /// <para>Valide la sortie de la barre de chute affichée à l’écran et met à jour les informations de stock.</para>
        /// <para>Étapes :</para>
        /// <list type="number">
        ///   <item><description>Empêche les exécutions multiples en vérifiant l’état <see cref="BatchStockRelease.D_Presentation.ViewModels.Generic.VM_MH_Page_Generic.IsProcessing"/>.</description></item>
        ///   <item><description>Active le curseur d’attente pendant le traitement.</description></item>
        ///   <item><description>Récupère les informations nécessaires depuis <see cref="IS_Settings_UseCase"/> (lot, barre, stock, quantité).</description></item>
        ///   <item><description>Déclenche le UseCase <see cref="IU_BarDropStockRelease"/> pour valider la sortie en base de données.</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteValidateBarDrop()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteValidateBarDrop));

            // Ne rien faire si déjà en cours
            if (IsProcessing)
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                // Appeller le usecase UC_BarDropStockRelease pour valider la sortie de la barre de chute affichée
                int decoupeLotId = _settingsUseCase.GetDecoupeLotId();
                int decoupeBarreId = _settingsUseCase.GetDecoupeBarreId();
                int decoupeBarreIdStock = _settingsUseCase.GetDecoupeBarreIdStock();
                int quantity = _settingsUseCase.GetDecoupeBarreStockQuantite();

                await _ucBarDropStockRelease.ExecuteAsync(
                    decoupeLotId,
                    decoupeBarreId,
                    decoupeBarreIdStock,
                    quantity,
                    callChain);
            }
            finally
            {
                // Signaler la fin du process
                EndProcessing();
            }
        }
        #endregion
    }
}