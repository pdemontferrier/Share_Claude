using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase responsable de la mise à jour du statut de toutes les commandes d'un lot,
    /// et enregistrement dans l'historique des modifications.
    /// </summary>
    public interface IU_CommandeClientStatut
    {
        /// <summary>
        /// Exécute la mise à jour du statut pour chaque commande du lot en cours.
        /// </summary>
        /// <param name="idStatut">Identifiant du statut à mettre à jour.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Rechercher toutes les CommanceClient lié au DecoupeLotId (<c> IQ_CommandeClient </c>).
        /// 2. Traiter chaque commande du lot (<c> ... </c>).
        /// 21. Mettre à jour le statut de la commande (<c> IS_CommandeClient_UpdateStatut </c>).
        /// 22. Ajouter une ligne d'historique dans la table CommandeClientModification (<c> IS_CommandeClientModification_Add </c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int idStatut, string caller);
    }
}