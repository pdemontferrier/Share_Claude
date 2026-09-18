using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de finaliser les informations d’une barre (quantité, chute, type) après optimisation.
    /// </summary>
    public class SR_DecoupeBarre_FinalizeBarre : IS_DecoupeBarre_FinalizeBarre
    {
        private readonly string _callee;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IS_Settings_UseCase _settings;

        public SR_DecoupeBarre_FinalizeBarre(IC_DecoupeBarre chDecoupeBarre, IS_Settings_UseCase settings)
        {
            _callee = GetType().Name;
            _chDecoupeBarre = chDecoupeBarre;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(DecoupeBarre decoupeBarre, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                decoupeBarre.DecoupeNombre = _settings.GetDecoupeBarreNewBarIndex();
                decoupeBarre.DecoupeLongueurReste = _settings.GetDecoupeBarreRemainingBarLength();
                decoupeBarre.DecoupeTypeReste = decoupeBarre.DecoupeLongueurReste <= decoupeBarre.LongueurChuteMini ? "dechet" : "chute";

                await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}