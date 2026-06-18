
namespace BatchStockRelease.A_Domain.GestStock.DTOs
{
    public class DTO_StockQuantity
    {
        public int IdStock { get; set; }
        public int QuantiteSortie { get; set; }
        public int QuantiteAvant { get; set; }
        public int QuantiteApres { get; set; }

        public DTO_StockQuantity(int stockId, int quantiteSortie, int quantiteAvant, int quantiteApres)
        {
            IdStock = stockId;
            QuantiteSortie = quantiteSortie;
            QuantiteAvant = quantiteAvant;
            QuantiteApres = quantiteApres;
        }
    }
}
