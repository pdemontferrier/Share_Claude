using System.Reflection;
using System.Text.Json;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Handlers.Commands;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;

namespace DG244Cutting.B_UseCases.Handlers.Generic
{
    /// <summary>
    /// Implémentation générique d'un Command Handler CQRS pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et constitue le pivot entre la logique
    /// d'orchestration des Services métier et la couche de persistance de C_Infrastructure.
    /// Elle implémente <see cref="IC_Generic{T}"/> et coordonne systématiquement, pour chaque
    /// opération d'écriture, la mutation de l'entité métier et son enregistrement dans l'Event Store.
    /// </para>
    /// <para>
    /// Solidarité transactionnelle : les deux opérations — mutation via le repository et
    /// enregistrement Event Store via <see cref="IC_UserAppEventStore"/> — s'inscrivent dans
    /// la même transaction ouverte par le UseCase orchestrateur, via le DbContext partagé.
    /// Aucune de ces deux opérations ne déclenche de persistance autonome.
    /// </para>
    /// <para>
    /// Gestion des champs d'audit : cette classe est responsable du positionnement automatique
    /// des trois champs techniques communs à toutes les entités persistées, via réflexion sur
    /// la convention de nommage des propriétés. Le positionnement s'effectue avant tout appel
    /// au repository, garantissant que les valeurs sont en place dans le change tracker EF Core
    /// au moment du <c>SaveChangesAsync</c> du UseCase.
    /// </para>
    /// <list type="bullet">
    ///   <item><description>
    ///     <c>CreatedAt</c> (<c>DateTime</c> ou <c>DateTime?</c>) : positionné à <c>DateTime.UtcNow</c>
    ///     lors de <see cref="HandleAddAsync"/>. Non altéré lors des mises à jour ou du soft delete.
    ///   </description></item>
    ///   <item><description>
    ///     <c>UpdatedAt</c> (<c>DateTime</c> ou <c>DateTime?</c>) : positionné à <c>DateTime.UtcNow</c>
    ///     lors de <see cref="HandleUpdateAsync"/>, <see cref="HandleUpdateRangeAsync"/>
    ///     et <see cref="HandleSoftDeleteAsync"/>. Non renseigné lors de <see cref="HandleAddAsync"/>.
    ///   </description></item>
    ///   <item><description>
    ///     <c>IsDeleted</c> (<c>bool</c> ou <c>bool?</c>) : positionné à <see langword="true"/>
    ///     exclusivement lors de <see cref="HandleSoftDeleteAsync"/>. Non forcé lors des autres opérations.
    ///   </description></item>
    /// </list>
    /// <para>
    /// Convention de nommage : le positionnement repose sur la présence de propriétés publiques
    /// accessibles en écriture, nommées exactement <c>CreatedAt</c>, <c>UpdatedAt</c> et
    /// <c>IsDeleted</c> sur le type <typeparamref name="T"/>. Les entités ne les possédant pas
    /// ne sont pas impactées (comportement tolérant silencieux), à l'exception de <c>IsDeleted</c>
    /// lors d'un soft delete, qui lève une <see cref="Ex_Business"/> si la propriété est absente.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Valider les paramètres métier entrants avant toute délégation au repository.</description></item>
    ///   <item><description>Positionner les champs d'audit avant toute mutation.</description></item>
    ///   <item><description>Déléguer la mutation de l'entité au repository via <see cref="IR_Generic{T}"/>.</description></item>
    ///   <item><description>Déclencher l'enregistrement Event Store via <see cref="IC_UserAppEventStore"/> pour chaque mutation.</description></item>
    ///   <item><description>Propager la CallChain depuis le Service appelant jusqu'au repository et à l'Event Store.</description></item>
    ///   <item><description>Requalifier les exceptions non contrôlées via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne gère pas les transactions : responsabilité exclusive du UseCase orchestrateur.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : responsabilités du UseCase via <c>IU_LogAndNotify</c>.</description></item>
    ///   <item><description>Ne contient aucune logique métier : la validation se limite aux préconditions structurelles des paramètres.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité métier manipulée. Doit être une classe référence.
    /// </typeparam>
    public class CH_Generic<T> : IC_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>Repository générique délégué pour les opérations de mutation sur les entités <typeparamref name="T"/>.</summary>
        private readonly IR_Generic<T> _repository;

