using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Queries
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du QueryHandler (IQ) dédié à l’entité <see cref="ProductionSeries"/> dans le cadre du modèle CQRS.
    /// Définit les requêtes de lecture spécifiques nécessaires aux UseCases d’import et de synchronisation.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases (ex : UC_TemporImport_ProcessFile) afin de consulter l’état d’une série
    /// (existence, statut IsImported) sans accéder directement au repository ni au DbContext.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir des points d’entrée de requêtes traçables via CallChain et homogènes avec les conventions projet 104.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer une série par IdSerialNumber.</description></item>
    /// <item><description>Récupérer le statut IsImported d’une série.</description></item>
    /// </list>
    /// </summary>
    public interface IQ_ProductionSeries
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la série de production correspondant à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors des contrôles d’existence et des validations avant import.</para>
        /// <para>Objectif</para>
        /// <para>Permettre la lecture de l’entité <see cref="ProductionSeries"/> à partir de l’identifiant IdSerialNumber.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Interroger la table ProductionSeries via un QueryHandler.</description></item>
        /// <item><description>Retourner null si aucune série active n’est trouvée.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="ProductionSeries"/> ou null.</returns>
        /// </summary>
        Task<ProductionSeries?> HandleGetByIdSerialNumberAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut d’import (IsImported) associé à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour déterminer si une série a déjà été importée afin d’éviter un retraitement.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée du statut IsImported sans charger l’entité complète.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Interroger ProductionSeries par IdSerialNumber.</description></item>
        /// <item><description>Retourner null si aucune série active n’est trouvée.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        Task<bool?> HandleGetIsImportedAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default);

        #endregion
    }
}
