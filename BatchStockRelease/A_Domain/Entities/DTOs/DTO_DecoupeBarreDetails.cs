
namespace BatchStockRelease.A_Domain.GestStock.DTOs
{
    public class DTO_DecoupeBarreDetails
    {
        public int Id { get; set; }
        public int IdDecoupeLot { get; set; }
        public int IdArticleInterne { get; set; }
        public int IdStock { get; set; }
        public decimal? LongueurBarre { get; set; }
        public decimal? LongueurChuteMini { get; set; }
        public int? Categorie1 { get; set; }
        public int? Categorie2 { get; set; }
        public int? Categorie3 { get; set; }
        public string? Categorie4 { get; set; }
        public int? OrdreTri { get; set; }
        public string? ApproOrigine { get; set; }
        public string? ApproCodeBarre { get; set; }
        public bool ApproRupture { get; set; }
        public int ApproZonePriorite { get; set; }
        public string? ApproZoneDesignation { get; set; }
        public int ApproAdressePriorite { get; set; }
        public string? ApproAdresseDesignation { get; set; }
        public int ApproEmplacement { get; set; }
        public string? ApproEmplacementDesignation { get; set; }
        public string? ApproConteneur { get; set; }
        public string? ApproChariotDesignation { get; set; }
        public bool ApproSortieFaite { get; set; }
        public bool ApproSortieForce { get; set; }
        public bool? ApproInactif { get; set; }
        public int DecoupeNombre { get; set; }
        public decimal? DecoupeLongueurReste { get; set; }
        public string? DecoupeTypeReste { get; set; }
        public string? Reference { get; set; }
        public string? Couleur { get; set; }
        public string? Designation { get; set; }
        public int? QuantiteASortir { get; set; }

    }
}