using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de supprimer un enregistrement de stock si sa quantité est égale à zéro.
    /// </summary>
    public interface IS_Stock_DeleteIfQuantityNull
    {
        /// <summary> Supprime l'enregistrement de la table stock si sa quantité est nulle.</summary>
        /// <param name="stockId">Identifiant du stock à supprimer</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int stockId, string caller);
    }
}
