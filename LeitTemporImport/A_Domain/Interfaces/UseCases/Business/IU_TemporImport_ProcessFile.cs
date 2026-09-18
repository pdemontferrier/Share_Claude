using System.Threading;
using System.Threading.Tasks;

namespace LeitTemporImport.A_Domain.Interfaces.UseCases.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du UseCase chargé de traiter un fichier MDB unique dans le cadre du pipeline
    /// d’import MDB → SQL Server du projet 104.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Appelé par un UseCase orchestrateur supérieur (ex : UC_TemporImport) qui itère sur les fichiers
    /// présents dans le répertoire cible et délègue le traitement d’un fichier à ce UseCase.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Standardiser le traitement d’un fichier MDB : lecture du numéro de série (SerieNr),
    /// mise à jour du contexte métier et décisions associées (ex : suppression si déjà importé).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases et orchestrateurs batch de l’application console.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Traiter un fichier MDB identifié par son chemin.</description></item>
    /// <item><description>Garantir la traçabilité via la CallChain.</description></item>
    /// <item><description>Supporter l’annulation via <see cref="CancellationToken"/>.</description></item>
    /// </list>
    /// </summary>
    public interface IU_TemporImport_ProcessFile
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Exécute le traitement d’un fichier MDB.</para>
        /// <para>Contexte</para>
        /// <para>Appelé depuis un UseCase orchestrateur qui fournit la CallChain amont.</para>
        /// <para>Objectif</para>
        /// <para>Lire/valider les données nécessaires au workflow et déclencher les actions associées.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider le chemin du fichier.</description></item>
        /// <item><description>Effectuer le traitement métier associé au fichier.</description></item>
        /// <item><description>Propager la traçabilité (CallChain) à toutes les dépendances.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="filePath">Chemin complet du fichier MDB à traiter.</param>
        /// <param name="failedDir">Chemin complet du répertoire si non importé.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// </summary>
        Task ExecuteAsync(string caller, string filePath, string failedDir, CancellationToken ct = default);

        #endregion
    }
}
