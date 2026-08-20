using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeDetail : CR_Generic<DecoupeDetail>, IR_DecoupeDetail
    {
        public CR_DecoupeDetail(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
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
                        && ((dd.Inactif == false && dd.ValidationLigne == true && dd.ApproComposeInactif == false)
                        ||  (dd.Inactif == true && dd.ValidationLigne == false  && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
                .OrderByDescending(detail => detail.LongueurOptim)
                .ToListAsync();
        }

        // Requête spécifique : Retourne les découpes d'article composé (virtuel) pour un lot, une machine, et un article donné
        public async Task<List<DecoupeDetail>> GetArticleComposeToBeAddedAsync(int decoupeLotId, string decoupeMachineId, int articleInterneId)
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
                        && dd.Inactif == false
                        && dd.ApproComposeInactif == true)
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
                        && ((dd.Inactif == false && dd.ValidationLigne == true && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ValidationLigne == false && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
                .Select(dd => dd.Categorie4!)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des machines de découpe DG ayant des découpe à approvisionner pour un lot donné
        public async Task<List<string>> GetCuttingMachineDGListToBeSuppliedAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.IndiceDecoupe == 1
                        && dd.ApproOptimBarreChute == false
                        && dd.ApproOptimBarreNeuve == false
                        && dd.Categorie4.StartsWith("DG")
                        && ((dd.Inactif == false && dd.ValidationLigne == true && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ValidationLigne == false && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
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
                        && dd.ApproOptimBarreChute == false
                        && dd.ApproOptimBarreNeuve == false
                        && dd.Categorie4 == decoupeMachineId
                        && ((dd.Inactif == false && dd.ValidationLigne == true && dd.ApproComposeInactif == false)
                        || (dd.Inactif == true && dd.ValidationLigne == false && dd.ApproComposant == true && dd.ApproCompoNeuf == true)))
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
                        && dd.Inactif == false
                        && dd.ApproComposeInactif == true)
                .Select(dd => dd.IdArticleInterne)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeDetail pour une barre donnée
        public async Task<List<DecoupeDetail>> GetAllByDecoupeBarreIdAsync(int decoupeBarreId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeBarre == decoupeBarreId
                        && dd.Inactif == false
                        && dd.ValidationLigne == true
                        && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true))
                .OrderByDescending(detail => detail.NumLigne)
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

        /// <summary>
        /// Requête spécifique : Retourne la liste complète des enregistrements DecoupeDetail pour un lot donné dans un ordre bien précis.
        /// </summary>
        public async Task<List<DecoupeDetail>> GetBatchDecoupeDetailAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.Inactif == false
                        && dd.ValidationLigne == true)
                .OrderBy(d => d.Structure)
                .ThenBy(d => d.OrdreTri)
                .ThenBy(dd => dd.Designation)
                .ThenBy(idc => idc.Reference)
                .ThenBy(idc => idc.Couleur)
                .ThenBy(d => d.IdArticleInterne)
                .ThenBy(d => d.IdDecoupeBarre)
                .ThenBy(d => d.DecoupeBarreIndex)
                .ThenBy(d => d.NumLigne)
                .ThenBy(d => d.IndiceDecoupe)
                .ToListAsync();
        }

        /// <summary>
        /// Requête spécifique : Retourne la liste des machines de découpe ayant des découpe pour un lot donné
        /// </summary>
        public async Task<List<string?>> GetBatchCuttingMachineListAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.Inactif == false
                        && dd.ValidationLigne == true
                        && dd.Categorie2 == 1)
                .Select(dd => dd.Categorie4)
                .Distinct()
                .ToListAsync();
        }

        /// <summary>
        /// Requête spécifique : Retourne la liste des MessageElumatec des enregistrements DecoupeDetail pour un lot donné dans un ordre bien précis.
        /// </summary>
        public async Task<List<string?>> GetBatchDecoupeDetailMessageElumatecAsync(int decoupeLotId, string categorie4Value)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeDetails
                .AsNoTracking()
                .Where(dd => dd.IdDecoupeLot == decoupeLotId
                        && dd.Categorie4 == categorie4Value
                        && dd.Inactif == false
                        && dd.ValidationLigne == true
                        && dd.Categorie2 == 1)
                .OrderBy(dd => dd.IdDecoupeLot)
                .ThenBy(dd => dd.Structure)
                .ThenBy(dd => dd.OrdreTri)
                .ThenBy(dd => dd.Designation)
                .ThenBy(idc => idc.Reference)
                .ThenBy(idc => idc.Couleur)
                .ThenBy(d => d.IdArticleInterne)
                .ThenBy(d => d.IdDecoupeBarre)
                .ThenBy(d => d.DecoupeBarreIndex)
                .ThenBy(d => d.NumLigne)
                .ThenBy(d => d.IndiceDecoupe)
                .Select(dd => dd.MessageElumatec)
                .ToListAsync();
        }
    }
}