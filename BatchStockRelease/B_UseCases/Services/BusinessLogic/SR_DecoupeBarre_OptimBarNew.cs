using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de calculer et créer les barres neuves nécessaires à un lot donné.
    /// </summary>
    public class SR_DecoupeBarre_OptimBarNew : IS_DecoupeBarre_OptimBarNew
    {
        private readonly string _callee;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_DecoupeBarre_AddNewBarre _addNewBarre;
        private readonly IS_DecoupeBarre_FinalizeBarre _finalizeBarre;
        private readonly IS_DecoupeDetail_UpdateAllocation _updateAllocation;
        private readonly IS_Settings_UseCase _settings;

        public SR_DecoupeBarre_OptimBarNew(
            IQ_DecoupeDetail qhDecoupeDetail,
            IQ_DecoupeBarre qhDecoupeBarre,
            IS_DecoupeBarre_AddNewBarre addNewBarre,
            IS_DecoupeBarre_FinalizeBarre finalizeBarre,
            IS_DecoupeDetail_UpdateAllocation updateAllocation,
            IS_Settings_UseCase settings)
        {
            _callee = GetType().Name;
            _qhDecoupeDetail = qhDecoupeDetail;
            _qhDecoupeBarre = qhDecoupeBarre;
            _addNewBarre = addNewBarre;
            _finalizeBarre = finalizeBarre;
            _updateAllocation = updateAllocation;
            _settings = settings;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId, 
            string decoupeMachineId, 
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                // Étape 1 : listes d’articles à traiter
                var idsNeufs = await _qhDecoupeDetail.HandleGetArticleInterneIdListToBeSuppliedAsync(decoupeLotId, decoupeMachineId);
                var idsVirtuels = await _qhDecoupeDetail.HandleGetArticleComposeIdListToBeAddedAsync(decoupeLotId, decoupeMachineId);

                // Si rien à faire du tout
                if ((idsNeufs == null || !idsNeufs.Any()) && (idsVirtuels == null || !idsVirtuels.Any()))
                    throw new Ex_Business(callChain, "OPT_02", "No_Er_Bu_44");

                // Étape 2 : traiter les barres neuves (réelles)
                if (idsNeufs != null && idsNeufs.Any())
                {
                    foreach (var articleInterneId in idsNeufs)
                        await ProcessOptim(decoupeLotId, decoupeMachineId, articleInterneId, false, callChain);
                }

                // Étape 3 : traiter les barres virtuelles (composés non préparés)
                if (idsVirtuels != null && idsVirtuels.Any())
                {
                    foreach (var articleInterneId in idsVirtuels)
                        await ProcessOptim(decoupeLotId, decoupeMachineId, articleInterneId, true, callChain);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        private async Task ProcessOptim(
            int decoupeLotId,
            string decoupeMachineId,
            int articleInterneId,
            bool isVirtual,
            string caller)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ProcessOptim)}";

            try
            {
                // Étape 1 : Sélection des découpes en fonction du type
                var decoupesNonOptimisees = isVirtual
                    ? await _qhDecoupeDetail.HandleGetArticleComposeToBeAddedAsync(decoupeLotId, decoupeMachineId, articleInterneId)
                    : await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);

                // Vérification métier : aucune découpe disponible
                if (decoupesNonOptimisees == null || !decoupesNonOptimisees.Any())
                    throw new Ex_Business(callChain, isVirtual ? "OPT_04" : "OPT_03", "No_Er_Bu_45");

                // Étape 2 : Boucle d’optimisation tant qu'il reste des découpes à optimiser
                while (decoupesNonOptimisees.Any())
                {
                    // Ajouter une nouvelle barre (flag isApproInactif selon isVirtual)
                    var decoupeEnTraitement = decoupesNonOptimisees.First();
                    await _addNewBarre.ExecuteAsync(decoupeLotId, decoupeEnTraitement, false, null, isVirtual, callChain);

                    // Mise à jour de l'allocation dans DécoupeDetail
                    await _updateAllocation.ExecuteAsync(decoupeEnTraitement, false, callChain);

                    // Parcourir les découpes restantes
                    foreach (var autreDecoupe in decoupesNonOptimisees.Skip(1))
                    {
                        // Vérifier si la découpe peut être réalisée avec la barre actuelle
                        if ((autreDecoupe.LongueurOptim ?? 0) + _settings.DecoupeBarreGetSpaceBetweenCuts() <= _settings.GetDecoupeBarreRemainingBarLength())
                        {
                            // Incrémenter le compteur du nombre de découpes
                            _settings.SetDecoupeBarreNewBarIndex(_settings.GetDecoupeBarreNewBarIndex() + 1);

                            // Mise à jour de la longueur restante
                            _settings.SetDecoupeBarreRemainingBarLength(
                                _settings.GetDecoupeBarreRemainingBarLength() -
                                (autreDecoupe.LongueurOptim ?? 0) -
                                _settings.DecoupeBarreGetSpaceBetweenCuts()
                            );

                            // Mise à jour de l'allocation dans DécoupeDetail
                            await _updateAllocation.ExecuteAsync(autreDecoupe, false, callChain);
                        }
                    }

                    // Finaliser la barre après avoir traité toutes les découpes possible
                    var currentBarre = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetDecoupeBarreNewBarId());
                    if (currentBarre != null)
                        await _finalizeBarre.ExecuteAsync(currentBarre, callChain);

                    // Recharger la liste des découpes en fonction du type
                    decoupesNonOptimisees = isVirtual
                        ? await _qhDecoupeDetail.HandleGetArticleComposeToBeAddedAsync(decoupeLotId, decoupeMachineId, articleInterneId)
                        : await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
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