using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de mettre à jour les enregistrements de barres neuves dans la table DecoupeBarre
    /// suite à une sortie de stock. Marque les barres comme sorties ou les réinitialise si non sorties.
    /// </summary>
    public interface IS_DecoupeBarre_UpdateStockReleaseState
    {
        /// <summary>Met à jour les lignes DecoupeBarre associées à un stock donné.
        /// Les lignes correspondant à la quantité réellement sortie sont marquées comme libérées,
        /// les autres sont réinitialisées pour une future réallocation.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe</param>
        /// <param name="result">Détails de la sortie de stock</param>
        /// <param name="appCtx">Paramètres de l'application.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(
            int decoupeLotId, 
            DTO_StockQuantity result,
            DTO_AppContext appCtx,
            string caller);
    }
}