using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de mettre à jour les indices 2 de la table DecoupeDetail pour un lot donné.
    /// </summary>
    public interface IS_DecoupeDetail_UpdateIndice2
    {
        /// <summary>
        /// Met à jour les indices 2 de la table DecoupeDetail pour toutes les lignes concernées d’un lot donné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, string caller);
    }
}