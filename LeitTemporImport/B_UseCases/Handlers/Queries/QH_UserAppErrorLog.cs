using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.B_UseCases.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.B_UseCases.Handlers.Queries
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