using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page20 – Page d’affectation d’un chariot à un lot à approvisionner.</b></para>
    /// <para><b>Contexte :</b> Cette page est affichée lorsqu’un lot sélectionné
    /// dans la <b>Page10</b> n’a pas encore de chariot attribué. 
    /// Elle constitue une étape intermédiaire du processus BatchStockRelease avant la sortie de stock.</para>
    /// <para>
    /// Ce menu horizontal permet à l’opérateur de valider le chariot sélectionné 
    /// avant d’initier le UseCase <see cref="IU_Page20Dispatch"/>, 
    /// responsable du déclenchement du processus de préparation.
    /// </para>
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home).</para>
    /// </summary>
    public class VM_MH_Page20 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        private readonly IS_Settings_UseCase _settings;
        private readonly IS_Notification _notification;
        private readonly IU_Page20Dispatch _ucPage20Dispatch;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Commande permettant de valider le chariot sélectionné et de lancer le UseCase associé.
        /// </summary>
        public ICommand ValidateChariotCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page20 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page20(
            IS_Settings_UseCase settings,
            IS_Notification notification,
            IU_Page20Dispatch ucPage20Dispatch)
        {
            _settings = settings;
            _notification = notification;
            _ucPage20Dispatch = ucPage20Dispatch;

            ValidateChariotCommand = new UT_RelayCommandArg0(ExecuteValidateChariot);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// <para>Valide le chariot sélectionné avant d’exécuter le UseCase <see cref="IU_Page20Dispatch"/>.</para>
        /// <para>Étapes :</para>
        /// <list type="number">
        /// <item><description>Empêche l’exécution concurrente si un traitement est déjà en cours.</description></item>
        /// <item><description>Affiche un curseur d’attente pour signaler le traitement.</description></item>
        /// <item><description>Vérifie la sélection d’un chariot dans <see cref="IS_Settings_UseCase"/>.</description></item>
        /// <item><description>Affiche une notification si aucun chariot n’est sélectionné.</description></item>
        /// <item><description>Déclenche le UseCase <see cref="IU_Page20Dispatch.ExecuteAsync"/> avec le contexte du lot courant.</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteValidateChariot()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteValidateChariot));

            // Ne rien faire si déjà en cours
            if (IsProcessing) 
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                // Appeller le service
                // Tester si un chariot à été sélectionné
                var selectedChariot = _settings.GetDecoupeBarreChariotDesignation();
                if (string.IsNullOrEmpty(selectedChariot))
                {
                    // Afficher une notification pour demander de sélectionner un chariot
                    _notification.Warning("No_Se_07");
                    return;
                }

                // Exécution du UseCase principal
                await _ucPage20Dispatch.ExecuteAsync(_settings.GetDecoupeLotId(), callChain);
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