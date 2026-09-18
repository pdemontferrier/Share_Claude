using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Queries
{
    public class QH_TemporImport : QH_Generic<Tempor_Import>, IQ_TemporImport
    {
        public QH_TemporImport(IR_Generic<Tempor_Import> repository)
            : base(repository)
        {
        }

        // Requête spécifique :

    }
}