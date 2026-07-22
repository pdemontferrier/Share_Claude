using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.B_UseCases.Handlers.Generic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Handlers.Queries
{
    public class QH_PickingEmplacement : QH_Generic<PickingEmplacement>, IQ_PickingEmplacement
    {
        private readonly IR_PickingEmplacement _repositorySpecifique;
        private readonly IS_Settings_UseCase _settings;

        public QH_PickingEmplacement(IR_PickingEmplacement repository, IS_Settings_UseCase settings)
            : base(repository)
        {
            _repositorySpecifique = repository;
            _settings = settings;
        }

        // Requête spécifique :
        public async Task<List<PickingEmplacement>> HandleGetChariotListAsync()
        {
            return await _repositorySpecifique.GetChariotListAsync();
        }

    }
}