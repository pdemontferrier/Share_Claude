using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page23 – Page de forçage des barres neuves en rupture de stock.</b></para>
    ///
    /// <para><b>Contexte :</b> Cette page intervient lorsque certaines barres
    /// neuves d’un lot ne peuvent pas être approvisionnées. Elle permet au
    /// magasinier de décider de forcer l’approvisionnement partiel ou total.</para>
    ///
    /// <para><b>Objectif :</b> Offrir deux actions possibles via le Menu Horizontal :</para>
    /// <list type="bullet">
    ///   <item><description><b>Forçage du lot :</b> Autorise uniquement la découpe
    ///   des barres déjà disponibles.</description></item>
    ///   <item><description><b>Forçage complet :</b> Force l’approvisionnement des barres
    ///   en rupture et les rend visibles aux postes de découpe.</description></item>
    /// </list>
    ///
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home).</para>
    /// </summary>
    public class VM_MH_Page23 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        /// <summary>
        /// Service applicatif de gestion des paramètres utilisateur et du contexte de découpe.
        /// </summary>
        private readonly IS_Settings_UseCase _settings;

        /// <summary>
        /// Service de notification pour afficher des messages d’information ou d’avertissement.
        /// </summary>
        private readonly IS_Notification _notification;

        /// <summary>
        /// Cas d’usage responsable du forçage des barres neuves en rupture de stock.
        /// </summary>
        private readonly IU_BarNewForceOutOfStock _ucBarNewForceOutOfStock;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Commande permettant de forcer uniquement le lot sans forcer les barres en rupture.
        /// </summary>
        public ICommand ForceLotCommand { get; }

        /// <summary>
        /// Commande permettant de forcer le lot ainsi que les barres neuves en rupture.
        /// </summary>
        public ICommand ForceCompleteCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page23 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page23(
            IS_Settings_UseCase settings,
            IS_Notification notification,
            IU_BarNewForceOutOfStock ucBarNewForceOutOfStock)
        {
            _settings = settings;
            _notification = notification;
            _ucBarNewForceOutOfStock = ucBarNewForceOutOfStock;

            ForceLotCommand = new UT_RelayCommandArg0(ExecuteForceLot);
            ForceCompleteCommand = new UT_RelayCommandArg0(ExecuteForceLotAndBars);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// <para>Exécute le UseCase de forçage du lot de découpe sans modifier l’état des barres neuves.</para>
        /// <para>Étapes :</para>
        /// <list type="number">
        ///   <item><description>Vérifie qu’aucun autre processus n’est en cours.</description></item>
        ///   <item><description>Récupère l’identifiant du lot à forcer.</description></item>
        ///   <item><description>Affiche un avertissement si aucun lot n’est sélectionné.</description></item>
        ///   <item><description>Déclenche le UseCase <see cref="IU_BarNewForceOutOfStock"/> pour forcer le lot uniquement.</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteForceLot()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteForceLot));

            // Ne rien faire si déjà en cours
            if (IsProcessing)
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                // Appeller le usecase UC_BarDropStockRelease pour valider la sortie de la barre de chute affichée
                int decoupeLotId = _settings.GetDecoupeLotId();
                if (decoupeLotId == 0)
                {
                    _notification.Warning("No_Se_04");
                    return;
                }

                await _ucBarNewForceOutOfStock.ExecuteAsync(decoupeLotId, false, callChain);
            }
            finally
            {
                // Signaler la fin du process
                EndProcessing();
            }
        }

        /// <summary>
        /// <para>Exécute le UseCase de forçage du lot de découpe avec mise à jour des barres neuves en rupture.</para>
        /// <para>Étapes :</para>
        /// <list type="number">
        ///   <item><description>Vérifie qu’aucun autre processus n’est en cours.</description></item>
        ///   <item><description>Récupère l’identifiant du lot à forcer.</description></item>
        ///   <item><description>Affiche un avertissement si aucun lot n’est sélectionné.</description></item>
        ///   <item><description>Déclenche le UseCase <see cref="IU_BarNewForceOutOfStock"/> en mode complet.</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteForceLotAndBars()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteForceLotAndBars));

            // Ne rien faire si déjà en cours
            if (IsProcessing)
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                // Appeller le usecase UC_BarDropStockRelease pour valider la sortie de la barre de chute affichée
                int decoupeLotId = _settings.GetDecoupeLotId();
                if (decoupeLotId == 0)
                {
                    _notification.Warning("No_Se_04");
                    return;
                }

                await _ucBarNewForceOutOfStock.ExecuteAsync(decoupeLotId, true, callChain);
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