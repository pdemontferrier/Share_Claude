using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de générer et copier les fichiers .elu de découpe pour un lot donné.
    /// </summary>
    public interface IS_BatchDoc_AddEluFiles
    {
        /// <summary>
        /// Génère et copie les fichiers .elu pour les machines du lot spécifié.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="batchDirectoryPath">Emplacement et nom du lot</param>
        /// <param name="batchFileName">Designation du lot</param>
        /// <param name="directoryPathServer2">Emplacement et nom du fichier.</param>
        /// <param name="directoryPathElumatec">Emplacement dossier machine</param>
        /// <param name="directoryPathElumatec1">Emplacement dossier machine correspondant.</param>
        /// <param name="directoryPathElumatec2">Emplacement dossier machine correspondant.</param>
        /// <param name="directoryPathElumatec3">Emplacement dossier machine correspondant.</param>
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
            string batchDirectoryPath,
            string batchFileName,
            string directoryPathServer2,
            string directoryPathElumatec,
            string directoryPathElumatec1,
            string directoryPathElumatec2,
            string directoryPathElumatec3,
            string caller);
    }
}