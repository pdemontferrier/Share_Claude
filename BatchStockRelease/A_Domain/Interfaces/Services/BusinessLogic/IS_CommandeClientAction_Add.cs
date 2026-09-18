using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé d’ajouter une action client à une commande spécifiée.
    /// </summary>
    public interface IS_CommandeClientAction_Add
    {
        /// <summary>
        /// Crée et enregistre une nouvelle action client pour une commande donnée.
        /// </summary>
        /// <param name="commande">Commande concernée par l’action client.</param>
        /// <param name="idAction">Identifiant de l’action à enregistrer.</param>
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
            CommandeClient commande,
            int idAction,
            DateTime appDateTime,
            int appUserId,
            string caller);
    }
}