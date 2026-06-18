using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de calculer et créer les barres neuves nécessaires à un lot donné.
    /// </summary>
    public interface IS_DecoupeBarre_OptimBarNew
    {
        /// <summary>
        /// Exécute l’optimisation et la création des barres neuves pour un lot et une machine donnés.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="decoupeMachineId">Identifiant de la machine de découpe.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, string decoupeMachineId, string caller);
    }
}
