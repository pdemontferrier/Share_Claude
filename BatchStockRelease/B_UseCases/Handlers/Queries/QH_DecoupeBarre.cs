using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarre : QH_Generic<DecoupeBarre>, IQ_DecoupeBarre
    {
        private readonly IR_DecoupeBarre _repositorySpecifique;

        public QH_DecoupeBarre(IR_DecoupeBarre repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne les informations relatives au chariot utilisé pour un lot donné
        public async Task<(int chariotId, string chariotDesignation)> HandleGetChariotInfoAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetChariotInfoAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne une liste d'enregistrement pour un lot et un article interne donné
        public async Task<List<DecoupeBarre>> HandleGetAllForArticleInterneIdAsync(int decoupeLotId, int articleInterneId)
        {
            return await _repositorySpecifique.GetAllForArticleInterneIdAsync(decoupeLotId, articleInterneId);
        }

        // Requête spécifique : Retourne la liste des IdArticleInterne pour un lot donné, correspondant à des barres neuves non encore allouées ni sorties.
        public async Task<List<int>> HandleGetDistinctArticleInterneIdAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetDistinctArticleInterneIdAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des barres neuve pour un lot et un idStock donné
        public async Task<List<DecoupeBarre>> HandleGetAllBarNewByIdStock(int decoupeLotId, int idStock)
        {
            return await _repositorySpecifique.GetAllBarNewByIdStock(decoupeLotId, idStock);
        }

        #region Allocated

        // Requête spécifique : Retourne la liste des barres allouées pour un lot donné.
        public async Task<List<DecoupeBarre>> HandleGetAllocatedAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetAllocatedAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si il existe des barre allouées
        public async Task<bool> HandleCheckAllocated(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckAllocated(decoupeLotId);
        }

        #endregion

        #region Not Allocated

        // Requête spécifique : Retourne la liste des barres neuves non allouées pour un lot donné.
        public async Task<List<DecoupeBarre>> HandleGetBarNewNotAllocatedAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBarNewNotAllocatedAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si il existe des barre neuves non allouées
        public async Task<bool> HandleCheckBarNewNotAllocated(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckBarNewNotAllocated(decoupeLotId);
        }

        #endregion

        #region To release

        // Requête spécifique : Retourne la liste des barres de chute à approvisionner pour un lot donné
        public async Task<List<DecoupeBarre>> HandleGetBarDropToReleaseAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBarDropToReleaseAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si il existe des barre de chute à approvisionner
        public async Task<bool> HandleCheckBarDropToReleaseAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckBarDropToReleaseAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des barres neuves à approvisionner pour un lot donné
        public async Task<List<DecoupeBarre>> HandleGetBarNewToReleaseAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBarNewToReleaseAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si il existe des barre neuves à approvisionner
        public async Task<bool> HandleCheckBarNewToReleaseAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckBarNewToReleaseAsync(decoupeLotId);
        }

        // Requête spécifique : Retourne la liste des barres neuve allouée avec un IdStock inexistant.
        public async Task<List<DecoupeBarre>> HandleCheckBarNewAllocatedToReallocateAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBarNewAllocatedToReallocateAsync(decoupeLotId);
        }


        #endregion

        #region Out of Stock

        // Requête spécifique : Retourne la liste des barres neuves en rupture de stock pour un lot donné.
        public async Task<List<DecoupeBarre>> HandleGetBarNewOutOfStockAsync(int decoupeLotId)
        {
            return await _repositorySpecifique.GetBarNewOutOfStockAsync(decoupeLotId);
        }

        // Requête spécifique : Vérifier si il existe des barres neuves en rupture de stock
        public async Task<bool> HandleCheckBarNewOutOfStock(int decoupeLotId)
        {
            return await _repositorySpecifique.CheckBarNewOutOfStock(decoupeLotId);
        }

        #endregion
    }
}