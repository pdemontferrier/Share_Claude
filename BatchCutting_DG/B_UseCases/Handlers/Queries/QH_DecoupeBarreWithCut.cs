using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.B_UseCases.Handlers.Queries
{
    public class QH_DecoupeBarreWithCut : IQ_DecoupeBarreWithCut
    {
        private readonly IR_DecoupeBarreWithCut _repository;

        public QH_DecoupeBarreWithCut(IR_DecoupeBarreWithCut repository)
        {
            _repository = repository;
        }

        public async Task<DTO_DecoupeBarreWithCut?> HandleGetFirstAsync()
        {
            return await _repository.GetFirstAsync();
        }

        public async Task<List<DTO_DecoupeBarreWithCut>> HandleGetAllByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            return await _repository.GetAllByLotAndMachineAsync(decoupeLotId, machineId);
        }

        public async Task<List<DTO_DecoupeBarreWithCut>> HandleGetAllDropBarByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            return await _repository.GetAllDropBarByLotAndMachineAsync(decoupeLotId, machineId);
        }
    }
}