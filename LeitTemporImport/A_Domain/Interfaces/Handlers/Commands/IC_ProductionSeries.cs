using LeitTemporImport.A_Domain.Interfaces.Handlers.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Commands
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Contrat du CommandHandler (IC) dédié à la mise à jour d’informations de la table ProductionSeries.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les UseCases d’import pour marquer une série comme importée, à partir de son identifiant IdSerialNumber.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Fournir une commande explicite et traçable permettant de tagger <c>IsImported</c> à <c>true</c>
    /// sans charger l’entité complète.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>UseCases de la couche Application.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Valider l’identifiant de série.</description></item>
    /// <item><description>Mettre à jour IsImported à true en base.</description></item>
    /// </list>
    /// </summary>
    public interface IC_ProductionSeries : IC_Generic<ProductionSeries>
    {
        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour la série de production pour positionner <c>IsImported</c> à <c>true</c>.</para>
        /// <para>Contexte</para>
        /// <para>Appelée à la fin d’un import valide afin d’empêcher un retraitement.</para>
        /// <para>Objectif</para>
        /// <para>Réaliser une mise à jour ciblée par <c>IdSerialNumber</c>, sans recharger l’entité.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider <paramref name="idSerialNumber"/>.</description></item>
        /// <item><description>Exécuter l’update en base en filtrant les lignes soft-deleted.</description></item>
        /// </list>
        /// </summary>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        Task HandleSetIsImportedTrueAsync(string caller, int idSerialNumber, CancellationToken ct = default);

        #endregion
    }
}
