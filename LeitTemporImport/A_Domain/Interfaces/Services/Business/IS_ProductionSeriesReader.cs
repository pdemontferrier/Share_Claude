using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Services.Business
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat de service de lecture (Reader) dédié à <see cref="ProductionSeries"/>.
    /// Fournit une API métier simple pour interroger l’état d’import d’une série, en s’appuyant sur CQRS (QueryHandlers).
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import MDB → SQL Server pour déterminer si une série a déjà été importée
    /// et adapter le traitement (ex : suppression du fichier si déjà importé).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Encapsuler l’accès aux QueryHandlers ProductionSeries afin de simplifier l’usage côté UseCase
    /// tout en conservant traçabilité (CallChain) et robustesse (reclassification d’exceptions).
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Lire la série par IdSerialNumber.</description></item>
    /// <item><description>Lire le statut IsImported par IdSerialNumber.</description></item>
    /// </list>
    /// </summary>
    public interface IS_ProductionSeriesReader
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la série de production correspondant à un numéro de série.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour charger l’état complet d’une série lorsque nécessaire.</para>
        /// <para>Objectif</para>
        /// <para>Exposer une lecture métier simple, basée sur CQRS.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="ProductionSeries"/> ou null.</returns>
        /// </summary>
        Task<ProductionSeries?> GetByIdSerialNumberAsync(string caller, int idSerialNumber, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut IsImported d’une série identifiée par IdSerialNumber.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par UC_TemporImport_ProcessFile pour décider si un fichier MDB doit être supprimé.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée du statut d’import, sans charger l’entité complète.</para>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        Task<bool?> GetIsImportedAsync(string caller, int idSerialNumber, CancellationToken ct = default);

        #endregion
    }
}
