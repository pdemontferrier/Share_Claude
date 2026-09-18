using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service de validation du chariot sélectionné pour l’approvisionnement.
    /// Met à jour la table DecoupeBarre avec les informations du chariot choisi.
    /// </summary>
    public interface IS_DecoupeBarre_UpdateChariot
    {
        /// <summary>
        /// Met à jour les enregistrements <c>DecoupeBarre</c> avec le chariot sélectionné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe concerné.</param>
        /// <param name="chariotId">Identifiant du chariot concerné.</param>
        /// <param name="chariotDesignation">Désignation du chariot concerné.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, int chariotId, string chariotDesignation, string caller);
    }
}