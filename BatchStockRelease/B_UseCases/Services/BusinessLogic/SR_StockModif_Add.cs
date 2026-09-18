using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.AppLogic;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’enregistrer une ligne dans la table StockModif pour tracer la consommation d’un article interne.
    /// </summary>
    public class SR_StockModif_Add : IS_StockModif_Add
    {
        private readonly string _callee;
        private readonly IC_StockModif _chStockModif;

        public SR_StockModif_Add(IC_StockModif chStockModif)
        {
            _callee = GetType().Name;
            _chStockModif = chStockModif;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int idArticleInterne, 
            bool isSortie, 
            string? approConteneur, 
            DTO_StockQuantity result,
            DateTime appDateTime,
            int appUserId,
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (idArticleInterne <= 0)
                    throw new Ex_Business(callChain,"STK_06", "No_Er_Bu_23");

                if (isSortie)
                {
                    string changementDescription = $"Conso de : {result.QuantiteSortie} pcs dans la K7 : {approConteneur}, il reste {result.QuantiteApres} pcs";

                    var modif = new StockModif
                    {
                        Action = "Conso Picking",
                        Emplacement = approConteneur,
                        AncienneQuantite = result.QuantiteAvant,
                        NouvelleQuantite = result.QuantiteApres,
                        Changement = changementDescription,
                        IdUser = appUserId,
                        DateModification = appDateTime,
                        IdArticleInterne = idArticleInterne
                    };

                    await _chStockModif.HandleAddAsync(modif, callChain);
                }
                else
                {
                    string changementDescription = $"Retour de : {-result.QuantiteSortie} pcs dans la K7 : {approConteneur}, il a {result.QuantiteApres} pcs";

                    var modif = new StockModif
                    {
                        Action = "Retour Picking",
                        Emplacement = approConteneur,
                        AncienneQuantite = result.QuantiteAvant,
                        NouvelleQuantite = result.QuantiteApres,
                        Changement = changementDescription,
                        IdUser = appUserId,
                        DateModification = appDateTime,
                        IdArticleInterne = idArticleInterne
                    };

                    await _chStockModif.HandleAddAsync(modif, callChain);
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