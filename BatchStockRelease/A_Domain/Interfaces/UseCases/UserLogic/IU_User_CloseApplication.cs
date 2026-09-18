using System.ComponentModel;
using BatchStockRelease.A_Domain.Common.Enums;

namespace BatchStockRelease.A_Domain.Interfaces.UseCases.UserLogic
{
    /// <summary>
    /// Interface définissant le contrat du UseCase de fermeture contrôlée de l’application.
    /// </summary>
    public interface IU_User_CloseApplication
    {
        /// <summary>
        /// Exécute la procédure complète de fermeture de l’application.
        /// </summary>
        /// <param name="e">Arguments de l’événement <c>Closing</c>.</param>
        /// <param name="caller">Nom du composant appelant.</param>
        /// <returns>
        /// Retourne un <see cref="BatchStockRelease.A_Domain.Common.Enums.En_CloseResult"/> indiquant l’action à effectuer par la couche UI.
        /// </returns>
        Task<En_CloseResult> ExecuteAsync(CancelEventArgs e, string caller);
    }
}