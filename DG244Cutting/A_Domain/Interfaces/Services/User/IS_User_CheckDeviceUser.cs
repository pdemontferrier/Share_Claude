
namespace DG244Cutting.A_Domain.Interfaces.Settings.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service métier chargé de vérifier si un nom d’utilisateur Windows correspond à un utilisateur
    /// applicatif connu (table <c>UserApp</c> / <c>User</c> selon ton modèle) et de retourner son identifiant.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé au démarrage de l’application console afin d’identifier l’utilisateur courant avant
    /// d’autoriser le lancement des traitements d’import.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Identifier l’utilisateur à partir du login Windows et initialiser le contexte applicatif
    /// (IdUser) via <c>IS_Settings_User</c>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application (ex : <c>UC_UserIdentify</c>).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider le paramètre <c>deviceUser</c>.</description></item>
    /// <item><description>Rechercher l’utilisateur en base via <c>IQ_UserApp</c>.</description></item>
    /// <item><description>Initialiser <c>IS_Settings_User</c> avec l’IdUser si trouvé.</description></item>
    /// <item><description>Retourner l’IdUser, sinon 0 si non trouvé.</description></item>
    /// </list>
    /// </summary>
    public interface IS_User_CheckDeviceUser
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>
        /// Vérifie si le nom d’utilisateur Windows fourni correspond à un utilisateur enregistré.
        /// Si oui, retourne son identifiant et initialise le contexte utilisateur.
        /// </para>
        /// <para>Contexte</para>
        /// <para>Appelé au démarrage par le UseCase <c>UC_UserIdentify</c>.</para>
        /// <para>Objectif</para>
        /// <para>Retourner l’IdUser si reconnu, sinon 0.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le paramètre.</description></item>
        /// <item><description>Lire l’utilisateur en base via QueryHandler.</description></item>
        /// <item><description>Mettre à jour <c>IS_Settings_User</c> si trouvé.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="deviceUser">Login Windows (poste) à vérifier.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>IdUser si trouvé, sinon 0.</returns>
        /// <exception cref="Exception">Reclassifiée via <c>Ex_Classifier</c>.</exception>
        /// </summary>
        Task<int> ExecuteAsync(string caller, string deviceUser, CancellationToken ct = default);
    }
}
