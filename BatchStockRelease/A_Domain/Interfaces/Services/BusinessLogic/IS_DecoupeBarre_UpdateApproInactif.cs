using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de marquer une barre de découpe comme inactive.
    /// </summary>
    public interface IS_DecoupeBarre_UpdateApproInactif
    {
        /// <summary>Met à jour le champ <c>ApproInactif</c> pour une barre dont la quantité est épuisée ou non trouvée.</summary>
        /// <param name="decoupeBarreId">Identifiant de la barre concernée.</param>
        /// <param name="appCtx">Paramètres de l'application.</param>
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
            int decoupeBarreId,
            DTO_AppContext appCtx,
            string caller);
    }
}