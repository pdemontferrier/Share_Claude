using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé d’ajouter une chute dans la table d’archive.
    /// </summary>
    public interface IS_ChutesArchive_Add
    {
        /// <summary>Insère une nouvelle ligne dans la table <c>chutes_archive</c> à partir d’une chute utilisée.</summary>
        /// <param name="chuteMagasinId">Identifiant de la table (<c>ChuteMagasin</c>).</param>
        /// <param name="appDateTime">Timestamp</param>
        /// <param name="appUserId">User Id</param>
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
            int chuteMagasinId,
            DateTime appDateTime,
            int appUserId,
            string caller);
    }
}