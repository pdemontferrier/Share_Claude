using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Commands;
using BatchStockRelease.A_Domain.Interfaces.Handlers.Queries;
using BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic;

namespace BatchStockRelease.B_UseCases.Services.BusinessLogic
{
    /// <summary>
    /// Met à jour la quantité disponible dans le stock.
    /// </summary>
    public class SR_Stock_UpdateQuantity : IS_Stock_UpdateQuantity
    {
        private readonly string _callee;
        private readonly IC_Stock _chStock;
        private readonly IQ_Stock _qhStock;

        public SR_Stock_UpdateQuantity(IC_Stock chStock, IQ_Stock qhStock)
        {
            _callee = GetType().Name;
            _chStock = chStock;
            _qhStock = qhStock;
        }

        /// <inheritdoc />
        public async Task<DTO_StockQuantity> ExecuteAsync(int idStock, int quantiteSortie, bool isSortie, string caller)
        {
            // Conctruire la callChain
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            if (idStock == 0)
                throw new Ex_Business(callChain,"STK_01", "No_Er_Bu_20");

            var stock = await _qhStock.HandleGetByIdAsync(idStock);

            if (stock == null)
                throw new Ex_Business(callChain, $"STK_02 : idStock {idStock}", "No_Er_Bu_21");

            if (isSortie && stock.Quantite <= 0)
                throw new Ex_Business(callChain,"STK_03", "No_Er_Bu_22");

            if ((isSortie && quantiteSortie <= 0) || (!isSortie && quantiteSortie >= 0))
                throw new Ex_Business(callChain,"STK_04", "No_Er_Bu_28");

            int quantiteAvant = (int)stock.Quantite;
            int quantite = Math.Min((int)stock.Quantite, quantiteSortie);
            stock.Quantite -= quantite;
            int quantiteApres = (int)stock.Quantite;

            try
            {
                await _chStock.HandleUpdateAsync(stock, callChain);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }

            return new DTO_StockQuantity(stock.Id, quantite, quantiteAvant, quantiteApres);
        }
    }
}