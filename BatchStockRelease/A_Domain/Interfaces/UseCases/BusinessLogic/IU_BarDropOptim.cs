using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase chargé d’optimiser et d’allouer les barres de chute à découper pour un lot donné.
    /// </summary>
    public interface IU_BarDropOptim
    {
        /// <summary>
        /// Usecase chargé de l’optimisation et l’allocation des barres de chute pour un lot de découpe donné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Obtenir la liste des machines à approvisionner (<c>IS_DecoupeDetail_MachineList</c>).
        /// 2. Pour chaque machine, exécuter l’optimisation des barres de chute (<c>IS_DecoupeBarre_OptimBarDrop</c>).
        /// 3. Mettre à jour les découpes avec indice 2 (<c>IS_DecoupeDetail_UpdateIndice2</c>).
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