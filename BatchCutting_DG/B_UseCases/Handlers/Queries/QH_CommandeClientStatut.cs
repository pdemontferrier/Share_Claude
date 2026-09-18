using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
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