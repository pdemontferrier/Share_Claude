using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’optimiser l’utilisation des barres de chute pour un lot donné.
    /// </summary>
    public class SR_DecoupeBarre_OptimBarDrop : IS_DecoupeBarre_OptimBarDrop
    {
        private readonly string _callee;
        private readonly IQ_VieChuteMagasinReference _qhVieChuteMagasinReference;
        private readonly IC_ChutesMagasin _chChutesMagasin;
        private readonly IQ_ChutesMagasin _qhChutesMagasin;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IS_DecoupeBarre_AddNewBarre _addNewBarre;
        private readonly IS_DecoupeBarre_FinalizeBarre _finalizeBarre;
        private readonly IS_DecoupeDetail_UpdateAllocation _updateAllocation;
        private readonly IS_Settings_UseCase _settings;

        public SR_DecoupeBarre_OptimBarDrop(
            IQ_VieChuteMagasinReference qhVieChuteMagasinReference,
            IC_ChutesMagasin chChutesMagasin,
            IQ_ChutesMagasin qhChutesMagasin,
            IQ_DecoupeBarre qhDecoupeBarre,
            IQ_DecoupeDetail qhDecoupeDetail,
            IS_DecoupeBarre_AddNewBarre addNewBarre,
            IS_DecoupeBarre_FinalizeBarre finalizeBarre,
            IS_DecoupeDetail_UpdateAllocation updateAllocation,
            IS_Settings_UseCase settings)
        {
            _callee = GetType().Name;
            _qhVieChuteMagasinReference = qhVieChuteMagasinReference;
            _chChutesMagasin = chChutesMagasin;
            _qhChutesMagasin = qhChutesMagasin;
            _qhDecoupeBarre = qhDecoupeBarre;
            _qhDecoupeDetail = qhDecoupeDetail;
            _addNewBarre = addNewBarre;
            _finalizeBarre = finalizeBarre;
            _updateAllocation = updateAllocation;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(int decoupeLotId, string decoupeMachineId, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : Récupérer les articles internes à traiter pour la machine donnée
                var idArticleInterneList = await _qhDecoupeDetail.HandleGetArticleInterneIdListToBeSuppliedAsync(decoupeLotId, decoupeMachineId);

                if (idArticleInterneList == null || !idArticleInterneList.Any())
                    throw new Ex_Business(callChain,"OPT_12", "No_Er_Bu_61");

                // Étape 2 : Parcourir chaque article à traiter
                foreach (var articleInterneId in idArticleInterneList)
                {
                    // Récupérer les barres de chutes disponibles pour l'articleInterneId et trier par LongueurBarre (ordre croissant)
                    var chutesDisponible = await _qhVieChuteMagasinReference.HandleGetByArticleInterneIdAsync(articleInterneId);

                    if (chutesDisponible == null || !chutesDisponible.Any())
                        throw new Ex_Business(callChain,"OPT_13", "No_Er_Bu_62");

                    // Appeler le service de traitement du groupe article/chute
                    await ProcessArticleInterneOptim(decoupeLotId, decoupeMachineId, articleInterneId, chutesDisponible, callChain);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        private async Task ProcessArticleInterneOptim(int decoupeLotId, string decoupeMachineId, int articleInterneId, List<VieChuteMagasinReference> chutesDisponible, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ProcessArticleInterneOptim)}";

            try
            {
                // Étape 1 : Récupérer les découpes non optimisées
                var decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);

                // Vérification métier : aucune découpe disponible
                if (decoupesNonOptimisees == null || !decoupesNonOptimisees.Any())
                    throw new Ex_Business(callChain,"OPT_03", "No_Er_Bu_45");

                // Étape 2 : Boucle d’optimisation tant qu'il reste des découpes à optimiser
                while (decoupesNonOptimisees.Any())
                {
                    foreach (var decoupeEnTraitement in decoupesNonOptimisees)
                    {
                        // Initialiser chuteUtilisee
                        var chuteUtilisee = false;

                        foreach (var chute in chutesDisponible)
                        {
                            if (chute.LongueurBarre.HasValue && decoupeEnTraitement.LongueurOptim.HasValue &&
                                chute.LongueurBarre > decoupeEnTraitement.LongueurOptim + _settings.DecoupeBarreGetSpaceBetweenCuts())
                            {
                                // Mettre à jour chuteUtilisee
                                chuteUtilisee = true;

                                // Ajouter une nouvelle barre de chute
                                await _addNewBarre.ExecuteAsync(decoupeLotId, decoupeEnTraitement, true, chute, false, callChain);

                                // Mise à jour de l'allocation dans DécoupeDetail
                                await _updateAllocation.ExecuteAsync(decoupeEnTraitement, true, callChain);

                                // Mettre à jour la liste des découpes non optimisées
                                decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);

                                // Metre à jour la table pour le champs Reserve
                                var chuteMagasin = await _qhChutesMagasin.HandleGetByIdAsync(chute.IdChuteMagasin);

                                if (chuteMagasin != null)
                                {
                                    chuteMagasin.Reserve = decoupeLotId.ToString();
                                    await _chChutesMagasin.HandleUpdateAsync(chuteMagasin, callChain);
                                }

                                // Utiliser le reste pour tenter une autre découpe
                                foreach (var autreDecoupe in decoupesNonOptimisees)
                                {
                                    // Vérifier si la découpe peut être réalisée avec la barre actuelle
                                    if ((autreDecoupe.LongueurOptim ?? 0) + _settings.DecoupeBarreGetSpaceBetweenCuts() <= _settings.GetDecoupeBarreRemainingBarLength())
                                    {
                                        // Incrémenter le compteur du nombre de découpes
                                        _settings.SetDecoupeBarreNewBarIndex(_settings.GetDecoupeBarreNewBarIndex() + 1);

                                        // Mise à jour de la longueur restante
                                        _settings.SetDecoupeBarreRemainingBarLength(_settings.GetDecoupeBarreRemainingBarLength() - (autreDecoupe.LongueurOptim ?? 0) - _settings.DecoupeBarreGetSpaceBetweenCuts());

                                        // Mise à jour de l'allocation dans DécoupeDetail
                                        await _updateAllocation.ExecuteAsync(autreDecoupe, true, callChain);

                                        // Mettre à jour la liste des découpes non optimisées
                                        decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
                                    }
                                }

                                // Finaliser la barre après avoir traité toutes les découpes possible
                                var currentBarre = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreNewBarId());

                                if (currentBarre != null && chuteUtilisee)
                                    await _finalizeBarre.ExecuteAsync(currentBarre, callChain);

                                // On sort après utilisation d'une seule chute pour cette découpe
                                break;
                            }
                        }

                        if (!chuteUtilisee) continue; // Aucune chute disponible n'a pu être utilisée pour cette découpe
                    }

                    // Mettre à jour la liste des découpes non optimisées après ce cycle
                    decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
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