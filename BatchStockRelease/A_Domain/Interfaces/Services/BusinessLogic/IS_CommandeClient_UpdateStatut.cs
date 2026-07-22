using BatchStockRelease.A_Domain.Common.Exceptions;
using BatchStockRelease.A_Domain.Entities.GestStock;

namespace BatchStockRelease.A_Domain.Interfaces.Services.BusinessLogic
{
    /// <summary>
    /// Interface du service métier chargé de mettre à jour le statut d’une commande client.
    /// </summary>
    public interface IS_CommandeClient_UpdateStatut
    {
        /// <summary>Met à jour le champ <c>Statut</c> d’une commande client avec un nouvel identifiant de statut, 
        /// et enregistre la modification via le handler de persistance.</summary>
        /// <param name="commande">
        /// Instance de <see cref="CommandeClient"/> à mettre à jour. 
        /// L'objet doit être chargé en mémoire et prêt à être modifié.
        /// </param>
        /// <param name="statutId">
        /// Nouvel identifiant du statut à affecter à la commande. 
        /// Il doit correspondre à un identifiant valide existant dans la table des statuts.
        /// </param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Exceptions possibles :</b></para>
        /// Les erreurs sont automatiquement classifiées par <c>Ex_Classifier</c>.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(CommandeClient commande, int statutId, string caller);
    }
}