using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant la génération complète de la documentation de découpe pour un lot donné.
    /// </summary>
    public interface IU_BatchDocumentation
    {
        /// <summary>
        /// Exécute la génération de fichiers et de répertoires nécessaires à la documentation de découpe.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à documenter.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <returns>Tâche asynchrone</returns>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Préparer le répertoire du lot.
        /// 2. Ajouter les données de DecoupeDetail dans le fichier Excel.
        /// 3. Ajouter les données de DecoupeBarre dans le fichier Excel.
        /// 4. Générer les fichiers .elu pour les machines Elumatec.
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, string caller);
    }
}