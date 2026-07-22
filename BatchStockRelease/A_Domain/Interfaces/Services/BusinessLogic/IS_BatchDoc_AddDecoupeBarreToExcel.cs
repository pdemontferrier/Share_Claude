using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’ajouter les données DecoupeBarre à un fichier Excel pour un lot donné.
    /// </summary>
    public interface IS_BatchDoc_AddDecoupeBarreToExcel
    {
        /// <summary>
        /// Écrit les données des barres découpées dans un onglet Excel dédié.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe.</param>
        /// <param name="filePath">Emplacement et nom du fichier.</param>
        /// <param name="batchFileSheet2">Nom de l'onglet.</param>
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
            string batchFileSheet2,
            string decoupeLotDesignation,
            string caller);
    }
}