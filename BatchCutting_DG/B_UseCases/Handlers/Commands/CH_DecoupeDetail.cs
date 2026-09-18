using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.Generic;
using BatchCutting_DG.B_UseCases.Handlers.Generic;
using System.Runtime.CompilerServices;

namespace BatchCutting_DG.B_UseCases.Handlers.Commands
{
    public class CH_DecoupeDetail : CH_Generic<DecoupeDetail>, IC_DecoupeDetail
    {
        private readonly IR_Generic<DecoupeDetail> _repository;

        public CH_DecoupeDetail(IR_Generic<DecoupeDetail> repository, IC_UserAppEventStore eventStore)
            : base(repository, eventStore)
        {
            _repository = repository;
        }

        // Commande spécifique : Mettre à jour un ensemble d'enregistrement pour un IdDecoupeBarre donné
        public async Task HandleDisableOptimisationForBarreAsync(int decoupeBarreId, string appService, string explicitCaller, [CallerMemberName] string actualCaller = "")
        {
            var all = await _repository.GetAllAsync();

            var toUpdate = all
                .Where(dd => dd.IdDecoupeBarre == decoupeBarreId)
                .ToList();

            foreach (var dd in toUpdate)
            {
                dd.ApproOptimBarreChute = false;
                dd.ApproOptimBarreNeuve = false;
            }

            string appServiceCaller = BuildAppServiceCaller(explicitCaller, actualCaller);
            await HandleUpdateRangeAsync(toUpdate, appService, appServiceCaller);
        }
    }
}