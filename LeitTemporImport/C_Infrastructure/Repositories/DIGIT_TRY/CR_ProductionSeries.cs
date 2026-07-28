using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.DIGIT_TRY;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using LeitTemporImport.C_Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace LeitTemporImport.C_Infrastructure.Repositories.DIGIT_TRY
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Repository concret (CR) dédié à l’entité <see cref="ProductionSeries"/> pour la base DIGIT_TRY.
    /// Fournit des requêtes de lecture spécifiques liées aux séries de production.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les QueryHandlers (QH) ou Services Infrastructure nécessitant des accès optimisés
    /// à la table ProductionSeries (ex : vérifier si une série est déjà importée).
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser les accès EF Core spécifiques à ProductionSeries, avec traçabilité (CallChain)
    /// et reclassification des exceptions via <see cref="Ex_Classifier"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>Couche Infrastructure : QueryHandlers / Services / UseCases via interfaces.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Récupérer une série par IdSerialNumber.</description></item>
    /// <item><description>Lire le statut IsImported d’une série.</description></item>
    /// <item><description>Appliquer des filtres standards (IsDeleted = 0) et AsNoTracking en lecture.</description></item>
    /// </list>
    /// </summary>
    public class CR_ProductionSeries : CR_Generic<ProductionSeries>, IR_ProductionSeries
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        // Héritées via CR_Generic : _contextFactory

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le repository concret pour ProductionSeries.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>Permettre l’accès EF Core à ProductionSeries via un DbContextFactory.</para>
        /// <param name="contextFactory">Factory EF Core du contexte DIGIT_TRY.</param>
        /// </summary>
        public CR_ProductionSeries(
            IDbContextFactory<DigitTryDbContext> contextFactory,
            IQ_AppContext appContext)
            : base(
                  contextFactory ?? throw new ArgumentNullException(nameof(contextFactory)),
                  appContext ?? throw new ArgumentNullException(nameof(appContext)))
        {
            _callee = GetType().Name;
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne l’entité <see cref="ProductionSeries"/> correspondant à un numéro de série.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé lors des contrôles d’import (déjà importé ou non) ou pour charger la série cible.</para>
        /// <para>Objectif</para>
        /// <para>Fournir une lecture optimisée (AsNoTracking) avec filtre IsDeleted.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Créer un DbContext.</description></item>
        /// <item><description>Interroger ProductionSeries par IdSerialNumber.</description></item>
        /// <item><description>Retourner null si non trouvé.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Entité ProductionSeries ou null.</returns>
        /// </summary>
        public async Task<ProductionSeries?> GetByIdSerialNumberAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByIdSerialNumberAsync)}";

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                return await context.ProductionSeries
                    .FirstOrDefaultAsync(ps =>
                        ps.IdSerialNumber == idSerialNumber &&
                        ps.IsDeleted == false, ct);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le statut d’import (IsImported) pour un numéro de série donné.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par UC_TemporImport_ProcessFile pour déterminer si le fichier MDB doit être supprimé.</para>
        /// <para>Objectif</para>
        /// <para>Optimiser la lecture en ne sélectionnant que le champ IsImported.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Créer un DbContext.</description></item>
        /// <item><description>Interroger ProductionSeries filtré (IsDeleted = 0).</description></item>
        /// <item><description>Retourner null si aucune série active n’existe.</description></item>
        /// </list>
        /// <param name="caller">CallChain amont.</param>
        /// <param name="idSerialNumber">Identifiant métier du numéro de série.</param>
        /// <param name="ct">Token d’annulation.</param>
        /// <returns>Bool? : true/false si trouvé, null sinon.</returns>
        /// </summary>
        public async Task<bool?> GetIsImportedAsync(
            string caller,
            int idSerialNumber,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetIsImportedAsync)}";

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync(ct);

                return await context.ProductionSeries
                    .AsNoTracking()
                    .Where(ps => ps.IdSerialNumber == idSerialNumber && ps.IsDeleted == false)
                    .Select(ps => (bool?)ps.IsImported)
                    .FirstOrDefaultAsync(ct);
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
