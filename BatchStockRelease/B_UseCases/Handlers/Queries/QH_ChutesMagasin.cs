using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;


namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_ChutesMagasin : QH_Generic<ChutesMagasin>, IQ_ChutesMagasin
    {
        private readonly IR_ChutesMagasin _repositorySpecifique;

        public QH_ChutesMagasin(IR_ChutesMagasin repository)
            : base(repository)
        {
            _repositorySpecifique = repository;
        }


        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<List<ChutesMagasin>> HandleGetByArticleInterneIdAsync(int articleInterneId)
        {
            return await _repositorySpecifique.GetByArticleInterneIdAsync(articleInterneId);
        }

        // Requête spécifique : Retourne la liste des chutes pour un article interne donné
        public async Task<ChutesMagasin?> HandleGetByQrCodeAsync(string qrCode)
        {
            return await _repositorySpecifique.GetByQrCodeAsync(qrCode);
        }

    }
}