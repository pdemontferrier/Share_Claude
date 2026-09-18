using BatchStockRelease.A_Domain.Common.Exceptions;

namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// Service d’infrastructure chargé de journaliser les erreurs et de notifier l’utilisateur.
    /// </summary>
    public interface IS_LogAndNotify
    {
        /// <summary>
        /// Enregistre une erreur dans les logs et affiche une notification à l’utilisateur.
        /// </summary>
        /// <param name="dictionaryKey">Clé du dictionnaire pour le message principal d’erreur.</param>
        /// <param name="ex">Exception levée dans le système.</param>
        /// <param name="notification">Indication relative à l'affichage d'un message d'erreur.</param>
        /// <remarks>
        /// Cette méthode gère les trois cas d’exception :
        /// <list type="bullet">
        /// <item><description><see cref="Ex_Business"/></description></item>
        /// <item><description><see cref="Ex_Infrastructure"/></description></item>
        /// <item><description><see cref="System.Exception"/> générique</description></item>
        /// </list>
        /// </remarks>
        Task ExecuteAsync(string dictionaryKey, Exception ex, bool notification = true);
    }
}
