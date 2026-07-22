using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Implémentation du service d’ajout d’une nouvelle barre (neuve ou de chute) à partir d’une découpe.
    /// </summary>
    public class SR_DecoupeBarre_AddNewBarre : IS_DecoupeBarre_AddNewBarre
    {
        private readonly IC_DecoupeBarre _chDecoupeBarre;
        private readonly IQ_DecoupeBarre _qhDecoupeBarre;
        private readonly IS_Settings_UseCase _settings;

        private readonly string _callee;

        public SR_DecoupeBarre_AddNewBarre(
            IC_DecoupeBarre chDecoupeBarre,
            IQ_DecoupeBarre qhDecoupeBarre,
            IS_Settings_UseCase settings)
        {
            _chDecoupeBarre = chDecoupeBarre;
            _qhDecoupeBarre = qhDecoupeBarre;
            _settings = settings;
            _callee = GetType().Name;
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(
            int decoupeLotId, 
            DecoupeDetail decoupeEnTraitement, 
            bool isChute, 
            VieChuteMagasinReference? chute , 
            bool isVirtualBar, 
            string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (decoupeEnTraitement == null)
                    throw new Ex_Business(callChain, "OPT_04", "No_Er_Bu_46");

                if (decoupeEnTraitement.LongueurBarre is null or <= 0)
                    throw new Ex_Business(callChain, "OPT_05", "No_Er_Bu_47");

                // Initialiser le BarIndex à 1
                _settings.SetDecoupeBarreNewBarIndex(1);

                // Calculer la longueur restante
                if (isChute)
                {
                    if (chute == null)
                        throw new Ex_Business(callChain, "OPT_07", "No_Er_Bu_57");

                    _settings.SetDecoupeBarreRemainingBarLength((chute.LongueurBarre ?? 0) - (decoupeEnTraitement.LongueurOptim ?? 0) - _settings.DecoupeBarreGetSpaceBetweenCuts());
                }
                else
                {
                    _settings.SetDecoupeBarreRemainingBarLength((decoupeEnTraitement.LongueurBarre ?? 0) - (decoupeEnTraitement.LongueurOptim ?? 0) - _settings.DecoupeBarreGetSpaceBetweenCuts());
                }

                // Récupérer les informations relative au chariot de destination
                var (chariotId, chariotName) = await _qhDecoupeBarre.HandleGetChariotInfoAsync(decoupeLotId);
                if (chariotId <= 0)
                    throw new Ex_Business(callChain, "OPT_06", "No_Er_Bu_48");

                // Ajouter une nouvelle ligne à DecoupeBarre
                var newBarre = new DecoupeBarre
                {
                    IdDecoupeLot = decoupeEnTraitement.IdDecoupeLot,
                    IdArticleInterne = decoupeEnTraitement.IdArticleInterne,
                    LongueurBarre = decoupeEnTraitement.LongueurBarre,
                    LongueurChuteMini = decoupeEnTraitement.LongueurChuteMini,
                    Categorie1 = decoupeEnTraitement.Categorie1,
                    Categorie2 = decoupeEnTraitement.Categorie2,
                    Categorie3 = decoupeEnTraitement.Categorie3,
                    Categorie4 = decoupeEnTraitement.Categorie4,
                    OrdreTri = decoupeEnTraitement.OrdreTri ?? 0,
                    ApproOrigine = isChute ? "chute" : (isVirtualBar ? "virtuel" : "neuf"),
                    ApproIdChariot = chariotId,
                    ApproChariotDesignation = chariotName,
                    ApproSortieFaite = isVirtualBar ? true : false,
                    ApproSortieSupp = false,
                    ApproInactif = isVirtualBar,
                    DecoupeLongueurReste = _settings.GetDecoupeBarreRemainingBarLength(),
                    DecoupeNombre = _settings.GetDecoupeBarreNewBarIndex()
                };

                // Mettre à jour le nouvel enregistrement si isChute
                if (isChute)
                {
                    newBarre.IdStock = chute.IdChuteMagasin;
                    newBarre.LongueurBarre = chute.LongueurBarre;
                    newBarre.ApproCodeBarre = chute.CodeBarre;
                    newBarre.ApproAllocation = true;
                    newBarre.ApproRupture = false;
                    newBarre.ApproEmplacement = chute.Emplacement ?? 0;
                    newBarre.ApproEmplacementDesignation = chute.EmplacementDesignation;
                }

                // Mettre à jour le nouvel enregistrement DecoupeBarre
                await _chDecoupeBarre.HandleAddAsync(newBarre, callChain);

                // Garder en mémoire l'ID de la barre en cours de calcul
                _settings.SetDecoupeBarreNewBarId(newBarre.Id);
            }
            catch (Ex_Business) { throw; }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }
    }
}