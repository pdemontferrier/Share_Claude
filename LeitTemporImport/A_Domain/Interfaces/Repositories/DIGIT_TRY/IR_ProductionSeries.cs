using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du repository (IR) dédié à l’entité <see cref="ProductionSeries"/> pour la base DIGIT_TRY.
    /// Définit les opérations de lecture spécifiques nécessaires aux traitements d’import et de synchronisation.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par la couche Infrastructure (QueryHandlers, Services) afin d’accéder aux données de
    /// ProductionSeries via EF Core sans exposer le DbContext aux couches supérieures.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les accès spécifiques à ProductionSeries (ex : lecture du statut IsImported)
    /// et garantir la traçabilité via la CallChain dans toutes les méthodes.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>QueryHandlers (QH) et Services Infrastructure du projet 104.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer une série par IdSerialNumber.</description></item>
    /// <item><description>Lire le statut IsImported d’une série.</description></item>
    /// </list>
    /// </summary>
    public interface IR_ProductionSeries : IR_Generic<ProductionSeries>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne la série de production correspondant à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors des contrôles de présence et des validations avant import.</para>
        /// <para>Objectif</para>
        /// <para>Permettre une lecture de l’entité ProductionSeries à partir de l’identifiant métier IdSerialNumber.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Interroger ProductionSeries par IdSerialNumber.</description></item>
        /// <item><description>Retourner null si la série n’existe pas (ou est supprimée selon filtre Infrastructure).</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité <see cref="ProductionSeries"/> ou null.</returns>
        /// </summary>
        Task<ProductionSeries?> GetByIdSerialNumberAsync( string caller, int idSerialNumber, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut d’import (IsImported) associé à un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé pour déterminer si une série a déjà été importée, afin d’éviter un retraitement.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Fournir une lecture optimisée du seul champ IsImported, sans charger l’entité complète.
        /// </para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Interroger ProductionSeries par IdSerialNumber.</description></item>
        /// <item><description>Retourner null si la série n’existe pas (ou est supprimée selon filtre Infrastructure).</description></item>
        /// </list>
        /// <param name="caller">CallChain amont pour la traçabilité.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        Task<bool?> GetIsImportedAsync( string caller, int idSerialNumber, CancellationToken ct = default);

        #endregion
    }
}
