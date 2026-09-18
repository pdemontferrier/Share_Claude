using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_UserAppMessage : QH_Generic<UserAppMessage>, IQ_UserAppMessage
    {
        private readonly IR_UserAppMessage _repositorySpecifique;
        private readonly IS_Settings _settings;

        public QH_UserAppMessage(IR_UserAppMessage repository, IS_Settings settings)
            : base(repository)
        {
            _repositorySpecifique = repository;
            _settings = settings;
        }


        // Requête spécifique : Obtenir la liste des messages reçu
        public async Task<List<UserAppMessage>> HandleGetMessagesReceivedAsync()
        {
            return await _repositorySpecifique.GetReceivedMessagesAsync(_settings.GetAppID());
        }

        // Requête spécifique : Obtenir la liste des messages envoyés
        public async Task<List<UserAppMessage>> HandleGetMessagesSentAsync()
        {
            return await _repositorySpecifique.GetSentMessagesAsync(_settings.GetAppID());
        }

        // Requête spécifique : Vérifier s'il existe des messages non lus
        public async Task<bool> HandleGetAnyMessageNotReadAsync()
        {
            return await _repositorySpecifique.HasUnreadMessagesAsync(_settings.GetAppID());
        }
    }
}