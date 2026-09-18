using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de mettre à jour les détails de l'approvisionnement associés à une barre.
    /// </summary>
    public interface IS_DecoupeDetail_AnnuleAllocation
    {
        /// <summary>Met à jour les enregistrements <c>decoupe_detail</c> liés à une barre spécifique.</summary>
        /// <param name="decoupeBarreId">Identifiant de la barre de découpe.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeBarreId, string caller);
    }
}