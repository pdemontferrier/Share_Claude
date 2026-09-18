using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.Entities.GestStock;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.C_Infrastructure.Repositories.Generic;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeDetail : CR_Generic<DecoupeDetail>, IR_DecoupeDetail
    {
        public CR_DecoupeDetail(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Récupérer la première découpe prête à être réalisée, associée à une barre déjà sortie du stock
        public async Task<DecoupeDetail?> GetFirstToCutAsync(int decoupeLotId, string machineId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await (from dd in context.DecoupeDetails
                          join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                          where dd.IdDecoupeLot == decoupeLotId
                                && dd.Inactif == false
                                && dd.ValidationLigne == true
                                && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true)
                                && dd.IdDecoupeBarre != 0
                                && dd.DecoupeBarreIndex != 0
                                && dd.DecoupeFaite == false
                                && dd.Categorie2 == 1
                                && dd.Categorie4 == machineId
                                && db.ApproSortieFaite == true
                          orderby dd.Structure,
                                  dd.OrdreTri,
                                  dd.Designation,
                                  dd.Reference,
                                  dd.Couleur,
                                  dd.IdArticleInterne,
                                  dd.IdDecoupeBarre,
                                  dd.DecoupeBarreIndex,
                                  dd.NumLigne,
                                  dd.IndiceDecoupe
                          select dd)
                         .AsNoTracking()
                         .FirstOrDefaultAsync();
        }

        // Requête spécifique : Retourne la liste des découpes pour une barre donnée par son Id
        public async Task<List<DecoupeDetail>> GetAllForDecoupeBarreIdAsync(int decoupeBarreId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeBarre == decoupeBarreId)
                .OrderBy(dd => dd.DecoupeBarreIndex)
                .ToListAsync();
        }

        // Requête spécifique : Retourne un bool si une barre (decoupeBarreId) à une ou des découpes de réalisées
        public async Task<bool> ExistsByDecoupeBarreIdAsync(int decoupeBarreId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails.AnyAsync(dd => dd.IdDecoupeBarre == decoupeBarreId && dd.DecoupeFaite == true);
        }

        // Requête spécifique : Retourne les découpes à approvisionner pour un lot, une machine, et un article donné
        public async Task<List<DecoupeDetail>> GetToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.Categorie4 == decoupeMachineId
                        && dd.IdArticleInterne == articleInterneId
                        && dd.IndiceDecoupe == 1
                        && dd.ApproOptimBarreChute == false
                        && dd.ApproOptimBarreNeuve == false
                        && dd.ValidationLigne == true
                        && ((dd.Inactif == false && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
                .OrderByDescending(detail => detail.LongueurOptim)
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des machines de découpe ayant des découpe à approvisionner pour un lot donné
        public async Task<List<string>> GetCuttingMachineListToBeSuppliedAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.IndiceDecoupe == 1
                        && dd.ApproOptimBarreChute == false
                        && dd.ApproOptimBarreNeuve == false
                        && dd.Categorie4.StartsWith("DG")
                        && dd.ValidationLigne == true
                        && ((dd.Inactif == false && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
                .Select(dd => dd.Categorie4!)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des articles interne pour une machine de découpe ayant des découpe à approvisionner pour un lot donné
        public async Task<List<int>> GetArticleInterneIdListToBeSuppliedAsync(int decoupeLotId, string decoupeMachineId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.IdArticleInterne > 0
                        && dd.IndiceDecoupe == 1
                        && dd.Categorie4 == decoupeMachineId
                        && dd.ValidationLigne == true
                        && ((dd.Inactif == false && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
                .Select(dd => dd.IdArticleInterne)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des articles composés pour une machine de découpe ayant des découpe pour un lot donné (barre virtuelle)
        public async Task<List<int>> GetArticleComposeIdListToBeAddedAsync(int decoupeLotId, string decoupeMachineId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.IdArticleInterne > 0
                        && dd.IndiceDecoupe == 1
                        && dd.ApproOptimBarreChute == false
                        && dd.ApproOptimBarreNeuve == false
                        && dd.Categorie4 == decoupeMachineId
                        && dd.ValidationLigne == true
                        && dd.Inactif == true
                        && dd.ApproComposeInactif == true)
                .Select(dd => dd.IdArticleInterne)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 1 et pour un lot donné
        public async Task<List<DecoupeDetail>> GetIndice1ByLotAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.IndiceDecoupe == 1
                        && dd.Inactif == false
                        && dd.ValidationLigne == true
                        && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true))
                .OrderByDescending(detail => detail.NumLigne)
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour l'indice = 2 et pour un lot donné
        public async Task<List<DecoupeDetail>> GetIndice2ByLotAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                    && dd.IndiceDecoupe == 2
                    && dd.Inactif == false
                    && dd.ValidationLigne == true)
                .OrderByDescending(detail => detail.NumLigne)
                .ToListAsync();
        }

    }
}