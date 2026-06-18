using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’enregistrer une ligne de consommation dans la table ArticleInterneConsommation.
    /// </summary>
    public class SR_AIConsommation_Add : IS_AIConsommation_Add
    {
        private readonly string _callee;
        private readonly IC_ArticleInterneConsommation _chAIConsommation;

        public SR_AIConsommation_Add(IC_ArticleInterneConsommation chAIConsommation)
        {
            _callee = GetType().Name;
            _chAIConsommation = chAIConsommation;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId, 
            int idArticleInterne, 
            bool isSortie, 
            string? approConteneur, 
            string? approAdresseDesignation, 
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
                    throw new Ex_Business(callChain, "AIC_01", "No_Er_Bu_23");

                if (isSortie && result.QuantiteAvant <= 0)
                    throw new Ex_Business(callChain, "AIC_02", "No_Er_Bu_24");

                if (isSortie)
                {
                    var consommation = new ArticleInterneConsommation
                    {
                        IdArticleInterne = idArticleInterne,
                        Quantite = result.QuantiteSortie,
                        DateConso = appDateTime,
                        IdUtilisateur = appUserId,
                        Motif = string.Empty,
                        IdMotif = 27, // Code standard pour "Consommation lot découpe"
                        NumConsoManuelle = "Lot : " + decoupeLotId.ToString("D4"),
                        NumCommande = "000000000", // valeur par défaut
                        Conteneur = approConteneur,
                        Adresse = approAdresseDesignation
                    };

                    await _chAIConsommation.HandleAddAsync(consommation, callChain);
                }
                else
                {
                    var consommation = new ArticleInterneConsommation
                    {
                        IdArticleInterne = idArticleInterne,
                        Quantite = result.QuantiteSortie,
                        DateConso = appDateTime,
                        IdUtilisateur = appUserId,
                        Motif = "Retour en stock",
                        IdMotif = 28,
                        NumConsoManuelle = "Retour",
                        NumCommande = "000000000", // valeur par défaut
                        Conteneur = approConteneur,
                        Adresse = approAdresseDesignation
                    };

                    await _chAIConsommation.HandleAddAsync(consommation, callChain);
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