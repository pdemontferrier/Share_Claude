using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.QueryDataProviders.GestStock
{
    public class DP_DecoupeDetailWithCut : IR_DecoupeDetailWithCut
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        private readonly IS_Settings _settings;

        public DP_DecoupeDetailWithCut(IDbContextFactory<GestStockContext> contextFactory, IS_Settings settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
        }

        public async Task<DTO_DecoupeDetailWithCut?> GetFirstAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await (from dd in context.DecoupeDetails
                          join dl in context.DecoupeLots on dd.IdDecoupeLot equals dl.Id
                          join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                          join ai in context.ArticleInternes on dd.IdArticleInterne equals ai.Id
                          where dd.IdDecoupeLot == _settings.GetDecoupeLotId()
                                && dd.Id == _settings.GetDecoupeDetailId()
                                && dd.Categorie4 == _settings.GetCuttingMachine()
                                && db.ApproSortieFaite == true
                                && dd.DecoupeFaite == false

                          select new DTO_DecoupeDetailWithCut
                          {
                              Id = dd.Id,
                              IdCommandeClient = dd.IdCommandeClient,
                              NumProjet = dd.NumProjet,
                              NomProjet = dd.NomProjet,
                              Structure = dd.Structure,
                              Position = dd.Position,
                              NumLigne = dd.NumLigne,
                              IndiceDecoupe = dd.IndiceDecoupe,
                              ArticleInterneId = dd.IdArticleInterne,
                              Reference = dd.Reference,
                              Couleur = dd.Couleur,
                              Designation = dd.Designation,
                              LongueurBarre = dd.LongueurBarre,
                              LongueurDecoupe = dd.LongueurDecoupe,
                              Inclinaison1 = dd.Inclinaison1,
                              Pivot1 = dd.Pivot1,
                              Pivot2 = dd.Pivot2,
                              Inclinaison2 = dd.Inclinaison2,
                              ReferenceVue = dd.ReferenceVue,
                              Commentaires = dd.Commentaires,
                              MessageElumatec = dd.MessageElumatec,
                              DecoupeBarreId = dd.IdDecoupeBarre,
                              DecoupeBarreIndex = dd.DecoupeBarreIndex,
                              Decoupe = dd.DecoupeFaite,
                              DecoupeLotId = dd.IdDecoupeLot,
                              DecoupeLotDesignation = dl.Designation,
                              DecoupeBarreDecoupeNombre = db.DecoupeNombre,
                              DecoupeBarreLongueurReste = db.DecoupeLongueurReste,
                              DecoupeBarreTypeReste = db.DecoupeTypeReste,
                              DecoupeBarreGestionChutes = db.DecoupeGestionChutes,
                          }).FirstOrDefaultAsync();
        }

        // Retourne la liste des enregistrements de DecoupeDetail pour un lot donné et une machine donnée
        public async Task<List<DTO_DecoupeDetailWithCut>> GetAllByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            using var context = _contextFactory.CreateDbContext();

            return await (from dd in context.DecoupeDetails
                          join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                          join dl in context.DecoupeLots on dd.IdDecoupeLot equals dl.Id
                          where dd.IdDecoupeLot == decoupeLotId
                                && dd.Categorie4 == machineId
                                && dd.Inactif == false
                                && dd.ValidationLigne == true
                                && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true)
                                && dd.IdDecoupeBarre != 0
                                && dd.DecoupeBarreIndex != 0
                                && dd.Categorie2 == 1
                                && db.ApproSortieFaite == true
                          orderby dd.Structure,
                                dd.OrdreTri,
                                dd.Designation,
                                dd.Reference,
                                dd.IdArticleInterne,
                                dd.IdDecoupeBarre,
                                dd.DecoupeBarreIndex,
                                dd.ReferenceVue,
                                dd.Couleur,
                                dd.NumLigne,
                                dd.IndiceDecoupe
                          select new DTO_DecoupeDetailWithCut
                          {
                              Id = dd.Id,
                              IdCommandeClient = dd.IdCommandeClient,
                              NumProjet = dd.NumProjet,
                              NomProjet = dd.NomProjet,
                              Structure = dd.Structure,
                              Position = dd.Position,
                              NumLigne = dd.NumLigne,
                              IndiceDecoupe = dd.IndiceDecoupe,
                              ArticleInterneId = dd.IdArticleInterne,
                              Reference = dd.Reference,
                              Couleur = dd.Couleur,
                              Designation = dd.Designation,
                              LongueurBarre = dd.LongueurBarre,
                              LongueurDecoupe = dd.LongueurDecoupe,
                              Inclinaison1 = dd.Inclinaison1,
                              Pivot1 = dd.Pivot1,
                              Pivot2 = dd.Pivot2,
                              Inclinaison2 = dd.Inclinaison2,
                              ReferenceVue = dd.ReferenceVue,
                              Commentaires = dd.Commentaires,
                              MessageElumatec = dd.MessageElumatec,
                              DecoupeBarreId = dd.IdDecoupeBarre,
                              DecoupeBarreIndex = dd.DecoupeBarreIndex,
                              Decoupe = dd.DecoupeFaite,
                              DecoupeLotId = dd.IdDecoupeLot,
                              DecoupeLotDesignation = dl.Designation,
                              DecoupeBarreDecoupeNombre = db.DecoupeNombre,
                              DecoupeBarreLongueurReste = db.DecoupeLongueurReste,
                              DecoupeBarreTypeReste = db.DecoupeTypeReste,
                              DecoupeBarreGestionChutes = db.DecoupeGestionChutes
                          })
                        .AsNoTracking()
                        .ToListAsync();
        }
    }
}