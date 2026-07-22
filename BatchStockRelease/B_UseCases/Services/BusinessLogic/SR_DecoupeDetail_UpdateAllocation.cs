using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Implémentation du service de mise à jour de la table DecoupeDetail apprès allocation d'une barre.
    /// </summary>
    public class SR_DecoupeDetail_UpdateAllocation : IS_DecoupeDetail_UpdateAllocation
    {
        private readonly IC_DecoupeDetail _chDecoupeDetail;
        private readonly IS_Settings_UseCase _settings;

        private readonly string _callee;

        public SR_DecoupeDetail_UpdateAllocation(
            IC_DecoupeDetail chDecoupeDetail,
            IS_Settings_UseCase settings)
        {
            _chDecoupeDetail = chDecoupeDetail;
            _settings = settings;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(DecoupeDetail decoupeEnTraitement, bool isChute, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeEnTraitement == null)
                    throw new Ex_Business(callChain, "OPT_04", "No_Er_Bu_46");

                if (decoupeEnTraitement.LongueurBarre is null or <= 0)
                    throw new Ex_Business(callChain, "OPT_05", "No_Er_Bu_47");

                // Mise à jour de decoupeEnTraitement
                if (isChute)
                {
                    decoupeEnTraitement.ApproOptimBarreChute = true;
                }
                else 
                {
                    decoupeEnTraitement.ApproOptimBarreNeuve = true;
                }
                decoupeEnTraitement.IdDecoupeBarre = _settings.GetDecoupeBarreNewBarId();
                decoupeEnTraitement.DecoupeBarreIndex = _settings.GetDecoupeBarreNewBarIndex();
                decoupeEnTraitement.DecoupeLongueurReste = _settings.GetDecoupeBarreRemainingBarLength();

                // Mise à jour de DecoupeDetail
                await _chDecoupeDetail.HandleUpdateAsync(decoupeEnTraitement, callChain);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}