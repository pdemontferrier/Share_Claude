using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de supprimer une chute du stock magasin.
    /// </summary>
    public interface IS_ChutesMagasin_Delete
    {
        /// <summary>Supprime l’enregistrement d’une chute de la table <c>chutes_magasin</c>.</summary>
        /// <param name="chuteMagasinId">Identifiant de la table (<c>ChuteMagasin</c>).</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int chuteMagasinId, string caller);
    }
}