using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’extraire les machines de découpe à approvisionner pour un lot.
    /// </summary>
    public class SR_DecoupeDetail_MachineList : IS_DecoupeDetail_MachineList
    {
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly string _callee;

        public SR_DecoupeDetail_MachineList(IQ_DecoupeDetail qhDecoupeDetail)
        {
            _qhDecoupeDetail = qhDecoupeDetail;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task<List<string>> ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0)
                    throw new Ex_Business(callChain,"DDM_01", "No_Er_Bu_43");

                var machineIds = await _qhDecoupeDetail.HandleGetCuttingMachineListToBeSuppliedAsync(decoupeLotId);

                return machineIds;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}