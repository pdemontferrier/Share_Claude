using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.UserLogic;
using BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic;

namespace BatchStockRelease.B_UseCases.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase responsable de la mise à jour du statut de toutes les commandes d'un lot,
    /// et enregistrement dans l'historique des modifications.
    /// </summary>
    public class UC_CommandeClientStatut : IU_CommandeClientStatut
    {
        private readonly string _callee;
        private readonly IS_CommandeClientAction_Add _addAction;
        private readonly IS_CommandeClient_UpdateStatut _updateStatut;
        private readonly IS_CommandeClientModification_Add _addModification;
        private readonly IQ_CommandeClient _qhCommandeClient;
        private readonly IQ_CommandeClientActionType _qhActionType;
        private readonly IS_Settings_App _settingsApp;
        private readonly IS_Settings_UseCase _settingsUseCase;
        private readonly IS_Settings_User _settingsUser;
        private readonly IS_LogAndNotify _logAndNotify;

        public UC_CommandeClientStatut(
            IS_CommandeClientAction_Add addAction,
            IS_CommandeClient_UpdateStatut updateStatut,
            IS_CommandeClientModification_Add addModification,
            IQ_CommandeClient qhCommandeClient,
            IQ_CommandeClientActionType qhActionType,
            IS_Settings_App settingsApp,
            IS_Settings_UseCase settingsUseCase,
            IS_Settings_User settingsUser,
            IS_LogAndNotify logAndNotify)
        {
            _callee = GetType().Name;
            _addAction = addAction;
            _updateStatut = updateStatut;
            _addModification = addModification;
            _qhCommandeClient = qhCommandeClient;
            _qhActionType = qhActionType;
            _settingsApp = settingsApp;
            _settingsUseCase = settingsUseCase;
            _settingsUser = settingsUser;
            _logAndNotify = logAndNotify;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int idStatut, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            // Eléments de paramétrage
            DateTime appDateTime = _settingsApp.GetAppDateTime();
            int appUserId = _settingsUser.GetAppUserId();

            try
            {
                // Etape 1 : Rechercher toutes les CommanceClient lié au DecoupeLotId
                var commandes = await _qhCommandeClient.HandleGetByIdDecoupeLotAsync(_settingsUseCase.GetDecoupeLotId());

                if (commandes == null || !commandes.Any())
                    return;

                // Etape 2 : Traiter chaque commande du lot
                foreach (var commande in commandes)
                {
                    // Mettre à jour le statut de la commande
                    await _updateStatut.ExecuteAsync(commande, idStatut, callChain);

                    // Ajouter une ligne d'historique dans la table CommandeClientModification
                    await _addModification.ExecuteAsync(commande, idStatut, appDateTime, appUserId, callChain);
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