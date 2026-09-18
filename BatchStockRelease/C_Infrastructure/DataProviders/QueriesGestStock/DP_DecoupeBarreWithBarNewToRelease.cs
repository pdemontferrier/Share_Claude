using Microsoft.EntityFrameworkCore;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.C_Infrastructure.Persistence.GestStock;
using BatchStockRelease.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchStockRelease.C_Infrastructure.DataProviders.QueriesGestStock
{
    public class DP_DecoupeBarreWithBarNewToRelease : IR_DecoupeBarreWithBarNewToRelease
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        public DP_DecoupeBarreWithBarNewToRelease(IDbContextFactory<GestStockContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Retourne la liste enrichie des enregistrements DecoupeBarre pour un lot donné.
        /// </summary>
        public async Task<List<DTO_DecoupeBarreWithBarNewToRelease>> GetAsync(int decoupeLotId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await (
                from db in context.DecoupeBarres
                join ai in context.ArticleInternes on db.IdArticleInterne equals ai.Id
                join s in context.Stocks on db.IdStock equals s.Id
                where db.IdDecoupeLot == decoupeLotId
                      && db.ApproOrigine == "neuf"
                      && db.ApproAllocation == true
                      && db.ApproSortieFaite == false
                      && db.ApproInactif == false

                select new DTO_DecoupeBarreWithBarNewToRelease
                {
                    Id = db.Id,
                    IdDecoupeLot = db.IdDecoupeLot,
                    IdArticleInterne = db.IdArticleInterne,
                    IdStock = db.IdStock,
                    LongueurBarre = db.LongueurBarre,
                    LongueurChuteMini = db.LongueurChuteMini,
                    Categorie1 = db.Categorie1,
                    Categorie2 = db.Categorie2,
                    Categorie3 = db.Categorie3,
                    Categorie4 = db.Categorie4,
                    OrdreTri = db.OrdreTri,
                    ApproOrigine = db.ApproOrigine,
                    ApproCodeBarre = db.ApproCodeBarre,
                    ApproRupture = db.ApproRupture,
                    ApproZonePriorite = db.ApproZonePriorite,
                    ApproZoneDesignation = db.ApproZoneDesignation,
                    ApproAdressePriorite = db.ApproAdressePriorite,
                    ApproAdresseDesignation = db.ApproAdresseDesignation,
                    ApproEmplacement = db.ApproEmplacement,
                    ApproEmplacementDesignation = db.ApproEmplacementDesignation,
                    ApproChariotDesignation = db.ApproChariotDesignation,
                    ApproConteneur = db.ApproConteneur,
                    ApproSortieFaite = db.ApproSortieFaite,
                    ApproInactif = db.ApproInactif,
                    DecoupeNombre = db.DecoupeNombre,
                    DecoupeLongueurReste = db.DecoupeLongueurReste,
                    DecoupeTypeReste = db.DecoupeTypeReste,
                    Reference = ai.Reference,
                    Couleur = ai.Couleur,
                    Designation = ai.Designation,
                    QuantiteStock = s != null ? (double?)s.Quantite : null
                })
                .OrderBy(db => db.ApproZonePriorite)
                .ThenBy(db => db.ApproAdressePriorite)
                .ThenBy(db => db.ApproConteneur)
                .ThenBy(db => db.IdArticleInterne)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}