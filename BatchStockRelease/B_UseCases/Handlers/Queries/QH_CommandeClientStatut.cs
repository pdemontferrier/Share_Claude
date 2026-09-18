using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.Generic;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;
using BatchStockRelease.C_Infrastructure.Repositories.GestStock;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_CommandeClientStatut : QH_Generic<CommandeClientStatut>, IQ_CommandeClientStatut
    {
        public QH_CommandeClientStatut(IR_Generic<CommandeClientStatut> repository)
            : base(repository)
        {
        }


        // Requête spécifique :

    }
}