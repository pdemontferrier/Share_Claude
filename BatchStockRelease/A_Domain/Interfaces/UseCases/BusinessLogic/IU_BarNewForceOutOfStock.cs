using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// UseCase orchestrateur permettant de forcer la sortie des barres neuves d’un lot de découpe en rupture de stock.
    /// Il met à jour l’état du lot, désactive les barres concernées si demandé, ajoute une ligne d’historique de modification de stock,
    /// et notifie l’utilisateur en cas d’erreur.
    /// </summary>
    public interface IU_BarNewForceOutOfStock
    {
        /// <summary>
        /// Exécute le processus complet de forçage de sortie des barres neuves pour un lot donné.
        /// Cette méthode met à jour l’état du lot de découpe, marque éventuellement les barres en rupture comme sorties de force,
        /// et rafraîchit la page en cours.
        /// </summary>
        /// <param name="decoupeLotId">Identifiant du lot à traiter.</param>
        /// <param name="forceDecoupeBarre">Indique si il faut aussi forcer le barres assciées au lot.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Mettre à jour la table (<c> DecoupeLot </c>)
        /// 2. Mettre à jour la table DecoupeBarre (<c> DecoupeBarre </c>).
        /// 3. Mettre à jour la documentation.
        /// 4. Raffraichir la page encours.
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task ExecuteAsync(int decoupeLotId, bool forceDecoupeBarre, string caller);
    }
}