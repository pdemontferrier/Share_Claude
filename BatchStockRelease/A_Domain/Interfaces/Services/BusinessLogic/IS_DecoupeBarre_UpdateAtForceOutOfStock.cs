using BatchStockRelease.A_Domain.App.DTOs;
using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service métier chargé de marquer toutes les barres neuves en rupture de stock comme sorties forcées,
    /// en mettant à jour les champs de traçabilité associés.
    /// </summary>
    public interface IS_DecoupeBarre_UpdateAtForceOutOfStock
    {
        /// <summary>
        /// Exécute l’opération de forçage de sortie de stock pour toutes les barres neuves en rupture
        /// du lot spécifié. Les barres concernées sont désactivées (Inactif = true) et marquées
        /// comme sorties forcées avec l’ensemble des informations de traçabilité de l’utilisateur.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à mettre à jour.</param>
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
            int decoupeLotId,
            DTO_AppContext appCtx,
            string caller);
    }
}