using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserAppMessage : QH_Generic<UserAppMessage>, IQ_UserAppMessage
    {
        private readonly IR_UserAppMessage _repositorySpecifique;
        private readonly IS_Settings_App _settingsApp;

        public QH_UserAppMessage(
            IR_UserAppMessage repository, 
            IS_Settings_App settingsApp)
            : base(repository)
        {
            _repositorySpecifique = repository;
            _settingsApp = settingsApp;
        }


        // Requête spécifique : Obtenir la liste des messages reçu
        public async Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync()
        {
            return await _repositorySpecifique.GetReceivedMessagesAsync(_settingsApp.GetAppId());
        }

        // Requête spécifique : Obtenir la liste des messages envoyés
        public async Task<List<UserAppMessage>> HandleGetMessagesSentAsync()
        {
            return await _repositorySpecifique.GetSentMessagesAsync(_settingsApp.GetAppId());
        }

        // Requête spécifique : Vérifier s'il existe des messages non lus
        public async Task<bool> HandleGetAnyMessageNotReadAsync()
        {
            return await _repositorySpecifique.HasUnreadMessagesAsync(_settingsApp.GetAppId());
        }
    }
}