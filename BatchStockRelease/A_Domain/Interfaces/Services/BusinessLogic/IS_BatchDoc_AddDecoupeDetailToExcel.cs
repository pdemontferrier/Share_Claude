using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’ajouter les données de découpe à un fichier Excel pour un lot donné.
    /// </summary>
    public interface IS_BatchDoc_AddDecoupeDetailToExcel
    {
        /// <summary>
        /// Ajoute les lignes de découpe à l’onglet Excel dédié pour un lot donné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="filePath">Emplacement et nom du fichier.</param>
        /// <param name="batchFileSheet1">Nom de l'onglet.</param>
        /// <param name="decoupeLotDesignation">Désignation du lot de découpe.</param>
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
            string filePath,
            string batchFileSheet1,
            string decoupeLotDesignation,
            string caller);
    }
}