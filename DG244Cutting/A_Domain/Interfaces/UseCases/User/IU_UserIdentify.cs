namespace DG244Cutting.A_Domain.Interfaces.UseCases.User
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// UseCase d’identification et de contrôle d’ouverture de l’application console.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Exécuté au démarrage (Program) avant les imports.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir que l’utilisateur du poste est reconnu. En cas d’échec, journaliser et
    /// retourner <c>0</c> afin que l’application interrompe proprement le flux.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Application console d’import (projet 104).</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire le login Windows via <c>IS_Settings_User</c>.</description></item>
    /// <item><description>Vérifier l’utilisateur via <c>IS_User_CheckAppDeviceUser</c>.</description></item>
    /// <item><description>Journaliser une erreur métier si l’utilisateur n’est pas reconnu.</description></item>
    /// <item><description>Retourner l’identifiant utilisateur ou <c>0</c>.</description></item>
    /// </list>
    /// </summary>
    public interface IU_UserIdentify
    {
        /// <summary>
        /// <para>Description : Exécute l’identification de l’utilisateur.</para>
        /// <para>Objectif : Retourner l’Id utilisateur si identifié, sinon <c>0</c>.</para>
        /// </summary>
        /// <param name="caller">Chaîne d’appel pour la traçabilité.</param>
        /// <returns>Id utilisateur si identifié, sinon 0.</returns>
        Task<int> ExecuteAsync(string caller, CancellationToken ct = default);
    }
}
