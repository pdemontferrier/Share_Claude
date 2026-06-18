using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase responsable de l'ajout d'une action client sur toutes les commandes d'un lot,
    /// avec mise à jour du statut et enregistrement dans l'historique des modifications.
    /// </summary>
    public interface IU_CommandeClientAction
    {
        /// <summary>
        /// Exécute l’ajout d’une action client pour chaque commande du lot en cours.
        /// </summary>
        /// <param name="idAction">Identifiant de l’action client à ajouter.</param>
        /// <param name="updateStatut">Indiquer si il faut mettre à jour le statut.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Rechercher l'action demandée (<c> IQ_CommandeClientActionType </c>)
        /// 2. Extraire le statut lié à l'action (<c> ... </c>).
        /// 3. Rechercher toutes les CommanceClient lié au DecoupeLotId (<c> IQ_CommandeClient </c>).
        /// 4. Traiter chaque commande du lot (<c> ... </c>).
        /// 41. Ajouter un enregistrement à CommandeClientAction (<c> IS_CommandeClientAction_Add </c>).
        /// 42. Mettre à jour le statut de la commande (<c> IS_CommandeClient_UpdateStatut </c>).
        /// 43. Ajouter une ligne d'historique dans la table CommandeClientModification (<c> IS_CommandeClientModification_Add </c>).
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int idAction, bool updateStatut, string caller);
    }
}