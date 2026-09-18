using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace BatchStockRelease.C_Infrastructure.Repositories.GestStock
{
    public class CR_DecoupeBarre : CR_Generic<DecoupeBarre>, IR_DecoupeBarre
    {
        public CR_DecoupeBarre(IDbContextFactory<GestStockContext> contextFactory)
            : base(contextFactory)
        {
        }

        // Requête spécifique : Retourne les informations relatives au chariot utilisé pour un lot donné
        public async Task<(int chariotId, string chariotDesignation)> GetChariotInfoAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            var chariotInfo = await context.DecoupeBarres
                .AsNoTracking()
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproIdChariot != 0
                        && db.ApproChariotDesignation != null)
                .Select(d => new { d.ApproIdChariot, d.ApproChariotDesignation })
                .FirstOrDefaultAsync();

            if (chariotInfo == null)
                return (0, string.Empty);

            return (chariotInfo.ApproIdChariot, chariotInfo.ApproChariotDesignation!);
        }

        // Requête spécifique : Retourne une liste d'enregistrement pour un lot et un article interne donné
        public async Task<List<DecoupeBarre>> GetAllForArticleInterneIdAsync(int decoupeLotId, int articleInterneId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                            && db.IdArticleInterne == articleInterneId
                            && db.ApproOrigine == "neuf"
                            && db.ApproAllocation == false
                            && db.ApproRupture == false
                            && db.ApproSortieFaite == false
                            && db.ApproInactif == false)
                .OrderBy(db => db.IdArticleInterne)
                .ThenBy(db => db.Id)
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des IdArticleInterne pour un lot donné, correspondant à des barres neuves non encore allouées ni sorties.
        public async Task<List<int>> GetDistinctArticleInterneIdAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .AsNoTracking()
                .Where(d => d.IdDecoupeLot == decoupeLotId
                        && d.ApproOrigine == "neuf"
                        && d.ApproAllocation == false
                        && d.ApproSortieFaite == false
                        && d.ApproInactif == false)
                .Select(d => d.IdArticleInterne)
                .Distinct()
                .ToListAsync();
        }

        // Requête spécifique : Retourne la liste des barres neuve pour un lot et un idStock donné
        public async Task<List<DecoupeBarre>> GetAllBarNewByIdStock(int decoupeLotId, int idStock)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId 
                            && db.ApproOrigine == "neuf" 
                            && db.ApproSortieFaite == false
                            && db.IdStock == idStock
                            && db.ApproInactif == false)
                .ToListAsync();
        }

        #region Allocated

        // Requête spécifique : Retourne la liste des barres allouées pour un lot donné.
        public async Task<List<DecoupeBarre>> GetAllocatedAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproAllocation == true
                        && db.ApproSortieFaite == false
                        && db.ApproInactif == false)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier si il existe des barre allouées
        public async Task<bool> CheckAllocated(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.DecoupeBarres
                .AnyAsync(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproAllocation == true
                            && db.ApproSortieFaite == false
                            && db.ApproInactif == false);
        }

        #endregion

        #region Not Allocated

        // Requête spécifique : Retourne la liste des barres neuves non allouées pour un lot donné.
        public async Task<List<DecoupeBarre>> GetBarNewNotAllocatedAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproOrigine == "neuf"
                        && db.ApproAllocation == false
                        && db.ApproSortieFaite == false
                        && db.ApproInactif == false)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier si il existe des barre neuves non allouées
        public async Task<bool> CheckBarNewNotAllocated(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();
            return await context.DecoupeBarres
                .AnyAsync(db => db.IdDecoupeLot == decoupeLotId
                            && db.ApproOrigine == "neuf"
                            && db.ApproAllocation == false
                            && db.ApproSortieFaite == false
                            && db.ApproInactif == false);
        }

        #endregion

        #region To release

        // Requête spécifique : Retourne la liste des barres de chute à approvisionner pour un lot donné
        public async Task<List<DecoupeBarre>> GetBarDropToReleaseAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproOrigine == "chute"
                        && db.ApproAllocation == true
                        && db.ApproRupture == false
                        && db.ApproSortieFaite == false
                        && db.ApproInactif == false)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier si il existe des barre de chute à approvisionner
        public async Task<bool> CheckBarDropToReleaseAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .AnyAsync(db => db.IdDecoupeLot == decoupeLotId
                                && db.ApproOrigine == "chute"
                                && db.ApproAllocation == true
                                && db.ApproRupture == false
                                && db.ApproSortieFaite == false
                                && db.ApproInactif == false);
        }

        // Requête spécifique : Retourne la liste des barres neuves à approvisionner pour un lot donné
        public async Task<List<DecoupeBarre>> GetBarNewToReleaseAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproOrigine == "neuf"
                        && db.ApproAllocation == true
                        && db.ApproInactif == false
                        && db.ApproSortieFaite == false
                        && db.ApproInactif == false)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier si il existe des barre neuves à approvisionner
        public async Task<bool> CheckBarNewToReleaseAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .AnyAsync(db => db.IdDecoupeLot == decoupeLotId
                                && db.ApproOrigine == "neuf"
                                && db.ApproAllocation == true
                                && db.ApproRupture == false
                                && db.ApproSortieFaite == false
                                && db.ApproInactif == false);
        }

        // Requête spécifique : Retourne vrai si au moins une barre neuve allouée référence un IdStock inexistant.
        public async Task<List<DecoupeBarre>> GetBarNewAllocatedToReallocateAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            var query =
                from db in context.DecoupeBarres
                where db.IdDecoupeLot == decoupeLotId
                      && db.ApproOrigine == "neuf"
                      && db.ApproAllocation
                      && !db.ApproRupture
                      && !db.ApproSortieFaite
                      && !db.ApproInactif
                      && db.IdStock > 0 // alloué historiquement à un stock
                join s in context.Stocks on db.IdStock equals s.Id into gj
                from s in gj.DefaultIfEmpty()
                where s == null // pas de correspondance dans Stock => lien cassé
                select db;

            return await query.ToListAsync();
        }
        #endregion

        #region Out of Stock

        // Requête spécifique : Retourne la liste des barres neuves en rupture de stock pour un lot donné.
        public async Task<List<DecoupeBarre>> GetBarNewOutOfStockAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                .Where(db => db.IdDecoupeLot == decoupeLotId
                        && db.ApproOrigine == "neuf"
                        && db.ApproAllocation == false
                        && db.ApproRupture == true
                        && db.ApproSortieFaite == false
                        && db.ApproInactif == false)
                .ToListAsync();
        }

        // Requête spécifique : Vérifier si il existe des barres neuves en rupture de stock
        public async Task<bool> CheckBarNewOutOfStock(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await context.DecoupeBarres
                    .AnyAsync(db => db.IdDecoupeLot == decoupeLotId
                                && db.ApproOrigine == "neuf"
                                && db.ApproAllocation == false
                                && db.ApproRupture == true
                                && db.ApproSortieFaite == false
                                && db.ApproInactif == false);
        }

        #endregion
    }
}