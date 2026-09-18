using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé d’enregistrer une modification de statut sur une commande client.
    /// </summary>
    public interface IS_CommandeClientModification_Add
    {
        /// <summary>
        /// Crée et enregistre une ligne d’historique de modification liée à une commande client.
        /// </summary>
        /// <param name="commande">Commande client concernée par la modification.</param>
        /// <param name="statutId">Identifiant du nouveau statut appliqué à la commande.</param>
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
            int statutId,
            DateTime appDateTime,
            int appUserId,
            string caller);
    }
}