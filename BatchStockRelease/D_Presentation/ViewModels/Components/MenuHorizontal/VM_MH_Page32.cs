using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page32 – Page de sortie de stock des barres neuves.</b></para>
    ///
    /// <para><b>Contexte :</b> Cette page permet la validation de la sortie de stock
    /// des barres neuves destinées à l’approvisionnement des postes de découpe.
    /// Elle intervient après la Page31 (sortie des chutes) et complète le processus
    /// de préparation du lot en affectant les barres neuves nécessaires.</para>
    ///
    /// <para><b>Objectif :</b> Offrir deux actions principales via le menu horizontal :</para>
    /// <list type="bullet">
    ///   <item><description><b>Validation de la sortie :</b> confirme la sortie des barres neuves pour le lot affiché.</description></item>
    ///   <item><description><b>Accès aux détails :</b> ouvre la Page30 afin d’afficher le détail complet des barres du lot.</description></item>
    /// </list>
    ///
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home)
    /// ainsi que des méthodes génériques <see cref="VM_MH_Page_Generic.BeginProcessing"/> et <see cref="VM_MH_Page_Generic.EndProcessing"/>
    /// pour la gestion de l’état de traitement.</para>
    /// </summary>
    public class VM_MH_Page32 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        /// <summary>
        /// Service applicatif contenant les paramètres de contexte du lot et des barres.
        /// </summary>
        private readonly IS_Settings_UseCase _settings;

        /// <summary>
        /// Cas d’usage principal chargé de valider la sortie du stock des barres neuves.
        /// </summary>
        private readonly IU_BarNewStockRelease _ucBarNewStockRelease;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Commande permettant de naviguer vers la Page30 pour afficher les détails du lot sélectionné.
        /// </summary>
        public ICommand DetailsCommand { get; }

        /// <summary>
        /// Commande permettant de valider la sortie du stock des barres neuves affichées.
        /// </summary>
        public ICommand ValidateCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page32 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page32(
            IS_Settings_UseCase settings,
            IU_BarNewStockRelease ucBarNewStockRelease)
        {
            _settings = settings;
            _ucBarNewStockRelease = ucBarNewStockRelease;

            DetailsCommand = new UT_RelayCommandArg0(ExecuteNavigateToPage30);
            ValidateCommand = new UT_RelayCommandArg0(ExecuteValidateBarNew);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// <para>Navigation vers la Page30 – permet de visualiser la liste complète des barres associées au lot sélectionné.</para>
        /// </summary>
        private void ExecuteNavigateToPage30()
        {
            _navigation.NavigateToNewPage("Page30");
        }

        /// <summary>
        /// <para>Valide la sortie des barres neuves du stock pour le lot en cours.</para>
        /// <para>Étapes :</para>
        /// <list type="number">
        ///   <item><description>Empêche un double déclenchement de la commande en cas de traitement en cours.</description></item>
        ///   <item><description>Active le curseur d’attente et marque le début du traitement (<see cref="VM_MH_Page_Generic.BeginProcessing"/>).</description></item>
        ///   <item><description>Récupère les informations du lot et la quantité à sortir depuis <see cref="IS_Settings_UseCase"/>.</description></item>
        ///   <item><description>Déclenche le UseCase <see cref="IU_BarNewStockRelease"/> pour enregistrer la sortie.</description></item>
        ///   <item><description>Réinitialise le curseur et l’état de traitement (<see cref="VM_MH_Page_Generic.EndProcessing"/>).</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteValidateBarNew()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteValidateBarNew));

            if (IsProcessing)
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                // Appeller le usecase UC_BarDropStockRelease pour valider la sortie de la barre de chute affichée
                int decoupeLotId = _settings.GetDecoupeLotId();
                int decoupeBarreIdStock = _settings.GetDecoupeBarreIdStock();
                int quantity = _settings.GetDecoupeBarreStockQuantite();

                await _ucBarNewStockRelease.ExecuteAsync(decoupeLotId, decoupeBarreIdStock, quantity, callChain);
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