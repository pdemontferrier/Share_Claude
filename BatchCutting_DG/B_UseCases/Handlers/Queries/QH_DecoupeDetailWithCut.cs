using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeDetailWithCut : IQ_DecoupeDetailWithCut
    {
        private readonly IR_DecoupeDetailWithCut _repository;

        public QH_DecoupeDetailWithCut(IR_DecoupeDetailWithCut repository)
        {
            _repository = repository;
        }

        public async Task<DTO_DecoupeDetailWithCut?> HandleGetFirstAsync()
        {
            return await _repository.GetFirstAsync();
        }

        public async Task<List<DTO_DecoupeDetailWithCut>> HandleGetAllByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            return await _repository.GetAllByLotAndMachineAsync(decoupeLotId, machineId);
        }
    }
}