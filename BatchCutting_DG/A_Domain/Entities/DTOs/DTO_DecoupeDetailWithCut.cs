
namespace BatchCutting_DG.A_Domain.GestStock.DTOs
{
    public class DTO_DecoupeDetailWithCut
    {
        // Origine : DecoupeDetail
        public int Id { get; set; }
        public int IdCommandeClient { get; set; }
        public string? NumProjet { get; set; }
        public string? NomProjet { get; set; }
        public string? Structure { get; set; }
        public string? Position { get; set; }
        public ulong? NumLigne { get; set; }
        public int IndiceDecoupe { get; set; }
        public int ArticleInterneId { get; set; }
        public string? Reference { get; set; }
        public string? Couleur { get; set; }
        public string? Designation { get; set; }
        public decimal? LongueurBarre { get; set; }
        public decimal? LongueurDecoupe { get; set; }
        public decimal? Inclinaison1 { get; set; }
        public decimal? Pivot1 { get; set; }
        public decimal? Pivot2 { get; set; }
        public decimal? Inclinaison2 { get; set; }
        public string? ReferenceVue { get; set; }
        public string? Commentaires { get; set; }
        public string? MessageElumatec { get; set; }
        public int DecoupeBarreId { get; set; }
        public int DecoupeBarreIndex { get; set; }
        public bool Decoupe { get; set; }

        // Origine : DecoupeLot
        public int DecoupeLotId { get; set; }
        public string? DecoupeLotDesignation { get; set; } = string.Empty;

        // Origine : DecoupeBarre
        public int DecoupeBarreDecoupeNombre { get; set; }
        public decimal? DecoupeBarreLongueurReste { get; set; }
        public string? DecoupeBarreTypeReste { get; set; }
        public bool DecoupeBarreGestionChutes { get; set; }

    }
}