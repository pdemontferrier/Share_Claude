using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Queries;
using BatchCutting_DG.A_Domain.Interfaces.Handlers.Commands;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchCutting_DG.B_UseCases.Services.BusinessLogic
{
    public class SR_BarNewOptim : IS_BarNewOptim
    {
        private readonly string ServiceName;
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IC_DecoupeDetail _chDecoupeDetail;
        private readonly IQ_DecoupeDetail _qhDecoupeDetail;
        private readonly IS_Settings _settings;

        public SR_BarNewOptim(  IC_DecoupeBarre chDecoupeBarre, IQ_DecoupeBarre qhDecoupeBarre,
                                    IC_DecoupeDetail chDecoupeDetail, IQ_DecoupeDetail qhDecoupeDetail,
                                    IS_Settings settings)
        {
            ServiceName = GetType().Name;
            _chDecoupeBarre = chDecoupeBarre;
            _qhDecoupeBarre = qhDecoupeBarre;
            _chDecoupeDetail = chDecoupeDetail;
            _qhDecoupeDetail = qhDecoupeDetail;
            _settings = settings;
        }

        public async Task ExecuteAsync(int decoupeLotId)
        {
            // Étape 1 : Récupérer la liste des IdDecoupeMacine à traiter
            var idDecoupeMachineList = await _qhDecoupeDetail.HandleGetCuttingMachineListToBeSuppliedAsync(decoupeLotId);

            // Tester si la liste est vide et si oui sortir, sinon continuer
            if (idDecoupeMachineList == null || !idDecoupeMachineList.Any())
            {
                return;
            }
            else
            {
                // Étape 2 : Parcourir la liste des IdDecoupeMachine
                foreach (var decoupeMachineId in idDecoupeMachineList)
                {
                    if (decoupeMachineId != null)
                    {
                        // Étape 2.1 : Récupérer la liste des IdArticleInterne à traiter
                        var idArticleInterneList = await _qhDecoupeDetail.HandleGetArticleInterneIdListToBeSuppliedAsync(decoupeLotId, decoupeMachineId);

                        // Tester si la liste est vide et si oui sortir, sinon continuer
                        if (idArticleInterneList == null || !idArticleInterneList.Any())
                        {
                            return;
                        }
                        else
                        {
                            // Étape 2.2 : Parcourir la liste des IdArticleInterne
                            foreach (var articleInterneId in idArticleInterneList)
                            {
                                // Appeler ProcessArticleGroup avec seulement l'IdArticleInterne à traiter
                                await ProcessArticleGroup(decoupeLotId, decoupeMachineId, articleInterneId);
                            }
                        }
                    }
                }

                // Étape 3 : Mettre à jour les lignes avec IndiceDecoupe == 2
                await UpdateDecoupeDetailsForIndice2(decoupeLotId);
            }
        }

        private async Task ProcessArticleGroup(int decoupeLotId, string decoupeMachineId, int articleInterneId)
        {
            // Récupérer les découpes non optimisées pour cet IdArticleInterne
            var decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);

            // Tester si la liste est vide et si oui sortir, sinon continuer
            if (decoupesNonOptimisees == null || !decoupesNonOptimisees.Any())
            {
                return;
            }
            else
            {
                // Tant qu'il reste des découpes à optimiser
                while (decoupesNonOptimisees.Any())
                {
                    // Ajouter une nouvelle barre avec la première découpe
                    await AddNewBarre(decoupesNonOptimisees.First());

                    // Parcourir les découpes restantes
                    foreach (var decoupe in decoupesNonOptimisees.Skip(1))
                    {
                        // Vérifier si la découpe peut être réalisée avec la barre actuelle
                        if (((decoupe.LongueurOptim ?? 0) + _settings.GetSpaceBetweenCuts()) <= _settings.GetRemainingBarLength())
                        {
                            // Remplir la barre avec la découpe actuelle
                            await TryFillBarWithDecoupe(decoupe);
                        }
                    }

                    // Finaliser la barre après avoir traité toutes les découpes possible
                    var currentBarre = await _qhDecoupeBarre.HandleGetByIdAsync(_settings.GetNewBarId());

                    if (currentBarre != null)
                    {
                        await FinalizeBarre(currentBarre);
                    }

                    // Mettre à jour la liste des découpes non optimisées après ce cycle
                    decoupesNonOptimisees = await _qhDecoupeDetail.HandleGetToBeSuppliedAsync(decoupeLotId, decoupeMachineId, articleInterneId);
                }
            }
        }

        private async Task AddNewBarre(DecoupeDetail firstDecoupe)
        {
            // Mise à jour de DecoupeBarre
            _settings.SetNewBarIndex(1);

            // Initialiser la longueur restante avec la longueur de la nouvelle barre
            _settings.SetRemainingBarLength(firstDecoupe.LongueurBarre ?? 0);

            // Récupérer les informations relative au chariot de destination
            var (chariotId, chariotName) = await _qhDecoupeBarre.HandleGetChariotInfoForLotAsync(_settings.GetDecoupeLotId());

            // Ajouter une nouvelle ligne à DecoupeBarre
            var newBarre = new DecoupeBarre
            {
                IdDecoupeLot = firstDecoupe.IdDecoupeLot,
                IdArticleInterne = firstDecoupe.IdArticleInterne,
                LongueurBarre = firstDecoupe.LongueurBarre,
                LongueurChuteMini = firstDecoupe.LongueurChuteMini,
                Categorie1 = firstDecoupe.Categorie1,
                Categorie2 = firstDecoupe.Categorie2,
                Categorie3 = firstDecoupe.Categorie3,
                Categorie4 = firstDecoupe.Categorie4,
                OrdreTri = firstDecoupe.OrdreTri ?? 0,
                ApproOrigine = "neuf",
                ApproIdChariot = chariotId,
                ApproChariotDesignation = chariotName,
                ApproSortieSupp = true,
                DecoupeLongueurReste = _settings.GetRemainingBarLength(),
                DecoupeNombre = _settings.GetNewBarIndex(),
            };
            await _chDecoupeBarre.HandleAddAsync(newBarre, ServiceName, nameof(AddNewBarre));

            // Garder en mémoire l'ID de la barre en cours de calcul
            _settings.SetNewBarId(newBarre.Id);

            // Calculer la longueur restante
            _settings.SetRemainingBarLength(_settings.GetRemainingBarLength() - (firstDecoupe.LongueurOptim ?? 0) - _settings.GetSpaceBetweenCuts());

            // Mise à jour de DecoupeDetail
            firstDecoupe.ApproOptimBarreNeuve = true;
            firstDecoupe.IdDecoupeBarre = _settings.GetNewBarId();
            firstDecoupe.DecoupeBarreIndex = _settings.GetNewBarIndex();
            firstDecoupe.DecoupeLongueurReste = _settings.GetRemainingBarLength();

            await _chDecoupeDetail.HandleUpdateAsync(firstDecoupe, ServiceName, nameof(AddNewBarre));
        }

        private async Task TryFillBarWithDecoupe(DecoupeDetail decoupe)
        {
            if (((decoupe.LongueurOptim ?? 0) + _settings.GetSpaceBetweenCuts()) <= _settings.GetRemainingBarLength())
            {
                // Incrémenter le compteur du nombre de découpes
                var newIndex = _settings.GetNewBarIndex();
                _settings.SetNewBarIndex(newIndex + 1);

                // Mise à jour de la longueur restante
                _settings.SetRemainingBarLength(_settings.GetRemainingBarLength() - (decoupe.LongueurOptim ?? 0) - _settings.GetSpaceBetweenCuts());

                // Mise à jour avec la barre existante
                decoupe.ApproOptimBarreNeuve = true;
                decoupe.IdDecoupeBarre = _settings.GetNewBarId();
                decoupe.DecoupeBarreIndex = _settings.GetNewBarIndex();
                decoupe.DecoupeLongueurReste = _settings.GetRemainingBarLength();

                await _chDecoupeDetail.HandleUpdateAsync(decoupe, ServiceName, nameof(TryFillBarWithDecoupe));
            }
        }

        private async Task FinalizeBarre(DecoupeBarre barre)
        {
            barre.DecoupeNombre = _settings.GetNewBarIndex();
            barre.DecoupeLongueurReste = _settings.GetRemainingBarLength();
            barre.DecoupeTypeReste = _settings.GetRemainingBarLength() <= barre.LongueurChuteMini ? "dechet" : "chute";

            await _chDecoupeBarre.HandleUpdateAsync(barre, ServiceName, nameof(FinalizeBarre));
        }

        private async Task UpdateDecoupeDetailsForIndice2(int decoupeLotId)
        {
            // Étape 1 : Récupérer les lignes avec IndiceDecoupe == 1
            var decoupesIndice1 = await _qhDecoupeDetail.HandleGetIndice1ByLotAsyncAsync(decoupeLotId);

            // Étape 2 : Récupérer les lignes avec IndiceDecoupe == 2
            var decoupesIndice2 = await _qhDecoupeDetail.HandleGetIndice2ByLotAsyncAsync(decoupeLotId);

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

                // Enregistrer les changements dans la base de données
                await _chDecoupeDetail.HandleUpdateAsync(decoupe2, ServiceName, nameof(UpdateDecoupeDetailsForIndice2));
            }
        }
    }
}