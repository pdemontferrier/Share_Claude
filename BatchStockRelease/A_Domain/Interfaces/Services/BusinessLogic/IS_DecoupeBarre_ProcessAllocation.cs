using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé d’allouer des barres neuves pour un article donné dans un lot.
    /// </summary>
    public interface IS_DecoupeBarre_ProcessAllocation
    {
        /// <summary>Tente d’allouer les barres nécessaires depuis le stock pour l’article interne spécifié dans le lot.</summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe.</param>
        /// <param name="articleInterneId">Identifiant de l’article interne à allouer.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, int articleInterneId, string caller);
    }
}