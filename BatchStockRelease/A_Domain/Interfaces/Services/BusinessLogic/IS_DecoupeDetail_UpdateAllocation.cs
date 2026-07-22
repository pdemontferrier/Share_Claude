using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’ajouter une nouvelle barre (neuve ou chute) pour une ligne de découpe donnée.
    /// </summary>
    public interface IS_DecoupeDetail_UpdateAllocation
    {
        /// <summary>
        /// AMettre à jour la Table DecoupeDetail après avoir allocation d'une barre.
        /// </summary>
        /// <param name="decoupeEnTraitement">Ligne de découpe allouée.</param>
        /// <param name="isChute">Indique s’il s’agit d’une barre de chute.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(DecoupeDetail decoupeEnTraitement, bool isChute, string caller);
    }
}