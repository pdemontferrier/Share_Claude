using Microsoft.EntityFrameworkCore;
using BatchCutting_DG.A_Domain.GestStock.DTOs;
using BatchCutting_DG.C_Infrastructure.Persistence.GestStock;
using BatchCutting_DG.A_Domain.Interfaces.Services.App;
using BatchCutting_DG.A_Domain.Interfaces.Repositories.GestStock;

namespace BatchCutting_DG.C_Infrastructure.QueryDataProviders.GestStock
{
    public class DP_DecoupeBarreWithCut : IR_DecoupeBarreWithCut
    {
        private readonly IDbContextFactory<GestStockContext> _contextFactory;
        private readonly IS_Settings _settings;

        public DP_DecoupeBarreWithCut(IDbContextFactory<GestStockContext> contextFactory, IS_Settings settings)
        {
            _contextFactory = contextFactory;
            _settings = settings;
        }

        public async Task<DTO_DecoupeBarreWithCut?> GetFirstAsync()
        {
            using var context = _contextFactory.CreateDbContext();
            return await (from db in context.DecoupeBarres
                          join dl in context.DecoupeLots on db.IdDecoupeLot equals dl.Id
                          join ai in context.ArticleInternes on db.IdArticleInterne equals ai.Id
                          where db.IdDecoupeLot == _settings.GetDecoupeLotId()
                                && db.Id == _settings.GetDecoupeBarreId()
                                && db.Categorie4 == _settings.GetCuttingMachine()
                                && db.ApproSortieFaite == true
                                && db.DecoupeFaite == false

                          select new DTO_DecoupeBarreWithCut
                          {
                              Id = db.Id,
                              IdDecoupeLot = db.IdDecoupeLot,
                              IdArticleInterne = db.IdArticleInterne,
                              ApproOrigine = db.ApproOrigine,
                              ChariotDesignation = db.ApproChariotDesignation,
                              LongueurBarre = db.LongueurBarre,
                              LongueurChuteMini = db.LongueurChuteMini,
                              Categorie4 = db.Categorie4,
                              DecoupeNombre = db.DecoupeNombre,
                              LongueurReste = db.DecoupeLongueurReste,
                              TypeReste = db.DecoupeTypeReste,
                              GestionChutes = db.DecoupeGestionChutes,
                              EmpSc = db.DecoupeEmpSc,
                              IdEmpSc = db.DecoupeIdEmpSc,
                              LongueurChuteFinale = db.DecoupeLongueurChuteFinale,
                              CodeBarreChute = db.DecoupeCodeBarreChute,
                              DernierPrixMm = db.DernierPrixMm,
                              Decoupe = db.DecoupeFaite,
                              DesignationLot = dl.Designation,
                              ReferenceArticle = ai.Reference,
                              DesignationArticle = ai.Designation,
                              CouleurArticle = ai.Couleur
                          }).FirstOrDefaultAsync();
        }

        // Retourne la liste des enregistrements de DecoupeBarre pour un lot donné et une machine donnée
        public async Task<List<DTO_DecoupeBarreWithCut>> GetAllByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            using var context = _contextFactory.CreateDbContext();

