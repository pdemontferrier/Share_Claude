using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Services.Infrastructure;

namespace LeitTemporImport.C_Infrastructure.Services
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Service d’infrastructure dédié à la suppression sécurisée de fichiers.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé lors du traitement des fichiers MDB dans le pipeline d’import.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Encapsuler la suppression de fichiers avec gestion centralisée des erreurs
    /// et respect des standards projet 104 (CallChain, classification).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases applicatifs.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Vérifier la validité du chemin.</description></item>
    /// <item><description>Vérifier l’existence du fichier.</description></item>
    /// <item><description>Supprimer le fichier.</description></item>
    /// <item><description>Classifier toute exception via <c>Ex_Classifier</c>.</description></item>
    /// </list>
    /// </summary>
    public class SR_FileDelete : IS_FileDelete
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Aucune dépendance

        #endregion

        #region === Constructeur ===

        public SR_FileDelete()
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime un fichier du système de fichiers.</para>
        /// <param name="caller">Chaîne de traçabilité amont.</param>
        /// <param name="filePath">Chemin complet du fichier à supprimer.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        public async Task ExecuteAsync(string caller, string filePath, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(ExecuteAsync)}";

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("File path must be provided.", nameof(filePath));

                if (!File.Exists(filePath))
                    return;

                // I/O sync → encapsulé dans Task pour cohérence async pipeline
                await Task.Run(() => File.Delete(filePath), ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}
