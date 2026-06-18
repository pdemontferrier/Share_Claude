
namespace BatchStockRelease.A_Domain.GestStock.DTOs
{
    public class DTO_DecoupeLotWithBarNewToRelease
    {
        public int Id { get; set; }
        public string? Designation { get; set; }
        public int NombreBarres { get; set; }
        public int ApproIdChariot { get; set; }
        public bool ApproRupture { get; set; }
    }
}