using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// Interface du UseCase chargé de déterminer la page suivante à afficher à partir de la Page20,
    /// en fonction de l’état d’approvisionnement du lot de découpe sélectionné.
    /// </summary>
    public interface IU_Page20Dispatch
    {
        /// <summary>
        /// Détermine la navigation métier à partir de l’état d’un lot :
        /// approvisionnement par chutes, allocation, rupture ou sortie de stock.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot de découpe sélectionné</param>
        /// <param name="caller">Initiateur du lancement du UseCase.</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Enregistrer le chariot sélectionné.
        /// 2. Vérifier l’état d’approvisionnement.
        /// 3. Lancer allocation si des barres sont en attente.
        /// 4. Vérifier les barres à sortir.
        /// 5. Vérifier les ruptures.
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>
        /// Nom de la page vers laquelle rediriger l’utilisateur, selon l’état métier :
        /// <list type="bullet">
        /// <item><term>Page31</term> : si l’approvisionnement par chutes est à réaliser</item>
        /// <item><term>Page32</term> : si des barres neuves doivent être sorties</item>
        /// <item><term>Page30</term> : si des ruptures de stock sont présentes</item>
        /// <item><term>Page10</term> : si aucune action n’est possible ou si les prérequis ne sont pas remplis</item>
        /// </list>
        /// </returns>
        Task ExecuteAsync(int decoupeLotId, string caller);
    }
}