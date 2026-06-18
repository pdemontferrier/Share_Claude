using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrant les opérations de sortie de stock d’une barre de chute.
    /// </summary>
    public interface IU_BarDropStockRelease
    {
        /// <summary>
        /// Exécute les opérations de sortie du stock de chute : archivage, suppression, mise à jour DecoupeBarre et si besoin DecoupeDetail.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="decoupeBarreId">Identifiant de la barre à traiter.</param>
        /// <param name="chuteMagasinId">Identifiant de la table (<c>ChuteMagasin</c>).</param>
        /// <param name="quantite">Quantité restante après coupe (0 ou 1).</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Ajouter un nouvel enregistrement dans ChuteArchive (<c>IS_ChuteArchive_Add</c>).
        /// 2. Supprimer l'enregistrement de ChuteMagasin (<c>IS_ChuteMagasin_Delete</c>).
        /// 3. Mettre à Jour l'enregistrement DecoupeBarre (<c>IS_DecoupeBarre_UpdateAppro</c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, int decoupeBarreId, int chuteMagasinId, int quantite, string caller);
    }
}