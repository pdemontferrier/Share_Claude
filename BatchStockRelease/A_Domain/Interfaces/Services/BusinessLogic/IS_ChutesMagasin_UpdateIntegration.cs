using BatchStockRelease.A_Domain.Entities.GestStock;
using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du Service métier chargé de mettre à jour un enregistrement de la table <c>ChutesMagasin</c>
    /// en marquant l'état <c>AttenteIntegration</c> à <c>false</c> pour signaler l'intégration.
    /// </summary>
    public interface IS_ChutesMagasin_UpdateIntegration
    {
        /// <summary>Met à jour l’enregistrement d’une chute de la table <c>chutes_magasin</c>.</summary>
        /// <param name="chuteMagasin">Enregistrement de la table (<c>ChuteMagasin</c>).</param>
        /// <param name="appDateTime">Timestamp</param>
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
            ChutesMagasin chuteMagasin,
            DateTime appDateTime,
            string caller);
    }
}