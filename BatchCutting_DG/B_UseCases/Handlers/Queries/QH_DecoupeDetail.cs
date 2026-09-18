using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.B_UseCases.Handlers.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeDetail : QH_Generic<DecoupeDetail>, IQ_DecoupeDetail
    {
        private readonly IR_DecoupeDetail _repositorySpecifique;
        private readonly IS_Settings _settings;

        public QH_DecoupeDetail(IR_DecoupeDetail repository, IS_Settings settings)
            : base(repository)
        {
            _repositorySpecifique = repository;
            _settings = settings;
        }


        // Requête spécifique : Récupérer la première découpe prête à être réalisée, associée à une barre déjà sortie du stock
        public async Task<DecoupeDetail?> HandleGetFirstToCutAsync()
        {
            return await _repositorySpecifique.GetFirstToCutAsync(_settings.GetDecoupeLotId(), _settings.GetCuttingMachine());
        }

        // Requête spécifique : Retourne la liste des découpes pour une barre donnée par son Id
        public async Task<List<DecoupeDetail>> HandleGetAllForDecoupeBarreIdAsync()
        {
            return await _repositorySpecifique.GetAllForDecoupeBarreIdAsync(_settings.GetDecoupeBarreId());
        }

        // Requête spécifique : Retourne un bool si une barre (decoupeBarreId) à une ou des découpes de réalisées
        public async Task<bool> HandleExistsByDecoupeBarreIdAsync()
        {
            return await _repositorySpecifique.ExistsByDecoupeBarreIdAsync(_settings.GetDecoupeBarreId());
        }

        // Requête spécifique : Retourne les découpes à approvisionner pour un lot, une machine, et un article donné
        public async Task<List<DecoupeDetail>> HandleGetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId)
        {
            return await _repositorySpecifique.GetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
        }

        // Requête spécifique : Retourne la liste des machines de découpe ayant des découpe à approvisionner pour un lot donné
        public async Task<List<string>> HandleGetCuttingMachineListToBeSuppliedAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetCuttingMachineListToBeSuppliedAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des articles interne pour une machine de découpe ayant des découpe à approvisionner pour un lot donné
        public async Task<List<int>> HandleGetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId)
        {
            return await _repositorySpecifique.GetArticleInterneIdListToBeSuppliedAsync(decoupeLotId, decoupeMachineId);
        }

        // Requête spécifique : Retourne la liste des articles composés pour une machine de découpe ayant des découpe pour un lot donné (barre virtuelle)
        public async Task<List<int>> HandleGetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId)
        {
            return await _repositorySpecifique.GetArticleComposeIdListToBeAddedAsync(decoupeLotId, decoupeMachineId);
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 1 et pour un lot donné
        public async Task<List<DecoupeDetail>> HandleGetIndice1ByLotAsyncAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetIndice1ByLotAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 2 et pour un lot donné
        public async Task<List<DecoupeDetail>> HandleGetIndice2ByLotAsyncAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetIndice2ByLotAsync(decoupeLotId);
        }

    }
}