using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Met à jour la quantité disponible dans le stock.
    /// </summary>
    public interface IS_Stock_UpdateQuantity
    {
        /// <summary>Met à jour la quantité disponible dans le stock et retourne un DTO_StockQuantityUpdate.</summary>
        /// <param name="idStock">Identifiant du stock à modifier</param>
        /// <param name="quantiteSortie">Quantité à soustraire</param>
        /// <param name="isSortie">Indique si le mouvement est une sortie ou un retour</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Retourne les éléments relatifs aux états de quantité du stock pour un id donné.</returns>
        Task<DTO_StockQuantity> ExecuteAsync(int idStock, int quantiteSortie, bool isSortie, string caller);
    }

}
