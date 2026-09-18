using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé de finaliser les informations d’une barre (quantité, chute, type) après optimisation.
    /// </summary>
    public interface IS_DecoupeBarre_FinalizeBarre
    {
        /// <summary>
        /// Finalise les informations d'une barre de découpe, neuve ou de chute.
        /// </summary>
        /// <param name="decoupeBarre">Objet représentant la barre à finaliser.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(DecoupeBarre decoupeBarre, string caller);
    }
}