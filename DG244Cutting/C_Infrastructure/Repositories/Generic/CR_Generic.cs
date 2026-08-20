using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;

namespace DG244Cutting.C_Infrastructure.Repositories.Generic
{
    /// <summary>
    /// Implémentation générique d'un repository EF Core pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans C_Infrastructure et constitue la brique de base
    /// réutilisable de l'accès aux données. Elle implémente <see cref="IR_Generic{T}"/> et
    /// encapsule la totalité des appels EF Core. Aucun appel EF Core ne doit figurer en dehors
    /// de cette couche dans la solution.
    /// </para>
    /// <para>
    /// Modèle transactionnel : cette classe reçoit un <see cref="DbContext"/> partagé via injection
    /// de dépendances. Ce partage est la condition nécessaire à la cohérence transactionnelle portée
    /// par le UseCase orchestrateur : toutes les mutations inscrites via ce repository participent
    /// à la même transaction que les autres opérations du scénario métier en cours.
    /// </para>
    /// <para>
    /// Gestion des champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) :
    /// ces champs sont positionnés par le Command Handler appelant (<c>CH_Generic&lt;T&gt;</c>)
    /// avant tout appel aux méthodes de mutation de ce repository. Ce repository est une couche
    /// d'infrastructure pure : il n'applique aucune logique cross-coupante sur les entités.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Inscrire les mutations dans le change tracker du DbContext partagé.</description></item>
    ///   <item><description>Exécuter les requêtes de lecture via EF Core.</description></item>
    ///   <item><description>Propager la CallChain pour la traçabilité des accès données.</description></item>
    ///   <item><description>Requalifier les exceptions EF Core en types applicatifs via <see cref="IS_ExClassifier"/>.</description></item>
    ///   <item><description>Supporter l'annulation coopérative via <see cref="CancellationToken"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>
    ///     Ne persiste jamais les changements (<c>SaveChangesAsync</c> absent) :
    ///     cette responsabilité appartient exclusivement au UseCase orchestrateur.
    ///   </description></item>
    ///   <item><description>Ne gère aucune logique métier ni validation fonctionnelle.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas, n'absorbe pas d'exception silencieusement.</description></item>
    ///   <item><description>
    ///     Ne positionne pas les champs d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) :
    ///     cette responsabilité appartient au Command Handler appelant.
    ///   </description></item>
    ///   <item><description>
    ///     Ne crée pas son propre DbContext : il opère sur celui injecté par le conteneur DI,
    ///     à durée de vie scoped pour la durée du UseCase en cours.
    ///   </description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité manipulée. Doit être une classe référence mappée dans le DbContext.
    /// </typeparam>
    public class CR_Generic<T> : IR_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Contexte EF Core partagé, injecté par le conteneur DI pour la durée du UseCase en cours.
        /// Toutes les opérations d'écriture réalisées via ce contexte participent à la même
        /// transaction que celle ouverte par le UseCase orchestrateur.
        /// </summary>
        protected readonly DbContext _context;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="CR_Generic{T}"/> avec le contexte EF Core
        /// partagé et le classificateur d'exceptions.
        /// </summary>
        /// <remarks>
        /// Le DbContext reçu est celui ouvert et géré par le UseCase orchestrateur. Ce partage
        /// est la condition nécessaire à la propagation implicite de la transaction dans toute
        /// la chaîne d'appel du scénario métier en cours d'exécution.
        /// </remarks>
        /// <param name="context">
        /// Instance du DbContext EF Core partagé pour la durée du UseCase en cours d'exécution.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées. Utilisé pour convertir
        /// les exceptions EF Core en types applicatifs structurés compatibles avec le pipeline
        /// de gestion des erreurs. Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="context"/> ou <paramref name="classifier"/> est <see langword="null"/>.
        /// </exception>
        public CR_Generic(DbContext context, IS_ExClassifier classifier)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        // ─── Création ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Ajoute une entité dans le contexte EF Core sans déclencher la persistance.
        /// </summary>
        /// <remarks>
        /// La persistance effective est déclenchée par le UseCase via <c>SaveChangesAsync</c>
        /// dans le périmètre de sa transaction. Cette méthode se limite à inscrire l'entité
        /// dans le change tracker du DbContext partagé.
        /// <para>
        /// Les champs techniques d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>)
        /// doivent avoir été positionnés par le Command Handler appelant avant l'appel à cette méthode.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour enrichissement et traçabilité.</param>
        /// <param name="entity">Entité de type <typeparamref name="T"/> à ajouter. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'inscription dans le change tracker
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task AddAsync(string caller, T entity, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(AddAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"L'entité de type {typeof(T).Name} fournie est nulle.");

                ct.ThrowIfCancellationRequested();

                await _context.Set<T>().AddAsync(entity, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        // ─── Lecture ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Retourne l'entité correspondant à l'identifiant spécifié, ou <see langword="null"/> si absente.
        /// </summary>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant entier de l'entité recherchée. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>L'entité de type <typeparamref name="T"/> si trouvée ; <see langword="null"/> sinon.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> GetByIdAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByIdAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().FindAsync(new object[] { id }, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne l'entité correspondant à l'identifiant spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Variante NoTracking de <see cref="GetByIdAsync"/>. À privilégier pour les lectures pures
        /// effectuées par les Query Handlers où l'entité retournée ne sera pas mutée.
        /// </para>
        /// <para>
        /// L'accès par identifiant repose sur la convention "la clé primaire s'appelle <c>Id</c>"
        /// (type <see cref="int"/>), résolue dynamiquement via <c>EF.Property&lt;int&gt;</c>.
        /// <c>FindAsync</c> ne supporte pas <c>AsNoTracking</c> : la requête est construite avec
        /// <c>Where</c> + <c>FirstOrDefaultAsync</c>.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant entier de l'entité recherchée. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>L'entité de type <typeparamref name="T"/> si trouvée, non suivie ; <see langword="null"/> sinon.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> GetByIdAsNoTrackingAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetByIdAsNoTrackingAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne le premier enregistrement de la table, ou <see langword="null"/> si la table est vide.
        /// </summary>
        /// <remarks>
        /// À utiliser uniquement lorsque la table est censée contenir au plus un enregistrement,
        /// ou lorsque le premier enregistrement est pertinent indépendamment d'un critère de tri.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Le premier enregistrement de type <typeparamref name="T"/> ; <see langword="null"/> si la table est vide.</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> GetFirstOrDefaultAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFirstOrDefaultAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().FirstOrDefaultAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne le premier enregistrement de la table sans suivi des changements,
        /// ou <see langword="null"/> si la table est vide.
        /// </summary>
        /// <remarks>
        /// Variante NoTracking de <see cref="GetFirstOrDefaultAsync(string, CancellationToken)"/>.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Le premier enregistrement de type <typeparamref name="T"/>, non suivi ; <see langword="null"/> si la table est vide.</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> GetFirstOrDefaultAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFirstOrDefaultAsNoTrackingAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne le premier enregistrement satisfaisant le prédicat spécifié, sans suivi des changements,
        /// ou <see langword="null"/> si aucun enregistrement ne satisfait le prédicat.
        /// </summary>
        /// <remarks>
        /// Le filtrage est exécuté côté base de données via une clause <c>WHERE</c> SQL générée
        /// par EF Core, suivie d'une limitation à un seul enregistrement.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Le premier enregistrement satisfaisant le prédicat, non suivi ; <see langword="null"/> sinon.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat de filtrage fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> GetFirstOrDefaultAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFirstOrDefaultAsNoTrackingAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Indique si un enregistrement avec l'identifiant spécifié existe dans la table.
        /// </summary>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant entier de l'entité dont on vérifie l'existence. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns><see langword="true"/> si un enregistrement avec cet identifiant existe ; <see langword="false"/> sinon.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la vérification d'existence
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<bool> GetAnyAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAnyAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                var entity = await _context.Set<T>().FindAsync(new object[] { id }, ct);
                return entity is not null;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne l'ensemble des enregistrements de la table avec suivi des changements actif.
        /// </summary>
        /// <remarks>
        /// Utiliser <see cref="GetAllAsNoTrackingAsync"/> pour les lectures seules afin d'éviter
        /// la surcharge du change tracker EF Core.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste complète de toutes les entités <typeparamref name="T"/> persistées.</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la récupération de la liste
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> GetAllAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAllAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne l'ensemble des enregistrements de la table sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// À privilégier pour les opérations de lecture pure (Query Handlers). Le mode
        /// <c>AsNoTracking</c> améliore les performances et réduit la consommation mémoire.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste complète de toutes les entités <typeparamref name="T"/>, non suivies.</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la récupération de la liste
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> GetAllAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetAllAsNoTrackingAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().AsNoTracking().ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Retourne les enregistrements satisfaisant le prédicat de filtrage spécifié.
        /// </summary>
        /// <remarks>
        /// Le filtrage est exécuté côté base de données via une clause <c>WHERE</c> SQL générée
        /// par EF Core. Ce mécanisme remplace avantageusement l'usage de <c>IQueryable</c> direct,
        /// qui impose des contraintes sur la durée de vie du DbContext.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste des entités <typeparamref name="T"/> satisfaisant le prédicat.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat de filtrage fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête filtrée
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> GetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFilteredAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().Where(predicate).ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        /// <summary>
        /// Retourne les enregistrements satisfaisant le prédicat de filtrage spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// Variante NoTracking de <see cref="GetFilteredAsync"/>. À privilégier pour les lectures
        /// pures où les entités retournées ne seront pas mutées. Le filtrage est exécuté côté base
        /// de données via une clause <c>WHERE</c> SQL générée par EF Core.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste des entités <typeparamref name="T"/> satisfaisant le prédicat, non suivies.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat de filtrage fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête filtrée
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> GetFilteredAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetFilteredAsNoTrackingAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().AsNoTracking().Where(predicate).ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        // ─── Pagination ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Retourne une fenêtre d'enregistrements paginée, satisfaisant éventuellement un prédicat,
        /// triée selon le critère spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Mécanisme : EF Core traduit la requête en <c>OFFSET ... FETCH NEXT ... ROWS ONLY</c> (SQL Server)
        /// ou équivalent, ce qui limite le nombre d'enregistrements ramenés du serveur de base de données.
        /// </para>
        /// <para>
        /// Le critère de tri est obligatoire : sans <c>ORDER BY</c>, la pagination SQL retourne des
        /// résultats non déterministes entre deux appels successifs. Seul un tri ascendant simple
        /// (une seule colonne) est exposé par le socle ; les besoins de tri descendant ou multi-colonnes
        /// relèvent des repositories spécialisés.
        /// </para>
        /// <para>
        /// Pour obtenir le nombre total d'enregistrements satisfaisant le prédicat (affichage "page 3 sur 12"),
        /// l'appelant effectue un appel séparé à <see cref="CountAsync"/> avec le même prédicat
        /// (deux requêtes SQL distinctes).
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage. Peut être <see langword="null"/> : aucun filtre n'est alors appliqué.</param>
        /// <param name="orderBy">Expression de sélection de la colonne de tri ascendant. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="skip">Nombre d'enregistrements à sauter avant la fenêtre retournée. Doit être supérieur ou égal à zéro.</param>
        /// <param name="take">Nombre d'enregistrements à retourner dans la fenêtre. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste des entités <typeparamref name="T"/> correspondant à la fenêtre paginée, non suivies. Liste vide si aucun enregistrement ne correspond.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="orderBy"/> est <see langword="null"/> (code <c>BU_ER_01</c>),
        /// si <paramref name="skip"/> est strictement négatif (code <c>BU_ER_02</c>),
        /// ou si <paramref name="take"/> est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête paginée
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> GetPagedAsNoTrackingAsync(
            string caller,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            int skip,
            int take,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(GetPagedAsNoTrackingAsync)}";

            try
            {
                if (orderBy is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le critère de tri fourni pour la pagination de {typeof(T).Name} est nul. Le tri est obligatoire en pagination.");

                if (skip < 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Le paramètre skip fourni pour la pagination de {typeof(T).Name} est invalide : {skip}. Doit être supérieur ou égal à zéro.");

                if (take <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"Le paramètre take fourni pour la pagination de {typeof(T).Name} est invalide : {take}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                IQueryable<T> query = _context.Set<T>().AsNoTracking();
                if (predicate is not null)
                    query = query.Where(predicate);

                return await query.OrderBy(orderBy).Skip(skip).Take(take).ToListAsync(ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        // ─── Comptage et existence ───────────────────────────────────────────────────

        /// <summary>
        /// Retourne le nombre d'enregistrements de la table, éventuellement restreint par un prédicat.
        /// </summary>
        /// <remarks>
        /// L'opération est traduite en <c>SELECT COUNT(*)</c> SQL côté serveur de base de données :
        /// aucun enregistrement n'est matérialisé en mémoire. À utiliser conjointement avec
        /// <see cref="GetPagedAsNoTrackingAsync"/> pour calculer le nombre total de pages.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage. Peut être <see langword="null"/> : le comptage porte alors sur la table entière.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Nombre d'enregistrements satisfaisant le prédicat (ou de la table entière si <paramref name="predicate"/> est <see langword="null"/>).</returns>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors du comptage (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<int> CountAsync(string caller, Expression<Func<T, bool>>? predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(CountAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return predicate is null
                    ? await _context.Set<T>().CountAsync(ct)
                    : await _context.Set<T>().CountAsync(predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Indique si au moins un enregistrement de la table satisfait le prédicat spécifié.
        /// </summary>
        /// <remarks>
        /// L'opération est traduite en <c>SELECT TOP 1 1 WHERE ...</c> SQL côté serveur de base de données :
        /// aucun enregistrement n'est matérialisé en mémoire. Complète <see cref="GetAnyAsync"/> qui est
        /// limité à la vérification d'existence par clé primaire.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns><see langword="true"/> si au moins un enregistrement satisfait le prédicat ; <see langword="false"/> sinon.</returns>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la vérification d'existence
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<bool> AnyByPredicateAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(AnyByPredicateAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat fourni pour la vérification d'existence de {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _context.Set<T>().AnyAsync(predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        // ─── Mise à jour ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Marque une entité comme modifiée dans le change tracker sans déclencher la persistance.
        /// </summary>
        /// <remarks>
        /// Les champs techniques d'audit (<c>UpdatedAt</c>) doivent avoir été positionnés
        /// par le Command Handler appelant avant l'appel à cette méthode.
        /// <c>CreatedAt</c> ne doit jamais être modifié lors d'une mise à jour.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="entity">Entité de type <typeparamref name="T"/> à mettre à jour. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la mise à jour du change tracker
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant l'exécution.
        /// </exception>
        public async Task UpdateAsync(string caller, T entity, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(UpdateAsync)}";

            try
            {
                if (entity is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"L'entité de type {typeof(T).Name} fournie pour la mise à jour est nulle.");

                ct.ThrowIfCancellationRequested();

                _context.Set<T>().Update(entity);
                await Task.CompletedTask;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Marque un ensemble d'entités comme modifiées dans le change tracker sans déclencher la persistance.
        /// </summary>
        /// <remarks>
        /// Les champs techniques d'audit (<c>UpdatedAt</c>) doivent avoir été positionnés
        /// par le Command Handler appelant sur chaque entité avant l'appel à cette méthode.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="entities">Collection d'entités de type <typeparamref name="T"/> à mettre à jour. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si la collection d'entités fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la mise à jour en masse du change tracker
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant l'exécution.
        /// </exception>
        public async Task UpdateRangeAsync(string caller, IEnumerable<T> entities, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(UpdateRangeAsync)}";

            try
            {
                if (entities is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"La collection d'entités de type {typeof(T).Name} fournie pour la mise à jour en masse est nulle.");

                ct.ThrowIfCancellationRequested();

                _context.Set<T>().UpdateRange(entities);
                await Task.CompletedTask;
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }


        // ─── Suppression ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Supprime physiquement l'entité correspondant à l'identifiant spécifié, si elle existe.
        /// </summary>
        /// <remarks>
        /// Si aucune entité ne correspond à l'identifiant fourni, la méthode retourne sans
        /// erreur. La suppression effective est validée lors du <c>SaveChangesAsync</c> du UseCase.
        /// <para>
        /// Pour une suppression logique (soft delete), utiliser <see cref="SoftDeleteAsync"/> à la place.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant de l'entité à supprimer. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la suppression physique
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task DeleteAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(DeleteAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour la suppression physique de {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                var entity = await _context.Set<T>().FindAsync(new object[] { id }, ct);
                if (entity is not null)
                    _context.Set<T>().Remove(entity);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <summary>
        /// Marque l'entité correspondant à l'identifiant spécifié comme supprimée logiquement,
        /// sans suppression physique de l'enregistrement en base de données.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cette méthode ne supprime pas l'enregistrement. Elle inscrit dans le change tracker
        /// l'entité déjà modifiée (champs <c>IsDeleted</c> et <c>UpdatedAt</c> positionnés
        /// par le Command Handler appelant avant l'appel) comme étant à mettre à jour.
        /// </para>
        /// <para>
        /// Le mécanisme repose sur l'identity map du DbContext partagé : l'instance retournée
        /// par <c>FindAsync</c> en interne est la même instance déjà modifiée par le Command Handler
        /// appelant (qui a au préalable chargé l'entité via <see cref="GetByIdAsync"/>).
        /// </para>
        /// <para>
        /// Si aucune entité ne correspond à l'identifiant fourni, la méthode retourne sans erreur.
        /// La persistance effective est validée lors du <c>SaveChangesAsync</c> du UseCase.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant de l'entité à marquer comme supprimée logiquement. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <remarks>
        /// <para>
        /// Mécanisme d'identity map : le Command Handler appelant (<c>CH_Generic&lt;T&gt;</c>)
        /// a préalablement chargé l'entité via <c>GetByIdAsync</c> et positionné les champs
        /// <c>IsDeleted</c> et <c>UpdatedAt</c> sur l'instance en mémoire. Le <c>FindAsync</c>
        /// exécuté dans cette méthode retourne la même instance trackée (identity map du DbContext
        /// partagé) sans déclencher de requête SQL supplémentaire. L'appel à <c>Update</c>
        /// marque alors l'entité — avec ses champs d'audit déjà modifiés — comme <c>Modified</c>
        /// dans le change tracker.
        /// </para>
        /// </remarks>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors du soft delete
        /// (code <c>IN_ER_06</c>).
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task SoftDeleteAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(SoftDeleteAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour le soft delete de {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                // FindAsync retourne l'instance déjà trackée et déjà modifiée par le Command Handler
                // appelant (champs IsDeleted et UpdatedAt positionnés avant cet appel).
                // Aucune requête SQL supplémentaire n'est émise si l'entité est déjà dans le tracker.
                var entity = await _context.Set<T>().FindAsync(new object[] { id }, ct);
                if (entity is not null)
                    _context.Set<T>().Update(entity);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // Réservé aux méthodes privées d'assistance spécifiques à cette classe.
        // La logique de positionnement des champs d'audit (CreatedAt, UpdatedAt, IsDeleted)
        // relève du Command Handler (CH_Generic<T>) et non de ce repository.

        #endregion
    }
}