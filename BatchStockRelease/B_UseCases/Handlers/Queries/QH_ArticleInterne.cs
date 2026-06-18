using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_ArticleInterne : QH_Generic<ArticleInterne>, IQ_ArticleInterne
    {
        public QH_ArticleInterne(IR_Generic<ArticleInterne> repository)
            : base(repository)
        {
        }

        // Requête spécifique :


    }
}