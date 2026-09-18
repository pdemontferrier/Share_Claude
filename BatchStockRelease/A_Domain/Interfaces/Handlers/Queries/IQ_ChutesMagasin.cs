using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Handlers.Queries
{
    public interface IQ_ChutesMagasin : IQ_Generic<ChutesMagasin>
    {
        // Requête spécifique :
        Task<List<ChutesMagasin>> HandleGetByArticleInterneIdAsync(int articleInterneId);
        Task<ChutesMagasin?> HandleGetByQrCodeAsync(string qrCode);
    }
}