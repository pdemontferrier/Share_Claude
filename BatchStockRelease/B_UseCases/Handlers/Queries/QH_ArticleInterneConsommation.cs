using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_ArticleInterneConsommation : QH_Generic<ArticleInterneConsommation>, IQ_ArticleInterneConsommation
    {
        public QH_ArticleInterneConsommation(IR_Generic<ArticleInterneConsommation> repository)
            : base(repository)
        {
        }

        // Requête spécifique :


    }
}