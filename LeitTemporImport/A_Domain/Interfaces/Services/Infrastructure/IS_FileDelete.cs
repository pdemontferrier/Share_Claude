
namespace LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de service dédié à la suppression sécurisée de fichiers du système.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé dans le cadre du traitement des fichiers MDB après import,
    /// notamment lorsque la série correspondante est déjà marquée comme importée.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser la suppression de fichiers afin d’assurer traçabilité,
    /// gestion d’erreurs et conformité aux standards 104.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Vérifier l’existence du fichier.</description></item>
    /// <item><description>Supprimer le fichier si présent.</description></item>
    /// <item><description>Classifier les exceptions éventuelles.</description></item>
    /// </list>
    /// </summary>
    public interface IS_FileDelete
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime un fichier du système de fichiers.</para>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="filePath">Chemin complet du fichier à supprimer.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteAsync(string caller, string filePath, CancellationToken ct = default);

        #endregion
    }
}
