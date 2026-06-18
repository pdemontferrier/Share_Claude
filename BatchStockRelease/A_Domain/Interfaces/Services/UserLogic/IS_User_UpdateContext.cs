
namespace BatchStockRelease.A_Domain.Interfaces.Services.UserLogic
{
    /// <summary>
    /// <b>Description :</b>
    /// <para>
    /// Interface du service métier responsable de la mise à jour du contexte utilisateur
    /// après l’authentification ou la validation des droits d’accès.
    /// </para>
    ///
    /// <b>Contexte :</b>
    /// <para>
    /// Utilisée par les UseCases d’authentification ou de contrôle d’accès pour
    /// synchroniser les informations locales (identifiant, nom complet, autorisation)
    /// avec les données issues de la base de données.
    /// </para>
    ///
    /// <b>Objectif :</b>
    /// <para>
    /// Maintenir à jour le contexte utilisateur de l’application à chaque changement
    /// d’état d’authentification.
    /// </para>
    ///
    /// <b>Tâches / Actions :</b>
    /// <list type="bullet">
    /// <item><description>Récupérer le nom complet de l’utilisateur à partir de son identifiant.</description></item>
    /// <item><description>Mettre à jour les propriétés <c>AppUserID</c>, <c>AppUserFullName</c> et <c>CanUserAccessApp</c>.</description></item>
    /// </list>
    /// </summary>
    public interface IS_User_UpdateContext
    {
        /// <summary>
        /// Met à jour le contexte utilisateur en mémoire avec les informations actuelles.
        /// </summary>
        /// <param name="userId">Identifiant de l’utilisateur.</param>
        /// <param name="access">Statut d’accès (autorisé ou non).</param>
        /// <param name="caller">Chaîne d’appel pour la traçabilité (<c>CallChain</c>).</param>
        Task ExecuteAsync(int userId, bool access, string caller);
    }
}