using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.DTOs.App;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Queries;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Context;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace LeitTemporImport.C_Infrastructure.Repositories.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// Repository Infrastructure générique fournissant les opérations CRUD standard
    /// pour toute entité <typeparamref name="T"/> via Entity Framework Core.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé par les CommandHandlers et QueryHandlers génériques/spécifiques afin de centraliser
    /// l’accès aux tables MS SQL Server (projet 104). Chaque méthode instancie son propre DbContext
    /// via <see cref="IDbContextFactory{TContext}"/>, ce qui rend les appels isolés et adaptés
    /// aux traitements console/batch.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Garantir un accès robuste, thread-safe et sans état global, en encapsulant :
    /// </para>
    /// <list type="bullet">
    /// <item><description>La création contrôlée des DbContext.</description></item>
    /// <item><description>Les opérations CRUD standard.</description></item>
    /// <item><description>La traçabilité via <c>callChain</c> et la reclassification via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// <para>Utilisateurs cibles</para>
    /// <para>Handlers / Services de la couche UseCases et Infrastructure.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Créer un DbContext à la demande.</description></item>
    /// <item><description>Exécuter l’opération EF Core correspondante.</description></item>
    /// <item><description>Persister via <c>SaveChangesAsync</c> pour les écritures.</description></item>
    /// <item><description>Reclassifier toute exception via <see cref="Ex_Classifier"/>.</description></item>
    /// </list>
    /// <typeparam name="T">Type d’entité EF Core (table) manipulé par le repository.</typeparam>
    /// </summary>
    public class CR_Generic<T> : IR_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        private readonly string _callee;

        #endregion

        #region === Dépendances privées ===

        protected readonly IDbContextFactory<DigitTryDbContext> _contextFactory;
        private readonly IQ_AppContext _appContext;

        #endregion

        #region === Constructeur ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Construit le repository générique <see cref="CR_Generic{T}"/>.</para>
        /// <para>Contexte</para>
        /// <para>Instancié via DI dans la couche Infrastructure.</para>
        /// <para>Objectif</para>
        /// <para>Initialiser l’accès au contexte EF Core via une factory de DbContext.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Valider la dépendance <paramref name="contextFactory"/>.</description></item>
        /// <item><description>Initialiser <c>_callee</c>.</description></item>
        /// </list>
        /// <param name="contextFactory">Factory EF Core de création de <see cref="DigitTryDbContext"/>.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="contextFactory"/> est null.</exception>
        /// </summary>
        public CR_Generic(
            IDbContextFactory<DigitTryDbContext> contextFactory, 
            IQ_AppContext appContext)
        {
            _callee = GetType().Name;
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
        }

        #endregion

        #region === Méthodes publiques ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute une entité <typeparamref name="T"/> en base et persiste immédiatement.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par les CommandHandlers lors des insertions.</para>
        /// <para>Objectif</para>
        /// <para>Insérer la ligne correspondante et exécuter <c>SaveChangesAsync</c>.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Créer un DbContext.</description></item>
        /// <item><description>Ajouter l’entité via <c>DbSet.AddAsync</c>.</description></item>
        /// <item><description>Persister via <c>SaveChangesAsync</c>.</description></item>
        /// </list>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity">Entité à ajouter.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// </summary>
        public async Task AddAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(AddAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                ArgumentNullException.ThrowIfNull(entity);

                // 1) Création d'un DbContext propre à cette opération (thread-safe via IDbContextFactory).
                await using var context = await _contextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                // 2) Stratégie d’exécution (supporte EnableRetryOnFailure / SqlServerRetryingExecutionStrategy).
                var strategy = context.Database.CreateExecutionStrategy();

                // 3) Toute transaction explicite doit être exécutée dans ExecuteAsync().
                await strategy.ExecuteAsync(async () =>
                {
                    // 3.1) Transaction explicite : garantit l’atomicité "métier + event store".
                    await using var tx = await context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

                    try
                    {
                        // 3.2) Ajout de l'entité : EF Core va générer un INSERT lors du SaveChanges().
                        await context.Set<T>().AddAsync(entity, ct).ConfigureAwait(false);

                        // 3.3) Écriture en base de l’UPDATE.
                        // On valide d’abord l’écriture métier avant de journaliser (traçabilité fiable).
                        await context.SaveChangesAsync(ct).ConfigureAwait(false);

                        // 3.4) Journalisation EventStore (optionnelle).
                        if (logEventStore)
                        {
                            // On construit le record après la réussite de l’INSERT.
                            // Id attendu > 0 (sinon Ex_Business via BuildEventStoreRecord).
                            var eventStoreRecord = BuildEventStoreRecord(callChain, entity);

                            context.Set<UserAppEventStore>().Add(eventStoreRecord);

                            // Persiste l’event store dans la même transaction.
                            await context.SaveChangesAsync(ct).ConfigureAwait(false);
                        }

                        // 3.5) Commit : valide définitivement la transaction en base.
                        // Sans Commit, la transaction serait annulée lors du Dispose().
                        await tx.CommitAsync(ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Rollback explicite pour garantir un état propre en cas d’exception.
                        await tx.RollbackAsync(ct).ConfigureAwait(false);
                        throw;
                    }

                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne une entité <typeparamref name="T"/> par son identifiant technique.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé par les handlers pour charger une entité avant suppression / modification.</para>
        /// <para>Objectif</para>
        /// <para>Effectuer un <c>FindAsync</c> sur la clé primaire.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique.</param>
        /// <returns>Entité trouvée ou null.</returns>
        /// </summary>
        public async Task<T?> GetByIdAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByIdAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                return await context.Set<T>().FindAsync(id).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne le premier enregistrement <typeparamref name="T"/> ou null.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Première entité ou null.</returns>
        /// </summary>
        public async Task<T?> GetFirstOrDefaultAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFirstOrDefaultAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                return await context.Set<T>().FirstOrDefaultAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Indique si un enregistrement <typeparamref name="T"/> existe pour un identifiant donné.</para>
        /// <param name="id">Identifiant technique.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns><c>true</c> si l’entité existe, sinon <c>false</c>.</returns>
        /// </summary>
        public async Task<bool> GetAnyAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAnyAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                var entity = await context.Set<T>().FindAsync(id).ConfigureAwait(false);
                return entity != null;
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne tous les enregistrements de <typeparamref name="T"/>.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Liste complète.</returns>
        /// </summary>
        public async Task<List<T>> GetAllAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAllAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                return await context.Set<T>().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne tous les enregistrements de <typeparamref name="T"/> en lecture seule (<c>AsNoTracking</c>).</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <returns>Liste complète sans tracking EF.</returns>
        /// </summary>
        public async Task<List<T>> GetAllAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAllAsNoTrackingAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                return await context.Set<T>().AsNoTracking().ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Retourne les enregistrements de <typeparamref name="T"/> correspondant à un prédicat.</para>
        /// <param name="predicate">Expression de filtrage.</param>
        /// <returns>Liste filtrée.</returns>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="predicate"/> est null.</exception>
        /// </summary>
        public async Task<List<T>> GetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFilteredAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                ArgumentNullException.ThrowIfNull(predicate);

                await using var context = await _contextFactory.CreateDbContextAsync().ConfigureAwait(false);
                return await context.Set<T>().Where(predicate).ToListAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour une entité <typeparamref name="T"/> et persiste immédiatement.</para>
        /// <param name="entity">Entité à mettre à jour.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// </summary>
        public async Task UpdateAsync(string caller, T entity, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(UpdateAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                ArgumentNullException.ThrowIfNull(entity);

                // 1) Création d'un DbContext propre à cette opération (thread-safe via IDbContextFactory).
                await using var context = await _contextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                // 2) Stratégie d’exécution (supporte EnableRetryOnFailure / SqlServerRetryingExecutionStrategy).
                var strategy = context.Database.CreateExecutionStrategy();

                // 3) Toute transaction explicite doit être exécutée dans ExecuteAsync().
                await strategy.ExecuteAsync(async () =>
                {
                    // 3.1) Transaction explicite : garantit l’atomicité "métier + event store".
                    await using var tx = await context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

                    try
                    {
                        // 3.2) Mise à jour de l’entité métier (Update = toutes propriétés scalaires marquées Modified).
                        context.Set<T>().Update(entity);

                        // 3.3) Écriture en base de l’UPDATE.
                        // On valide d’abord l’écriture métier avant de journaliser (traçabilité fiable).
                        await context.SaveChangesAsync(ct).ConfigureAwait(false);

                        // 3.4) Journalisation EventStore (optionnelle).
                        if (logEventStore)
                        {
                            // On construit le record après la réussite de l’UPDATE.
                            // Id attendu > 0 (sinon Ex_Business via BuildEventStoreRecord).
                            var eventStoreRecord = BuildEventStoreRecord(callChain, entity);

                            context.Set<UserAppEventStore>().Add(eventStoreRecord);

                            // Persiste l’event store dans la même transaction.
                            await context.SaveChangesAsync(ct).ConfigureAwait(false);
                        }

                        // 3.5) Commit : valide définitivement la transaction en base.
                        // Sans Commit, la transaction serait annulée lors du Dispose().
                        await tx.CommitAsync(ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Rollback explicite pour garantir un état propre en cas d’exception.
                        await tx.RollbackAsync(ct).ConfigureAwait(false);
                        throw;
                    }

                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un ensemble d’entités <typeparamref name="T"/> et persiste immédiatement.</para>
        /// <param name="entities">Ensemble d’entités à mettre à jour.</param>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entities"/> est null.</exception>
        /// </summary>
        public async Task UpdateRangeAsync(string caller, IEnumerable<T> entities, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(UpdateRangeAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();
                ArgumentNullException.ThrowIfNull(entities);

                // Important : on matérialise la séquence une seule fois, pour éviter plusieurs itérations
                // (et pour garantir que la liste ne change pas pendant l'opération).
                var entityList = entities as IList<T> ?? entities.ToList();

                // Si rien à faire, on sort proprement (choix : no-op).
                if (entityList.Count == 0)
                    return;

                // 1) DbContext dédié à l’opération.
                await using var context = await _contextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                // 2) Stratégie d’exécution (EnableRetryOnFailure).
                var strategy = context.Database.CreateExecutionStrategy();

                // 3) Transaction explicite à exécuter dans ExecuteAsync().
                await strategy.ExecuteAsync(async () =>
                {
                    await using var tx = await context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

                    try
                    {
                        // 3.1) Update en masse.
                        // Attention : UpdateRange marque toutes les propriétés scalaires comme Modified (comme Update()).
                        context.Set<T>().UpdateRange(entityList);

                        // 3.2) Persistance des updates.
                        await context.SaveChangesAsync(ct).ConfigureAwait(false);

                        // 3.3) Journalisation EventStore (optionnelle) : 1 record par entité.
                        if (logEventStore)
                        {
                            foreach (var entity in entityList)
                            {
                                // On construit le record après la réussite de l'UPDATERANGE.
                                // Id attendu > 0 (sinon Ex_Business via BuildEventStoreRecord).
                                var eventStoreRecord = BuildEventStoreRecord(callChain, entity);
                                context.Set<UserAppEventStore>().Add(eventStoreRecord);
                            }

                            // Persistance des records EventStore dans la même transaction.
                            await context.SaveChangesAsync(ct).ConfigureAwait(false);
                        }

                        // 3.4) Commit final.
                        await tx.CommitAsync(ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        await tx.RollbackAsync(ct).ConfigureAwait(false);
                        throw;
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime physiquement une entité <typeparamref name="T"/> par identifiant.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique.</param>
        /// </summary>
        public async Task DeleteAsync(string caller, int id, bool logEventStore = true, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(DeleteAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                if (id <= 0)
                    throw new ArgumentOutOfRangeException(nameof(id), "L'identifiant doit être strictement supérieur à 0.");

                // 1) Création d'un DbContext propre à cette opération (thread-safe via IDbContextFactory).
                await using var context = await _contextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

                // 2) Stratégie d’exécution (supporte EnableRetryOnFailure / SqlServerRetryingExecutionStrategy).
                var strategy = context.Database.CreateExecutionStrategy();

                // 3) Toute transaction explicite doit être exécutée dans ExecuteAsync().
                await strategy.ExecuteAsync(async () =>
                {
                    // 3.1) Transaction explicite : garantit l’atomicité "delete + event store".
                    await using var tx = await context.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

                    try
                    {
                        // 3.2) Récupération de l'entité à supprimer (dans le même retriable unit).
                        var entity = await context.Set<T>().FindAsync(new object?[] { id }, ct).ConfigureAwait(false);

                        // 3.3) Si l'entité n'existe pas, on ne fait rien (choix : silent no-op).
                        if (entity is null)
                        {
                            await tx.CommitAsync(ct).ConfigureAwait(false);
                            return;
                        }

                        // 3.4) Suppression (hard delete).
                        context.Set<T>().Remove(entity);

                        // 3.5) Persistance du DELETE.
                        await context.SaveChangesAsync(ct).ConfigureAwait(false);

                        // 3.6) Journalisation EventStore (optionnelle).
                        if (logEventStore)
                        {
                            // On construit le record après la réussite du REMOVE.
                            // Id attendu > 0 (sinon Ex_Business via BuildEventStoreRecord).
                            var eventStoreRecord = BuildEventStoreRecord(callChain, entity);

                            context.Set<UserAppEventStore>().Add(eventStoreRecord);

                            // Persiste l’event store dans la même transaction.
                            await context.SaveChangesAsync(ct).ConfigureAwait(false);
                        }

                        // 3.7) Commit final.
                        await tx.CommitAsync(ct).ConfigureAwait(false);
                    }
                    catch
                    {
                        // Rollback explicite pour garantir un état propre en cas d’exception.
                        await tx.RollbackAsync(ct).ConfigureAwait(false);
                        throw;
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }
        }

        #endregion

        #region === Méthodes privées ===

        /// <summary>
        /// <para>Description</para>
        /// <para>Journalise un snapshot JSON de l’entité dans <see cref="UserAppEventStore"/>.</para>
        /// <para>Contexte</para>
        /// <para>Appelée après chaque opération d’écriture (add, update, delete)
        /// afin d’assurer une traçabilité complète des modifications en base.</para>
        /// <para>Objectif</para>
        /// <para>Capturer l’état courant de l’entité sous forme sérialisée et l’associer
        /// au contexte applicatif (caller, handler, méthode).</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Récupérer dynamiquement la propriété <c>Id</c>.</description></item>
        /// <item><description>Sérialiser l’entité en JSON.</description></item>
        /// </list>
        /// <param name="entity">Entité concernée par l’opération.</param>
        /// <param name="caller">Chaîne d’appel amont permettant d’identifier l’origine fonctionnelle.</param>
        /// <exception cref="Ex_Infrastructure">Si l’écriture dans l’Event Store échoue.</exception>
        /// </summary>
        private UserAppEventStore BuildEventStoreRecord(string caller, T entity)
        {
            // callChain utilisée uniquement pour la classification d’exception
            string callChain = $"{caller} > {_callee} > {nameof(BuildEventStoreRecord)}";

            try
            {
                ArgumentNullException.ThrowIfNull(entity);
                if (string.IsNullOrWhiteSpace(caller))
                    throw new ArgumentException("Le caller ne peut pas être vide.", nameof(caller));

                // Id doit être disponible AU MOMENT où on journalise.
                var idProp = typeof(T).GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                if (idProp is null || idProp.PropertyType != typeof(int))
                    throw new Ex_Business($"Entity type '{typeof(T).Name}' must expose an int Id property.");

                int id = (int)(idProp.GetValue(entity) ?? 0);
                if (id <= 0)
                    throw new Ex_Business($"Entity type '{typeof(T).Name}' has an invalid Id={id} for EventStore.");

                string data = JsonSerializer.Serialize(entity, _jsonOptions);

                DTO_AppContext appCtx = _appContext.GetAppContext();

                return new UserAppEventStore
                {
                    TableDesignation = typeof(T).Name,
                    TableId = id,
                    Timestamp = appCtx.AppDateTime,
                    Data = data,
                    AppId = appCtx.AppId,
                    AppCallChain = caller,
                    AppUserId = appCtx.AppUserId,
                    DeviceUser = appCtx.AppDeviceUser,
                    DeviceId = appCtx.AppDeviceId,
                    DeviceIp = appCtx.AppDeviceIP
                };

            }
            catch (Exception ex)
            {
                throw Ex_Classifier.Execute(callChain, ex);
            }

        }

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };
        #endregion
    }
}