using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’enregistrer une ligne dans la table StockModif pour tracer la consommation d’un article interne.
    /// </summary>
    public interface IS_StockModif_Add
    {
        /// <summary>
        /// Enregistre une modification de stock pour un article interne donné.
        /// </summary>
        /// <param name="idArticleInterne">Identifiant de l’article interne concerné</param>
        /// <param name="isSortie">Indique si le mouvement est une sortie ou un retour</param>
        /// <param name="approConteneur">Code du conteneur utilisé</param>
        /// <param name="result">Données de la mise à jour de stock</param>
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
            int idArticleInterne,
            bool isSortie,
            string? approConteneur,
            DTO_StockQuantity result,
            DateTime appDateTime,
            int appUserId,
            string caller);
    }
}