        /// <summary>Command Handler de l'Event Store, responsable de la persistance de chaque enregistrement d'audit.</summary>
        private readonly IC_UserAppEventStore _eventStore;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CH_Generic{T}"/> avec ses dépendances opérationnelles.
        /// </summary>
        /// <param name="repository">
        /// Repository générique EF Core pour les mutations d'entités <typeparamref name="T"/>.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="eventStore">
        /// Command Handler de l'Event Store. Chaque opération de mutation déclenche un appel
        /// à ce composant pour garantir la traçabilité durable. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="repository"/> ou <paramref name="eventStore"/> 
        /// ou <paramref name="classifier"/> est <see langword="null"/>.
        /// </exception>
        public CH_Generic(IR_Generic<T> repository, IC_UserAppEventStore eventStore, IS_ExClassifier classifier)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleAddAsync(string caller, T entity, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAddAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"L'entité de type {typeof(T).Name} fournie pour la création est nulle.");

                ct.ThrowIfCancellationRequested();

                // Positionnement de CreatedAt à la date UTC courante.
                // UpdatedAt n'est intentionnellement pas renseigné à la création (valeur NULL en base).
                // IsDeleted conserve sa valeur par défaut (false / 0) garantie par le défaut SQL et C#.
                SetCreatedAtIfExists(entity);

                await _repository.AddAsync(callChain, entity, ct);
                await LogEventAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleUpdateAsync(string caller, T entity, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleUpdateAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"L'entité de type {typeof(T).Name} fournie pour la mise à jour est nulle.");

                ct.ThrowIfCancellationRequested();

                // Positionnement de UpdatedAt à la date UTC courante.
                // CreatedAt n'est jamais altéré lors d'une mise à jour.
                // IsDeleted n'est pas forcé : toute décision métier sur ce champ appartient au UseCase appelant.
                SetUpdatedAtIfExists(entity);

