using System.Windows.Input;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;
using BatchStockRelease.D_Presentation.Utilities.RelayCommands;
using BatchStockRelease.D_Presentation.ViewModels.Generic;

namespace BatchStockRelease.D_Presentation.ViewModels.Components.MenuHorizontal
{
    /// <summary>
    /// <para><b>Menu horizontal de la Page21 – Page de retour en stock des barres neuves non consommées.</b></para>
    /// 
    /// <para><b>Contexte :</b> Ce menu permet de valider le retour en stock des barres
    /// neuves non consommées après la découpe, en sélectionnant la référence, la couleur
    /// et le conteneur de stockage proposé.</para>
    /// 
    /// <para><b>Objectif :</b> Guider l’utilisateur dans le processus de réintégration
    /// des barres non utilisées dans le stock, selon leur type et leur emplacement.</para>
    /// 
    /// <para>Hérite de <see cref="VM_MH_Page_Generic"/> pour disposer des commandes standard (Menu, Refresh, Previous, Home).</para>
    /// </summary>
    public class VM_MH_Page21 : VM_MH_Page_Generic
    {
        #region === Dépendances privées ===
        /// <summary>
        /// Service de paramètres applicatifs permettant d’accéder aux données de contexte.
        /// </summary>
        private readonly IS_Settings_UseCase _settings;

        /// <summary>
        /// Service de notification destiné à informer l’utilisateur (warnings, erreurs, etc.).
        /// </summary>
        private readonly IS_Notification _notification;

        /// <summary>
        /// Cas d’usage métier responsable de la réintégration des barres neuves dans le stock.
        /// </summary>
        private readonly IU_BarNewStockReturn _barNewStockReturn;
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Commande de validation du retour en stock des barres neuves.
        /// </summary>
        public ICommand ValidateReturnCommand { get; }
        #endregion

        #region === Constructeur ===
        /// <summary>
        /// Initialise le ViewModel du menu horizontal de la Page21 et configure les dépendances requises.
        /// </summary>
        public VM_MH_Page21(
            IS_Settings_UseCase settings,
            IS_Notification notification,
            IU_BarNewStockReturn barNewStockReturn)
        {
            _settings = settings;
            _notification = notification;
            _barNewStockReturn = barNewStockReturn;

            ValidateReturnCommand = new UT_RelayCommandArg0(ExecuteValidateReturn);
        }
        #endregion

        #region === Méthodes privées ===
        /// <summary>
        /// <para>Valide la réintégration des barres neuves non consommées en stock.</para>
        /// <para>Étapes principales :</para>
        /// <list type="number">
        ///   <item><description>Empêche un double déclenchement de la commande si un traitement est en cours.</description></item>
        ///   <item><description>Active un curseur d’attente durant le processus de retour.</description></item>
        ///   <item><description>Vérifie la validité de l’emplacement sélectionné dans les paramètres applicatifs.</description></item>
        ///   <item><description>Si valide, exécute le UseCase <see cref="IU_BarNewStockReturn"/>.</description></item>
        ///   <item><description>Sinon, notifie l’utilisateur qu’aucune sélection n’a été effectuée.</description></item>
        /// </list>
        /// </summary>
        private async void ExecuteValidateReturn()
        {
            // Conctruire la callChain
            string callChain = BuildFirstCallChain(nameof(ExecuteValidateReturn));

            // Ne rien faire si déjà en cours
            if (IsProcessing)
                return;

            try
            {
                // Signaler qu'un process est encours
                BeginProcessing();

                var stockQuantiteEmplacement = _settings.GetStockQuantiteEmplacement();
                if (stockQuantiteEmplacement == null)
                {
                    _notification.Warning("No_Se_02");
                    return;
                }

                await _barNewStockReturn.ExecuteAsync(
                    stockQuantiteEmplacement,
                    _settings.GetDecoupeBarreStockQuantite(),
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