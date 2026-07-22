using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Service chargé d’ajouter une nouvelle barre (neuve ou chute) pour une ligne de découpe donnée.
    /// </summary>
    public interface IS_DecoupeBarre_AddNewBarre
    {
        /// <summary>
        /// Ajoute une nouvelle barre et initialise ses propriétés à partir de la première découpe.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot concerné.</param>
        /// <param name="firstDecoupe">Ligne de découpe à l’origine de la barre.</param>
        /// <param name="isChute">Indique s’il s’agit d’une barre de chute.</param>
        /// <param name="chute">Objet représentant la chute utilisée (null si barre neuve).</param>
        /// <param name="isVirtualBar">Précise si la barre ajoutée est virtuelle).</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, DecoupeDetail firstDecoupe, bool isChute, VieChuteMagasinReference? chute, bool isVirtualBar, string caller);
    }
}