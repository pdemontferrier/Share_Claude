
namespace BatchCutting_DG.A_Domain.GestStock.DTOs
{
    public class DTO_DecoupeBarreWithCut
    {
        // Origine : DecoupeBarre
        public int Id { get; set; }
        public int IdDecoupeLot { get; set; }
        public int IdArticleInterne { get; set; }
        public string? ApproOrigine { get; set; }
        public string? ChariotDesignation { get; set; }
        public decimal? LongueurBarre { get; set; }
        public decimal? LongueurChuteMini { get; set; }
        public string? Categorie4 { get; set; }
        public int DecoupeNombre { get; set; }
        public decimal? LongueurReste { get; set; }
        public string? TypeReste { get; set; }
        public bool GestionChutes { get; set; }
        public string EmpSc { get; set; } = null!;
        public int IdEmpSc { get; set; }
        public int LongueurChuteFinale { get; set; }
        public string? CodeBarreChute { get; set; }
        public double? DernierPrixMm { get; set; }
        public bool Decoupe { get; set; }

        // Origine : DecoupeLot
        public string? DesignationLot { get; set; }

        // Origine : ArticleInterne
        public string? ReferenceArticle { get; set; }
        public string? DesignationArticle { get; set; }
        public string? CouleurArticle { get; set; }
    }
}