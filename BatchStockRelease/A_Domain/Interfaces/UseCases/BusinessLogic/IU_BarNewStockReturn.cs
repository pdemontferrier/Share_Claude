using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase permettant d’effectuer un retour en stock de barres neuves dans le cadre de la préparation d’un lot de découpe.
    /// Mise à jour de l’état du stock, la consommation article interne, et l'historisation de la modification du stock'.
    /// </summary>
    public interface IU_BarNewStockReturn
    {
        /// <summary>
        /// Exécute le processus de libération des barres neuves du stock pour un lot de découpe donné.
        /// Ce processus comprend la mise à jour du stock, l'ajout d'une trace de consommation, la mise à jour de l'état de découpe
        /// et la suppression du stock si la quantité restante est nulle. 
        /// </summary>
        /// <param name="StockRecord">Entité VieStockQuantiteEmplacement concernée</param>
        /// <param name="quantiteRetour">Quantité à retourner en stock</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Mettre à jour la table Stock (<c> IS_Stock_UpdateQuantity </c>)
        /// 2. Ajouter une entrée dans la table StockModif (<c> IS_StockModif_Add </c>).
        /// 3. Ajouter une entrée dans la table ArticleInterneConsommation (<c> IS_AIConsommation_Add </c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(VieStockQuantiteEmplacement StockRecord, int quantiteRetour, string caller);
    }
}