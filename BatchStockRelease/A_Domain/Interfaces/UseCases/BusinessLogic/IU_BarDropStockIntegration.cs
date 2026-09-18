using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.BusinessLogic
{
    /// <summary>
    /// Interface d'un UseCase permettant de valider l'intégration d'une chute générée en stock
    /// en mettant à jour l’état <c>AttenteIntegration</c> à <c>false</c>.
    /// </summary>
    public interface IU_BarDropStockIntegration
    {
        /// <summary>
        /// Exécute le processus d'intégration d'une chute à partir de son QR code.
        /// </summary>
        /// <param name="QrCodeInput">Code-barres (QR) de la chute.</param>
        /// <param name="caller">Call Chain appelant (pour traçabilité).</param>
        /// <remarks>
        /// <para><b>Étapes métier :</b></para>
        /// 1. Récuprérer l'enregistrement de la table (<c> ChutesMagasin </c>)
        /// 2. Tester si chuteMagasin est null.
        /// 3. Tester si chuteMagasin.AttenteIntegration = false.
        /// 4. Mettre à jour chuteMagasin.AttenteIntegration = false.
        /// 
        /// <para><b>Exceptions possibles :</b></para>
        /// Utilise le service <c>IS_LogAndNotify</c> pour journaliser et notifier les erreurs éventuelles.
        /// - <see cref="Ex_Business"/> : levée si une règle métier est violée.
        /// - <see cref="Ex_Infrastructure"/> : levée si une erreur technique survient.
        /// - <see cref="Exception"/> : levée si une autre erreur survient.
        /// </remarks>        
        /// <returns>Une tâche représentant un ensemble d'opérations asynchrones</returns>
        Task<int> ExecuteAsync(string QrCodeInput, string caller);
    }
}