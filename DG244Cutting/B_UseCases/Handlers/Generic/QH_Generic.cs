using System.Linq.Expressions;
using DG244Cutting.A_Domain.Common.Exceptions;
using DG244Cutting.A_Domain.Interfaces.Handlers.Generic;
using DG244Cutting.A_Domain.Interfaces.Repositories.Generic;
using DG244Cutting.A_Domain.Interfaces.Services.App;

namespace DG244Cutting.B_UseCases.Handlers.Generic
{
    /// <summary>
    /// Implémentation générique d'un Query Handler CQRS pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette classe réside dans B_UseCases et constitue la classe de base obligatoire
    /// des Query Handlers typés (<c>QH_UserApp</c>, <c>QH_Session</c>, etc.). Elle implémente
    /// <see cref="IQ_Generic{T}"/> et coordonne la délégation au repository générique pour les
    /// treize lectures du contrat de base.
    /// </para>
    /// <para>
    /// Mode d'usage normatif : extension par dérivation. Les Query Handlers typés héritent de
    /// <see cref="QH_Generic{T}"/> pour le compte de leur entité métier, appellent
    /// <c>base(repository, classifier)</c> en première instruction de leur constructeur, et
    /// ajoutent sans redéfinition les méthodes de lecture spécialisées propres à leur domaine.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Valider les paramètres entrants au niveau structurel avant toute délégation au repository.</description></item>
    ///   <item><description>Déléguer la lecture au repository via <see cref="IR_Generic{T}"/>.</description></item>
    ///   <item><description>Propager la CallChain depuis le Service appelant jusqu'au repository.</description></item>
    ///   <item><description>Requalifier les exceptions non contrôlées via <see cref="IS_ExClassifier"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne mute jamais l'état d'une entité ni ne déclenche d'enregistrement Event Store.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités appartiennent au UseCase via <c>IU_LogAndNotify</c>.</description></item>
    ///   <item><description>Ne contient aucune logique métier : la validation se limite aux préconditions structurelles des paramètres.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité métier manipulée. Doit être une classe référence.
    /// </typeparam>
    public class QH_Generic<T> : IQ_Generic<T> where T : class
    {
        #region === Propriétés privées ===

        /// <summary>Nom du composant courant, résolu dynamiquement pour la construction de la CallChain.</summary>
        private readonly string _callee;

        #endregion


        #region === Dépendances privées ===

        /// <summary>
        /// Repository générique délégué pour les opérations de lecture sur les entités <typeparamref name="T"/>.
        /// Privé : les classes dérivées accèdent aux lectures du socle via les méthodes Handle* héritées,
        /// jamais directement via ce champ. Pour leurs lectures spécialisées, elles injectent leur
        /// propre repository spécialisé dans leur propre constructeur.
        /// </summary>
        private readonly IR_Generic<T> _repository;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une instance de <see cref="QH_Generic{T}"/> avec ses dépendances opérationnelles.
        /// </summary>
        /// <param name="repository">
        /// Repository générique EF Core pour les lectures d'entités <typeparamref name="T"/>.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <param name="classifier">
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés.
        /// Ne doit pas être <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Levée si <paramref name="repository"/> ou <paramref name="classifier"/> est <see langword="null"/>.
        /// </exception>
        public QH_Generic(IR_Generic<T> repository, IS_ExClassifier classifier)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _callee = GetType().Name;
        }

        #endregion


        #region === Méthodes publiques ===

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> HandleGetByIdAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByIdAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetByIdAsync(callChain, id, ct);
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
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> HandleGetByIdAsNoTrackingAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetByIdAsNoTrackingAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetByIdAsNoTrackingAsync(callChain, id, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> HandleGetFirstOrDefaultAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFirstOrDefaultAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _repository.GetFirstOrDefaultAsync(callChain, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> HandleGetFirstOrDefaultAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFirstOrDefaultAsNoTrackingAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _repository.GetFirstOrDefaultAsNoTrackingAsync(callChain, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<T?> HandleGetFirstOrDefaultAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFirstOrDefaultAsNoTrackingAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetFirstOrDefaultAsNoTrackingAsync(callChain, predicate, ct);
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
        /// Levée si une défaillance technique survient lors de la vérification d'existence.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<bool> HandleGetAnyAsync(string caller, int id, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAnyAsync)}";

            try
            {
                if (id <= 0)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_02,
                        $"L'identifiant fourni pour {typeof(T).Name} est invalide : {id}. Doit être strictement positif.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetAnyAsync(callChain, id, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la récupération de la liste.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> HandleGetAllAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAllAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _repository.GetAllAsync(callChain, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la récupération de la liste.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> HandleGetAllAsNoTrackingAsync(string caller, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetAllAsNoTrackingAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _repository.GetAllAsNoTrackingAsync(callChain, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> HandleGetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFilteredAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetFilteredAsync(callChain, predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> HandleGetFilteredAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetFilteredAsNoTrackingAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat de filtrage fourni pour {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _repository.GetFilteredAsNoTrackingAsync(callChain, predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si <paramref name="orderBy"/> est <see langword="null"/> (code <c>BU_ER_01</c>),
        /// si <paramref name="skip"/> est strictement négatif (code <c>BU_ER_02</c>),
        /// ou si <paramref name="take"/> est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la pagination.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<List<T>> HandleGetPagedAsNoTrackingAsync(
            string caller,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            int skip,
            int take,
            CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleGetPagedAsNoTrackingAsync)}";

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

                return await _repository.GetPagedAsNoTrackingAsync(callChain, predicate, orderBy, skip, take, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors du comptage.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<int> HandleCountAsync(string caller, Expression<Func<T, bool>>? predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleCountAsync)}";

            try
            {
                ct.ThrowIfCancellationRequested();

                return await _repository.CountAsync(callChain, predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        /// <inheritdoc/>
        /// <exception cref="Ex_Business">
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la vérification d'existence.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        public async Task<bool> HandleAnyByPredicateAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        {
            string callChain = $"{caller} > {_callee} > {nameof(HandleAnyByPredicateAsync)}";

            try
            {
                if (predicate is null)
                    throw new Ex_Business(
                        callChain,
                        Ex_Business.ErrorCodes.BU_ER_01,
                        $"Le prédicat fourni pour la vérification d'existence de {typeof(T).Name} est nul.");

                ct.ThrowIfCancellationRequested();

                return await _repository.AnyByPredicateAsync(callChain, predicate, ct);
            }
            catch (Ex_Business) { throw; }
            catch (Ex_Infrastructure) { throw; }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { throw _classifier.Execute(callChain, ex); }
        }

        #endregion


        #region === Méthodes privées ===

        // A compléter

        #endregion
    }
}