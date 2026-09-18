using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;

namespace BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock
{
    public interface IR_ChutesMagasin : IR_Generic<ChutesMagasin>
    {
        Task<List<ChutesMagasin>> GetByArticleInterneIdAsync(int articleInterneId);
        Task<ChutesMagasin?> GetByQrCodeAsync(string qrCode);
    }
}