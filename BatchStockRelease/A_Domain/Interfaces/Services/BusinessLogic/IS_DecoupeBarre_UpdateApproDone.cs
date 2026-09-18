using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de marquer une barre comme sortie du stock.
    /// </summary>
    public interface IS_DecoupeBarre_UpdateApproDone
    {
        /// <summary>Met à jour le champ <c>sortie_stock</c> à <c>true</c> pour une barre partiellement utilisée.</summary>
        /// <param name="decoupeBarreId">Identifiant de la barre à mettre à jour.</param>
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
            int decoupeBarreId,
            DTO_AppContext appCtx,
            string caller);
    }
}