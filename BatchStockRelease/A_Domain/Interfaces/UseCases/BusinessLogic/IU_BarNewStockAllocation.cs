using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant l’allocation des barres neuves depuis le stock par article interne.
    /// </summary>
    public interface IU_BarNewStockAllocation
    {
        /// <summary>
        /// Exécute l’allocation des barres neuves pour un lot donné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Remettre à zéro les ruptures de stock (<c> IS_DecoupeBarre_ClearOutOfStockStatus </c>)
        /// 2. Récupérer la liste des IdArticleInterne à traiter (<c> IQ_DecoupeBarre </c>).
        /// 3. Parcourir la liste des IdArticleInterne (<c> IS_DecoupeBarre_ProcessAllocation </c>).
        /// 4. Mettre à jour les informations des ruptures de stock (<c> IS_DecoupeBarre_UpdateOutOfStockStatus </c>).
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