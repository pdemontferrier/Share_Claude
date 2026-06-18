
namespace BatchStockRelease.A_Domain.Interfaces.Services.AppLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Service métier responsable de la mise à jour du contexte applicatif et utilisateur
    /// au moment du démarrage de l’application.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Ce service est appelé depuis le <see cref="BatchStockRelease.B_UseCases.UseCases.AppLogic.UC_Application_OnStart"/> après la
    /// validation des préconditions techniques (connectivité, session, disponibilité applicative).
    /// Il agit comme un point d’initialisation visuel et contextuel, garantissant que
    /// l’interface et les informations utilisateur sont correctement affichées dès l’ouverture
    /// de l’application.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Mettre à jour le titre de l’application et le nom complet de l’utilisateur connecté
    /// afin d’assurer la cohérence entre le contexte système et l’affichage utilisateur.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Définir le titre principal de l’application à partir du dictionnaire multilingue.</description></item>
    /// <item><description>Vérifier si un utilisateur est identifié dans le contexte courant.</description></item>
    /// <item><description>Récupérer le nom complet de l’utilisateur à partir du service de requêtes <see cref="BatchStockRelease.A_Domain.Interfaces.Handlers.Queries.IQ_User"/>.</description></item>
    /// <item><description>Mettre à jour le paramètre <see cref="BatchStockRelease.A_Domain.Interfaces.Services.UserLogic.IS_Settings_User"/> avec le nom complet.</description></item>
    /// </list>
    /// </summary>
    public interface IS_StartupContextUpdater
    {
        /// <summary>
        /// <b>Description :</b>
        /// <para>
        /// Met à jour le titre de l’application et, si un utilisateur est connecté,
        /// recharge son nom complet depuis la base de données.
        /// </para>
        ///
        /// <b>Contexte :</b>
        /// <para>
        /// Cette méthode est appelée à la fin du démarrage de l’application, après la
        /// vérification de l’environnement. Elle garantit la synchronisation entre
        /// les informations de configuration et l’affichage utilisateur.
        /// </para>
        ///
        /// <b>Objectif :</b>
        /// <para>
        /// Rafraîchir le titre et le contexte utilisateur pour garantir que les
        /// informations affichées à l’écran reflètent fidèlement l’état du système.
        /// </para>
        ///
        /// <b>Tâches / Actions :</b>
        /// <list type="bullet">
        /// <item><description>Appliquer le titre principal défini dans le dictionnaire.</description></item>
        /// <item><description>Vérifier la présence d’un identifiant utilisateur.</description></item>
        /// <item><description>Recharger le nom complet depuis la table <c>User</c>.</description></item>
        /// <item><description>Mettre à jour la propriété <see cref="BatchStockRelease.A_Domain.Interfaces.Services.UserLogic.IS_Settings_User.SetAppUserFullName"/>.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">Chaîne d’appel utilisée pour la traçabilité (CallChain).</param>
        Task ExecuteAsync(string caller);
    }
}
