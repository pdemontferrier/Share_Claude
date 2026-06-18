using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé d’extraire les machines de découpe à approvisionner pour un lot.
    /// </summary>
    public interface IS_DecoupeDetail_MachineList
    {
        /// <summary>
        /// Retourne une liste d’identifiants de machines à partir des découpes actives du lot donné.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Liste des identifiants de machines à approvisionner.</returns>
        Task<List<string>> ExecuteAsync(int decoupeLotId, string caller);
    }
}