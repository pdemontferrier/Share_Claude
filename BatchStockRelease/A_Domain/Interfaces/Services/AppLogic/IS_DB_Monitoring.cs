
namespace BatchStockRelease.C_Infrastructure.Services
{
    /// <summary>
    /// <b>Interface du service de monitoring de la connexion base de données.</b>
    /// <para>
    /// Fournit la méthode de lancement du processus de surveillance asynchrone
    /// de la base de données <c>geststock</c>. Ce service détecte les pertes
    /// et rétablissements de connexion, en mettant à jour les états applicatifs
    /// via <see cref="BatchStockRelease.B_UseCases.Settings.AppLogic.SE_App"/>.
    /// </para>
    /// </summary>
    public interface IS_DB_Monitoring
    {
        /// <summary>
        /// Démarre la boucle de monitoring asynchrone de la connexion base de données.
        /// </summary>
        /// <param name="caller">Nom du composant appelant pour la traçabilité.</param>
        Task ExecuteAsync(string caller);
    }
}