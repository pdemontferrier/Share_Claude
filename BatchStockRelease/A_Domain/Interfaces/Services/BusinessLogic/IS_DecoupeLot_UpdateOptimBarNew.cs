using BatchStockRelease.A_Domain.Common.Exceptions;
namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de mettre à jour l’état d’un lot lors de la fin de l’optimisation.
    /// </summary>
    public interface IS_DecoupeLot_UpdateOptimBarNew
    {
        /// <summary>Met à jour les champs d’optimisation d’un lot de découpe et enregistre une action utilisateur.</summary>
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
