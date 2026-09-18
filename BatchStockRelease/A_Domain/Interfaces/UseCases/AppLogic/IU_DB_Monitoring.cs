
namespace BatchStockRelease.A_Domain.Interfaces.UseCases.AppLogic
{
    /// <summary>
    /// <b>UseCase : Démarrage de la surveillance de la base de données.</b>
    /// <para>
    /// Orchestration centralisée du lancement du service de monitoring (SR_ConnectionMonitor),
    /// incluant la gestion des exceptions et la traçabilité.
    /// </para>
    /// </summary>
    public interface IU_DB_Monitoring
    {
        /// <summary>
        /// Lance la surveillance de la base de données.
        /// </summary>
        Task ExecuteAsync(string caller);

    }
}
