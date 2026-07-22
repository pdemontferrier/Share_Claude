using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_UserAppErrorLog : QH_Generic<UserAppErrorLog>, IQ_UserAppErrorLog
    {
        public QH_UserAppErrorLog(IR_Generic<UserAppErrorLog> repository)
            : base(repository)
        {
        }

        // Requête spécifique :

    }
}