using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant la mise à jour d’un lot de découpe lors de la fin d’un approvisionnement.
    /// </summary>
    public interface IU_DecoupeLot_UpdateAtStockRelease
    {
        /// <summary>
        /// Exécute le processus de mise à jour du lot de découpe (approvisionnement chute ou neuf).
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe à traiter.</param>
        /// <param name="isChute">Indique s’il s’agit d’un approvisionnement chute (true) ou neuf (false).</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Mettre à jour DecouoeLot pour decoupeLotId et indiquer si isChute (<c> IS_DecoupeLot_UpdateAtStockRelease </c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, bool isChute, string caller);
    }
}