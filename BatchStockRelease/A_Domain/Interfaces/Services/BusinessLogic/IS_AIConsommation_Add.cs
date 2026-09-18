using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.GestStock.DTOs;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’enregistrer une ligne de consommation dans la table ArticleInterneConsommation.
    /// </summary>
    public interface IS_AIConsommation_Add
    {
        /// <summary>Enregistre une consommation d’article interne associée à un lot de découpe.</summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe</param>
        /// <param name="idArticleInterne">Identifiant de l’article consommé</param>
        /// <param name="isSortie">Indique si le mouvement est une sortie ou un retour</param>
        /// <param name="approConteneur">Code du conteneur utilisé</param>
        /// <param name="approAdresseDesignation">Désignation de l’adresse de stockage</param>
        /// <param name="result">Informations de quantité consommée</param>
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
            int decoupeLotId,
            int idArticleInterne,
            bool isSortie,
            string? approConteneur,
            string? approAdresseDesignation,
            DTO_StockQuantity result,
            DateTime appDateTime,
            int appUserId,
            string caller);
    }
}