                await _repository.UpdateAsync(callChain, entity, ct);
                await LogEventAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si la collection fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleUpdateRangeAsync(string caller, IEnumerable<T> entities, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleUpdateRangeAsync)}";

            try
            {
                if (entities is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"La collection d'entités de type {typeof(T).Name} fournie pour la mise à jour en masse est nulle.");

                ct.ThrowIfCancellationRequested();

                // Matérialisation de la séquence pour garantir une itération stable sur plusieurs passes
                // (SetUpdatedAtIfExists + UpdateRangeAsync + LogEventAsync séquentiel).
                var entityList = entities as IReadOnlyList<T> ?? entities.ToList();

                // Positionnement de UpdatedAt sur chaque entité avant délégation au repository.
                // CreatedAt n'est jamais altéré lors d'une mise à jour en masse.
                foreach (var entity in entityList)
                    SetUpdatedAtIfExists(entity);

                await _repository.UpdateRangeAsync(callChain, entityList, ct);

                // Enregistrement Event Store séquentiel : le DbContext partagé n'est pas thread-safe.
                // Task.WhenAll est intentionnellement exclu pour éviter toute concurrence sur le contexte.
                foreach (var entity in entityList)
                {
                    ct.ThrowIfCancellationRequested();
                    await LogEventAsync(callChain, entity, ct);
                }
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture, de la suppression ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleDeleteAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleDeleteAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour la suppression physique de {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture préalable : nécessaire pour capturer l'état de l'entité avant suppression
                // et pour vérifier l'existence sans lever d'erreur si l'enregistrement est absent.
                var entity = await _repository.GetByIdAsync(callChain, id, ct);

                if (entity is null)
                    return;

                await _repository.DeleteAsync(callChain, id, ct);
                await LogEventAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>),
        /// ou si le type <typeparamref name="T"/> ne possède pas de propriété <c>IsDeleted</c>
        /// accessible en écriture (code <c>BU_ER_03</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture, de la mise à jour ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task HandleSoftDeleteAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleSoftDeleteAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour le soft delete de {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // Lecture préalable : nécessaire pour capturer l'instance trackée par EF Core
                // (identity map du DbContext partagé) afin de modifier ses propriétés en mémoire,
                // et pour vérifier l'existence sans lever d'erreur si l'enregistrement est absent.
                var entity = await _repository.GetByIdAsync(callChain, id, ct);

                if (entity is null)
                    return; // Aucune entité à soft-supprimer — no-op, sans enregistrement Event Store.

                // IsDeleted doit exister sur l'entité : absence = erreur de conception explicite.
                if (!SetIsDeletedTrueIfExists(entity))
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_03,
                        $"L'entité de type {typeof(T).Name} ne supporte pas le soft delete : " +
                        $"la propriété IsDeleted (bool ou bool?) est absente ou non accessible en écriture.");

                // UpdatedAt renseigné après IsDeleted, dans la même opération logique.
                SetUpdatedAtIfExists(entity);

                // SoftDeleteAsync retrouve l'instance déjà modifiée via l'identity map EF Core
                // et la marque comme Modified dans le change tracker (sans requête SQL supplémentaire).
                await _repository.SoftDeleteAsync(callChain, id, ct);
                await LogEventAsync(callChain, entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        /// <summary>
        /// Construit et soumet un enregistrement Event Store pour la mutation d'entité spécifiée.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cette méthode est le point d'articulation entre la mutation métier et l'Event Store.
        /// Elle extrait l'identifiant de l'entité par réflexion sur la propriété <c>Id</c> (convention
        /// de nommage attendue, type <see langword="int"/>). Si la propriété est absente ou d'un type
        /// différent, l'identifiant est positionné à <c>0</c> sans erreur, afin de ne pas bloquer
        /// la persistance pour une limitation de traçabilité.
        /// </para>
        /// <para>
        /// La valeur <paramref name="caller"/> transmise à <see cref="IC_UserAppEventStore"/> est
        /// la callChain construite au niveau de la méthode publique appelante. Elle est stockée
        /// telle quelle dans le champ <c>AppCallChain</c> de l'entité Event Store, conformément
        /// aux spécifications du projet. La chaîne locale de cette méthode privée n'est utilisée
        /// qu'à des fins de diagnostic interne.
        /// </para>
        /// </remarks>
        /// <param name="caller">
        /// CallChain complète construite au niveau de la méthode publique appelante.
        /// Transmise sans modification à <see cref="IC_UserAppEventStore.HandleAddAsync"/>
        /// pour constituer le champ <c>AppCallChain</c> de l'enregistrement Event Store.
        /// </param>
        /// <param name="entity">Entité dont la mutation vient d'être inscrite dans le change tracker.</param>
        /// <param name="commandMethod">Nom de la méthode publique appelante, stocké dans <c>AppCommandMethod</c>.</param>
        /// <param name="ct">Jeton d'annulation propagé depuis la méthode publique appelante.</param>
        private async Task LogEventAsync(string caller, T entity, CancellationToken ct)
        {
            // Extraction de l'identifiant par convention de nommage (propriété "Id" de type int).
            // Retourne 0 si la propriété est absente ou d'un type non entier — comportement tolérant
            // documenté dans les remarques de cette méthode.
            var idProperty = typeof(T).GetProperty("Id");
            int tableId = idProperty?.GetValue(entity) is int value ? value : 0;

            string data = JsonSerializer.Serialize(entity);

            await _eventStore.HandleAddAsync(
                caller,           // callChain jusqu'à la méthode publique appelante
                typeof(T).Name,   // désignation de l'entité métier
                tableId,          // identifiant de l'enregistrement modifié
                data,             // image JSON de l'entité au moment de la mutation
                ct);
        }

        /// <summary>
        /// Positionne la propriété <c>CreatedAt</c> à la date UTC courante si elle existe sur
        /// le type <typeparamref name="T"/> et est accessible en écriture.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conventions supportées : <c>DateTime</c> (propriété non-nullable, correspondant à
        /// <c>datetime2 NOT NULL</c> en SQL Server) et <c>DateTime?</c> (propriété nullable).
        /// Si la propriété est absente ou d'un type incompatible, la méthode retourne silencieusement
        /// sans erreur ni avertissement.
        /// </para>
        /// <para>
        /// Cette méthode ne doit être appelée que dans <see cref="HandleAddAsync"/>. Elle ne doit
        /// jamais être invoquée lors des mises à jour ou du soft delete afin de ne pas altérer
        /// la date de création originale de l'enregistrement.
        /// </para>
        /// </remarks>
        /// <param name="entity">Instance de l'entité sur laquelle positionner <c>CreatedAt</c>.</param>
        private static void SetCreatedAtIfExists(T entity)
        {
            var prop = typeof(T).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite) return;

            if (prop.PropertyType == typeof(DateTime))
                prop.SetValue(entity, DateTime.UtcNow);
            else if (prop.PropertyType == typeof(DateTime?))
                prop.SetValue(entity, (DateTime?)DateTime.UtcNow);
        }

        /// <summary>
        /// Positionne la propriété <c>UpdatedAt</c> à la date UTC courante si elle existe sur
        /// le type <typeparamref name="T"/> et est accessible en écriture.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conventions supportées : <c>DateTime</c> et <c>DateTime?</c> (correspondant à
        /// <c>datetime2 NULL</c> en SQL Server). Si la propriété est absente ou d'un type
        /// incompatible, la méthode retourne silencieusement sans erreur.
        /// </para>
        /// <para>
        /// Cette méthode doit être appelée dans <see cref="HandleUpdateAsync"/>,
        /// <see cref="HandleUpdateRangeAsync"/> et <see cref="HandleSoftDeleteAsync"/>.
        /// Elle ne doit jamais être invoquée dans <see cref="HandleAddAsync"/> : <c>UpdatedAt</c>
        /// doit rester <see langword="null"/> à la création de l'enregistrement.
        /// </para>
        /// </remarks>
        /// <param name="entity">Instance de l'entité sur laquelle positionner <c>UpdatedAt</c>.</param>
        private static void SetUpdatedAtIfExists(T entity)
        {
            var prop = typeof(T).GetProperty("UpdatedAt", BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite) return;

            if (prop.PropertyType == typeof(DateTime))
                prop.SetValue(entity, DateTime.UtcNow);
            else if (prop.PropertyType == typeof(DateTime?))
                prop.SetValue(entity, (DateTime?)DateTime.UtcNow);
        }

        /// <summary>
        /// Positionne la propriété <c>IsDeleted</c> à <see langword="true"/> si elle existe sur
        /// le type <typeparamref name="T"/> et est accessible en écriture.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Conventions supportées : <c>bool</c> (correspondant à <c>bit NOT NULL</c> en SQL Server)
        /// et <c>bool?</c> (nullable). Si la propriété est présente et modifiée avec succès,
        /// retourne <see langword="true"/>. Si elle est absente, d'un type incompatible ou non
        /// accessible en écriture, retourne <see langword="false"/> sans erreur.
        /// </para>
        /// <para>
        /// Le retour <see langword="false"/> est traité comme une erreur dans
        /// <see cref="HandleSoftDeleteAsync"/> : le soft delete n'est pas applicable à une entité
        /// ne supportant pas ce champ. Une <see cref="Ex_Business"/> est levée dans ce cas.
        /// </para>
        /// <para>
        /// Cette méthode ne doit être appelée que dans <see cref="HandleSoftDeleteAsync"/>.
        /// Elle ne doit jamais être invoquée dans les opérations de création ou de mise à jour
        /// standard, où la valeur d'<c>IsDeleted</c> relève de la décision métier du UseCase.
        /// </para>
        /// </remarks>
        /// <param name="entity">Instance de l'entité sur laquelle positionner <c>IsDeleted</c>.</param>
        /// <returns>
        /// <see langword="true"/> si la propriété a été trouvée et positionnée à <see langword="true"/> ;
        /// <see langword="false"/> si la propriété est absente ou inaccessible.
        /// </returns>
        private static bool SetIsDeletedTrueIfExists(T entity)
        {
            var prop = typeof(T).GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance);
            if (prop is null || !prop.CanWrite) return false;

            if (prop.PropertyType == typeof(bool))
            {
                prop.SetValue(entity, true);
                return true;
            }

            if (prop.PropertyType == typeof(bool?))
            {
                prop.SetValue(entity, (bool?)true);
                return true;
            }

            return false;
        }

        #endregion
    }
}