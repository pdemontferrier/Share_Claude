using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour les informations relative à un lot.
    /// </summary>
    public class SR_DecoupeLot_SetInfo : IS_DecoupeLot_SetInfo
    {
        private readonly string _callee;
        private readonly IQ_DecoupeLot _qhDecoupeLot;
        private readonly IS_Settings_UseCase _settings;

        public SR_DecoupeLot_SetInfo(
            IQ_DecoupeLot qhDecoupeLot,
            IS_Settings_UseCase settings)
        {
            _callee = GetType().Name;
            _qhDecoupeLot = qhDecoupeLot;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                var decoupeLot = await _qhDecoupeLot.HandleGetByIdAsync(decoupeLotId);
                if (decoupeLot == null)
                    throw new Ex_Business(callChain,"DLI_01", "No_Er_Bu_30");

                _settings.SetDecoupeLotId(decoupeLotId);
                _settings.SetDecoupeLotDesignation(decoupeLot.Designation);
                _settings.SetDecoupeLotCouleur(decoupeLot.IdCouleur);
                _settings.SetDecoupeLotEcheance(decoupeLot.IdEcheance);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}