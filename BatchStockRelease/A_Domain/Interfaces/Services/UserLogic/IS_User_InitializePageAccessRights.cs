using System.Threading.Tasks;

namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Interface du service chargé d’initialiser les droits d’accès aux pages
    /// pour l’utilisateur courant de l’application.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Appelée après l’authentification ou la mise à jour du contexte utilisateur,
    /// cette initialisation garantit la cohérence entre les autorisations en base
    /// et celles chargées en mémoire.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Préparer les droits d’accès utilisateur à toutes les pages de l’application
    /// avant affichage de l’interface.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Initialiser les accès par défaut de toutes les pages à <c>false</c>.</description></item>
    /// <item><description>Charger les droits enregistrés dans la base via <see cref="BatchStockRelease.A_Domain.Interfaces.Handlers.Queries.IQ_UserAppPageDroit"/>.</description></item>
    /// <item><description>Mettre à jour les autorisations dans <see cref="IS_Settings_User"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IS_User_InitializePageAccessRights
    {
        /// <summary>
        /// Initialise les droits d’accès de l’utilisateur courant sur les pages applicatives.
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur à vérifier.</param>
        /// <param name="appId">Identifiant de l’application cible.</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        Task ExecuteAsync(int userId, int appId, string caller);
    }
}