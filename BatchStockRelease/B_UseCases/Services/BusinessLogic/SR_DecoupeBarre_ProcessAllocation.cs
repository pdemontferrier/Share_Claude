using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’allouer des barres neuves pour un article donné dans un lot.
    /// </summary>
    public class SR_DecoupeBarre_ProcessAllocation : IS_DecoupeBarre_ProcessAllocation
    {
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_VieStockQuantiteEmplacement _qhStock;
        private readonly string _callee;

        public SR_DecoupeBarre_ProcessAllocation(
            IQ_DecoupeBarre qhDecoupeBarre,
            IC_DecoupeBarre chDecoupeBarre,
            IQ_VieStockQuantiteEmplacement qhStock)
        {
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeBarre = chDecoupeBarre;
            _qhStock = qhStock;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, int articleInterneId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeLotId <= 0 || articleInterneId <= 0)
                    throw new Ex_Business(callChain,"DBU_11", "No_Er_Bu_40");

                // Récupérer les barres non allouées pour cet IdArticleInterne
                var barNotAllocated = await _qhDecoupeBarre.HandleGetAllForArticleInterneIdAsync(decoupeLotId, articleInterneId);

                // Récupèrer les enregistrements de VieStockQuantiteEmplacement (Stock disponible)
                var stockByArticle = await _qhStock.HandleGetByArticleInterneIdAsync(articleInterneId);

                // Initialiser un compteur pour l'index de stock
                int stockIndex = 0;
                int stockQuantityRemaining = stockByArticle.Any() ? (int)stockByArticle[stockIndex].Quantite : 0;

                // Gérer le cas d'une rupture de stock totale
                if (stockQuantityRemaining == 0)
                {
                    foreach (var decoupeBarre in barNotAllocated)
                    {
                        decoupeBarre.ApproRupture = true;
                        await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
                    }
                }
                else
                {
                    // Tant qu'il reste des découpes à optimiser
                    while (barNotAllocated.Any() && stockIndex < stockByArticle.Count)
                    {
                        foreach (var decoupeBarre in barNotAllocated.ToList()) // copie car on modifie potentiellement la collection
                        {
                            if (stockQuantityRemaining > 0)
                            {
                                var emplacement = stockByArticle[stockIndex];

                                // Assigner les informations d'emplacement au DecoupeBarre
                                decoupeBarre.IdStock = emplacement.Id;
                                decoupeBarre.ApproZonePriorite = emplacement.ZonePriorite;
                                decoupeBarre.ApproZoneDesignation = emplacement.ZoneDesignation;
                                decoupeBarre.ApproAdressePriorite = emplacement.AdressePriorite;
                                decoupeBarre.ApproAdresseDesignation = emplacement.AdresseDesignation;
                                decoupeBarre.ApproConteneur = emplacement.ConteneurDesignation;
                                decoupeBarre.ApproTypeConteneur = emplacement.TypeConteneur;
                                decoupeBarre.ApproTypeContenant = emplacement.TypeContenant;
                                decoupeBarre.ApproAllocation = true;
                                decoupeBarre.ApproRupture = false;

                                // Décrémenter la quantité restante
                                stockQuantityRemaining--;

                                await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);

                                if (stockQuantityRemaining <= 0)
                                {
                                    stockIndex++;
                                    if (stockIndex < stockByArticle.Count)
                                    {
                                        stockQuantityRemaining = (int)stockByArticle[stockIndex].Quantite;
                                    }
                                }
                            }
                            else
                            {
                                decoupeBarre.ApproRupture = true;
                                await _chDecoupeBarre.HandleUpdateAsync(decoupeBarre, callChain);
                            }
                        }

                        // Mettre à jour la liste des barres à allouer
                        barNotAllocated = await _qhDecoupeBarre.HandleGetAllForArticleInterneIdAsync(decoupeLotId, articleInterneId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}