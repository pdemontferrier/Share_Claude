using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase permettant d’effectuer la sortie de stock de barres neuves dans le cadre de la préparation d’un lot de découpe.
    /// L'opération est transactionnelle et comprend la mise à jour du stock, la traçabilité de la sortie,
    /// la mise à jour de l’état de découpe, la consommation article interne, et la suppression éventuelle du stock.
    /// </summary>
    public interface IU_BarNewStockRelease
    {
        /// <summary>
        /// Exécute le processus de libération des barres neuves du stock pour un lot de découpe donné.
        /// Ce processus comprend la mise à jour du stock, l'ajout d'une trace de consommation, la mise à jour de l'état de découpe
        /// et la suppression du stock si la quantité restante est nulle. 
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe concerné</param>
        /// <param name="decoupeBarreIdStock">Identifiant IdStock de la barre de découpe concerné</param>
        /// <param name="quantiteSortie">Quantité à sortir du stock</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Mettre à jour la table Stock (<c> IS_Stock_UpdateQuantity </c>)
        /// 2. Ajouter une entrée dans la table StockModif (<c> IS_StockModif_Add </c>).
        /// 3. Ajouter une entrée dans la table ArticleInterneConsommation (<c> IS_AIConsommation_Add </c>).
        /// 4. Mettre à jour la table DecoupeBarre (<c> IS_DecoupeBarre_UpdateStockReleaseState </c>).
        /// 5. Supprimer l'enregistrement de la table Stock si la quantité = 0 (<c> IS_Stock_DeleteIfQuantityNull </c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, int decoupeBarreIdStock, int quantiteSortie, string caller);
    }
}