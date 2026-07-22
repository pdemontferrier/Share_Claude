using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase responsable de l'ajout d'une action client sur toutes les commandes d'un lot,
    /// avec mise à jour du statut et enregistrement dans l'historique des modifications.
    /// </summary>
    public class UC_CommandeClientAction : IU_CommandeClientAction
    {
        private readonly string _callee;
        private readonly IQ_CommandeClient _qhCommandeClient;
        private readonly IQ_CommandeClientActionType _qhActionType;
        private readonly IS_CommandeClientAction_Add _addAction;
        private readonly IS_CommandeClient_UpdateStatut _updateStatut;
        private readonly IS_CommandeClientModification_Add _addModification;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_UseCase _settingsUseCase;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_LogAndNotify _logAndNotify;

        public UC_CommandeClientAction(
            IQ_CommandeClient qhCommandeClient,
            IQ_CommandeClientActionType qhActionType,
            IS_CommandeClientAction_Add addAction,
            IS_CommandeClient_UpdateStatut updateStatut,
            IS_CommandeClientModification_Add addModification,
            IS_Settings_App settingsApp,
            IS_Settings_UseCase settingsUseCase,
             IS_Settings_User settingsUser,
           IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;

            _qhCommandeClient = qhCommandeClient;
            _qhActionType = qhActionType;
            _addAction = addAction;
            _updateStatut = updateStatut;
            _addModification = addModification;
            _settingsApp = settingsApp;
            _settingsUseCase = settingsUseCase;
            _settingsUser = settingsUser;
            _logAndNotify = logAndNotify;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int idAction, bool updateStatut, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // Eléments de paramétrage
            DateTime appDateTime = _settingsApp.GetAppDateTime();
            int appUserId = _settingsUser.GetAppUserId();
            int decoupeLotId = _settingsUseCase.GetDecoupeLotId();

            try
            {
                // Etape 1 : Rechercher l'action demandée
                var actionType = await _qhActionType.HandleGetByIdAsync(idAction);
                if (actionType == null)
                    return;

                // Etape 2 : Extraire le statut lié à l'action
                int idStatut = actionType.IdCcStatut;

                // Etape 3 : Rechercher toutes les CommanceClient lié au DecoupeLotId
                var commandes = await _qhCommandeClient.HandleGetByIdDecoupeLotAsync(decoupeLotId);

                if (commandes == null || !commandes.Any())
                    return;

                // Etape 4 : Traiter chaque commande du lot
                foreach (var commande in commandes)
                {
                    // Ajouter un enregistrement à CommandeClientAction
                    await _addAction.ExecuteAsync(commande, idAction, appDateTime, appUserId, callChain);

                    if(updateStatut)
                    {
                        // Mettre à jour le statut de la commande
                        await _updateStatut.ExecuteAsync(commande, idStatut, callChain);

                        // Ajouter une ligne d'historique dans la table CommandeClientModification
                        await _addModification.ExecuteAsync(commande, idStatut, appDateTime, appUserId, callChain);
                    }
                }
            }
            catch (Ex_Business ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_18", ex);
            }
            catch (Ex_Infrastructure ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_19", ex);
            }
            catch (Exception ex)
            {
                await _logAndNotify.ExecuteAsync("No_EC_20", ex);
            }
        }
    }
}