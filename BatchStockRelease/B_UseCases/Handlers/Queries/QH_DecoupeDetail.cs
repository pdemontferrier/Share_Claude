using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeDetail : QH_Generic<DecoupeDetail>, IQ_DecoupeDetail
    {
        private readonly IR_DecoupeDetail _repositorySpecifique;

        public QH_DecoupeDetail(IR_DecoupeDetail repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne les découpes à approvisionner pour un lot, une machine, et un article donné
        public async Task<List<DecoupeDetail>> HandleGetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId)
        {
            return await _repositorySpecifique.GetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
        }

        // Requête spécifique : Retourne les découpes d'article composé (virtuel) pour un lot, une machine, et un article donné
        public async Task<List<DecoupeDetail>> HandleGetArticleComposeToBeAddedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId)
        {
            return await _repositorySpecifique.GetArticleComposeToBeAddedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
        }

        // Requête spécifique : Retourne la liste des machines de découpe ayant des découpe à approvisionner pour un lot donné
        public async Task<List<string>> HandleGetCuttingMachineListToBeSuppliedAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetCuttingMachineListToBeSuppliedAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des machines de découpe DG ayant des découpe à approvisionner pour un lot donné
        public async Task<List<string>> HandleGetCuttingMachineDGListToBeSuppliedAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetCuttingMachineDGListToBeSuppliedAsync(decoupeLotId);
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

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour une barre donnée
        public async Task<List<DecoupeDetail>> HandleGetAllByDecoupeBarreIdAsync(int decoupeBarreId)
        {
            return await _repositorySpecifique.GetAllByDecoupeBarreIdAsync(decoupeBarreId);
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 1 et pour un lot donné
        public async Task<List<DecoupeDetail>> HandleGetIndice1ByLotAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetIndice1ByLotAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 2 et pour un lot donné
        public async Task<List<DecoupeDetail>> HandleGetIndice2ByLotAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetIndice2ByLotAsync(decoupeLotId);
        }

        /// <summary>
        /// Requête spécifique : Retourne la liste complète des enregistrements DecoupeDetail pour un lot donné dans un ordre bien précis.
        /// </summary>
        public async Task<List<DecoupeDetail>> HandleGetBatchDecoupeDetailAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBatchDecoupeDetailAsync(decoupeLotId);
        }

        /// <summary>
        /// Requête spécifique : Retourne la liste des machines de découpe ayant des découpe pour un lot donné
        /// </summary>
        public async Task<List<string?>> HandleGetBatchCuttingMachineListAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBatchCuttingMachineListAsync(decoupeLotId);
        }

        /// <summary>
        /// Requête spécifique : Retourne la liste des MessageElumatec des enregistrements DecoupeDetail pour un lot donné dans un ordre bien précis.
        /// </summary>
        public async Task<List<string?>> HandleGetBatchDecoupeDetailMessageElumatecAsync(int decoupeLotId, string categorie4Value)
        {
            return await _repositorySpecifique.GetBatchDecoupeDetailMessageElumatecAsync(decoupeLotId, categorie4Value);
        }

    }
}