            var grouped = await (
                from dd in context.DecoupeDetails
                join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                join dl in context.DecoupeLots on db.IdDecoupeLot equals dl.Id
                join ai in context.ArticleInternes on db.IdArticleInterne equals ai.Id
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
                select new
                {
                    dd.IdDecoupeBarre,
                    DTO = new DTO_DecoupeBarreWithCut
                    {
                        Id = db.Id,
                        IdDecoupeLot = db.IdDecoupeLot,
                        IdArticleInterne = db.IdArticleInterne,
                        ApproOrigine = db.ApproOrigine,
                        ChariotDesignation = db.ApproChariotDesignation,
                        LongueurBarre = db.LongueurBarre,
                        LongueurChuteMini = db.LongueurChuteMini,
                        Categorie4 = db.Categorie4,
                        DecoupeNombre = db.DecoupeNombre,
                        LongueurReste = db.DecoupeLongueurReste,
                        TypeReste = db.DecoupeTypeReste,
                        GestionChutes = db.DecoupeGestionChutes,
                        EmpSc = db.DecoupeEmpSc,
                        IdEmpSc = db.DecoupeIdEmpSc,
                        LongueurChuteFinale = db.DecoupeLongueurChuteFinale,
                        CodeBarreChute = db.DecoupeCodeBarreChute,
                        DernierPrixMm = db.DernierPrixMm,
                        Decoupe = db.DecoupeFaite,
                        DesignationLot = dl.Designation,
                        ReferenceArticle = ai.Reference,
                        DesignationArticle = ai.Designation,
                        CouleurArticle = ai.Couleur
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            // Regrouper par IdDecoupeBarre et conserver le premier (déjà trié)
            var groupedByBar = grouped
                .GroupBy(x => x.IdDecoupeBarre)
                .Select(g => g.First().DTO)
                .ToList();

            return groupedByBar;
        }

        // Requête spécifique : Retourne la liste des enregistrements de DecoupeBarre 'Chute" pour un lot donné et une machine donnée
        public async Task<List<DTO_DecoupeBarreWithCut>> GetAllDropBarByLotAndMachineAsync(int decoupeLotId, string machineId)
        {
            using var context = _contextFactory.CreateDbContext();

            var grouped = await (
                from dd in context.DecoupeDetails
                join db in context.DecoupeBarres on dd.IdDecoupeBarre equals db.Id
                join dl in context.DecoupeLots on db.IdDecoupeLot equals dl.Id
                join ai in context.ArticleInternes on db.IdArticleInterne equals ai.Id
                where dd.IdDecoupeLot == decoupeLotId
                      && dd.Categorie4 == machineId
                      && dd.Inactif == false
                      && dd.ValidationLigne == true
                      && (dd.ApproOptimBarreChute == true || dd.ApproOptimBarreNeuve == true)
                      && dd.IdDecoupeBarre != 0
                      && dd.DecoupeBarreIndex != 0
                      && dd.Categorie2 == 1
                      && db.ApproSortieFaite == true
                      && db.DecoupeTypeReste == "chute"
                      && db.DecoupeGestionChutes == true
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
                select new
                {
                    dd.IdDecoupeBarre,
                    DTO = new DTO_DecoupeBarreWithCut
                    {
                        Id = db.Id,
                        IdDecoupeLot = db.IdDecoupeLot,
                        IdArticleInterne = db.IdArticleInterne,
                        ApproOrigine = db.ApproOrigine,
                        ChariotDesignation = db.ApproChariotDesignation,
                        LongueurBarre = db.LongueurBarre,
                        LongueurChuteMini = db.LongueurChuteMini,
                        Categorie4 = db.Categorie4,
                        DecoupeNombre = db.DecoupeNombre,
                        LongueurReste = db.DecoupeLongueurReste,
                        TypeReste = db.DecoupeTypeReste,
                        GestionChutes = db.DecoupeGestionChutes,
                        EmpSc = db.DecoupeEmpSc,
                        IdEmpSc = db.DecoupeIdEmpSc,
                        LongueurChuteFinale = db.DecoupeLongueurChuteFinale,
                        CodeBarreChute = db.DecoupeCodeBarreChute,
                        DernierPrixMm = db.DernierPrixMm,
                        Decoupe = db.DecoupeFaite,
                        DesignationLot = dl.Designation,
                        ReferenceArticle = ai.Reference,
                        DesignationArticle = ai.Designation,
                        CouleurArticle = ai.Couleur
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            // Grouper par barre et conserver la première occurrence (déjà triée)
            var groupedByBar = grouped
                .GroupBy(x => x.IdDecoupeBarre)
                .Select(g => g.First().DTO)
                .ToList();

            return groupedByBar;
        }

    }
}