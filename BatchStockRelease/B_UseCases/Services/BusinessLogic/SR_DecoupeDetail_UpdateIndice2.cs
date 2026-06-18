using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour les indices de découpe (DecoupeBarreIndex) pour un lot donné.
    /// </summary>
    public class SR_DecoupeDetail_UpdateIndice2 : IS_DecoupeDetail_UpdateIndice2
    {
        private readonly string _callee;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IC_DecoupeDetail _chDecoupeDetail;

        public SR_DecoupeDetail_UpdateIndice2(
            IQ_DecoupeDetail qhDecoupeDetail,
            IC_DecoupeDetail chDecoupeDetail)
        {
            _callee = GetType().Name;
            _qhDecoupeDetail = qhDecoupeDetail;
            _chDecoupeDetail = chDecoupeDetail;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0)
                    throw new Ex_Business(callChain,"DCD_02", "No_Er_Bu_49");

                // Étape 1 : Récupérer les lignes avec IndiceDecoupe == 1
                var decoupesIndice1 = await _qhDecoupeDetail.HandleGetIndice1ByLotAsync(decoupeLotId);

                // Étape 2 : Récupérer les lignes avec IndiceDecoupe == 2
                var decoupesIndice2 = await _qhDecoupeDetail.HandleGetIndice2ByLotAsync(decoupeLotId);

                // Étape 3 : Parcourir les lignes avec IndiceDecoupe == 2 et les mettre à jour
                foreach (var decoupe2 in decoupesIndice2)
                {
                    // Récupérer les 13 premiers chiffres de NumLigne pour la ligne avec IndiceDecoupe == 2
                    var numLignePartie = decoupe2.NumLigne.ToString().Substring(0, 13);

                    // Trouver la ligne correspondante avec IndiceDecoupe == 1 et le même NumLignePartie
                    var correspondanceDecoupe1 = decoupesIndice1
                        .FirstOrDefault(d => d.NumLigne.ToString().StartsWith(numLignePartie));

                    if (correspondanceDecoupe1 != null)
                    {
                        decoupe2.IdDecoupeBarre = correspondanceDecoupe1.IdDecoupeBarre;
                        decoupe2.DecoupeBarreIndex = correspondanceDecoupe1.DecoupeBarreIndex;
                        decoupe2.DecoupeLongueurReste = correspondanceDecoupe1.DecoupeLongueurReste;
                        decoupe2.ApproOptimBarreNeuve = correspondanceDecoupe1.ApproOptimBarreNeuve;
                    }

                    // Mettre à jour la table DecoupeDetail
                    await _chDecoupeDetail.HandleUpdateAsync(decoupe2, callChain);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}
