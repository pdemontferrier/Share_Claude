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
        /// </summary>
        /// <remarks>
        /// Privé : les classes dérivées accèdent aux lectures du socle via les méthodes Handle* héritées,
        /// jamais directement via ce champ. Pour leurs lectures spécialisées, elles injectent leur
        /// propre repository spécialisé dans leur propre constructeur.
        /// </remarks>
        private readonly IR_Generic<T> _repository;

        /// <summary>
        /// Service de classification des exceptions non contrôlées en types applicatifs normalisés
        /// (<see cref="Ex_Infrastructure"/> ou <see cref="Ex_Unclassified"/>).
        /// </summary>
        private readonly IS_ExClassifier _classifier;

        #endregion


        #region === Constructeur ===

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="QH_Generic{T}"/> avec ses dépendances opérationnelles.
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

        /// <summary>
        /// Retourne l'entité correspondant à l'identifiant spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// Variante NoTracking de <see cref="HandleGetByIdAsync"/>, par délégation à
        /// <see cref="IR_Generic{T}.GetByIdAsNoTrackingAsync"/>. À privilégier pour les lectures
        /// pures où l'entité retournée ne sera pas mutée.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant entier de l'entité recherchée. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>L'entité de type <typeparamref name="T"/> si trouvée, non suivie ; <see langword="null"/> sinon.</returns>
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

        /// <summary>
        /// Retourne le premier enregistrement de la table sans suivi des changements,
        /// ou <see langword="null"/> si la table est vide.
        /// </summary>
        /// <remarks>
        /// Variante NoTracking de <see cref="HandleGetFirstOrDefaultAsync(string, CancellationToken)"/>,
        /// par délégation à <see cref="IR_Generic{T}.GetFirstOrDefaultAsNoTrackingAsync(string, CancellationToken)"/>.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Le premier enregistrement de type <typeparamref name="T"/>, non suivi ; <see langword="null"/> si la table est vide.</returns>
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

        /// <summary>
        /// Retourne le premier enregistrement satisfaisant le prédicat spécifié, sans suivi des changements,
        /// ou <see langword="null"/> si aucun enregistrement ne satisfait le prédicat.
        /// </summary>
        /// <remarks>
        /// Par délégation à <see cref="IR_Generic{T}.GetFirstOrDefaultAsNoTrackingAsync(string, Expression{Func{T, bool}}, CancellationToken)"/> :
        /// le filtrage est exécuté côté base de données via une clause <c>WHERE</c> SQL, suivie
        /// d'une limitation à un seul enregistrement.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Le premier enregistrement satisfaisant le prédicat, non suivi ; <see langword="null"/> sinon.</returns>
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

        /// <summary>
        /// Retourne l'ensemble des enregistrements de la table avec suivi des changements actif.
        /// </summary>
        /// <remarks>
        /// Utiliser <see cref="HandleGetAllAsNoTrackingAsync"/> pour les lectures seules afin d'éviter
        /// la surcharge du change tracker EF Core.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste complète de toutes les entités <typeparamref name="T"/> persistées.</returns>
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

        /// <summary>
        /// Retourne l'ensemble des enregistrements de la table sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// À privilégier pour les opérations de lecture pure. Le mode <c>AsNoTracking</c>
        /// améliore les performances et réduit la consommation mémoire.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste complète de toutes les entités <typeparamref name="T"/>, non suivies.</returns>
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

        /// <summary>
        /// Retourne les enregistrements satisfaisant le prédicat de filtrage spécifié.
        /// </summary>
        /// <remarks>
        /// Le filtrage est exécuté côté base de données via une clause <c>WHERE</c> SQL générée
        /// par EF Core, par délégation à <see cref="IR_Generic{T}.GetFilteredAsync"/>.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste des entités <typeparamref name="T"/> satisfaisant le prédicat.</returns>
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

        /// <summary>
        /// Retourne les enregistrements satisfaisant le prédicat de filtrage spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// Variante NoTracking de <see cref="HandleGetFilteredAsync"/>, par délégation à
        /// <see cref="IR_Generic{T}.GetFilteredAsNoTrackingAsync"/>. À privilégier pour les
        /// lectures pures où les entités retournées ne seront pas mutées.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage appliquée à chaque entité. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Liste des entités <typeparamref name="T"/> satisfaisant le prédicat, non suivies.</returns>
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

        /// <summary>
        /// Retourne une fenêtre d'enregistrements paginée, satisfaisant éventuellement un prédicat,
        /// triée selon le critère spécifié, sans suivi des changements.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Par délégation à <see cref="IR_Generic{T}.GetPagedAsNoTrackingAsync"/> : la requête est
        /// traduite par EF Core en <c>OFFSET ... FETCH NEXT ... ROWS ONLY</c> (SQL Server) ou équivalent,
        /// ce qui limite le nombre d'enregistrements ramenés du serveur de base de données.
        /// </para>
        /// <para>
        /// Le critère de tri est obligatoire : sans <c>ORDER BY</c>, la pagination SQL retourne des
        /// résultats non déterministes entre deux appels successifs.
        /// </para>
        /// <para>
        /// Pour obtenir le nombre total d'enregistrements satisfaisant le prédicat (affichage "page 3 sur 12"),
        /// l'appelant effectue un appel séparé à <see cref="HandleCountAsync"/> avec le même prédicat.
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

        /// <summary>
        /// Retourne le nombre d'enregistrements de la table, éventuellement restreint par un prédicat.
        /// </summary>
        /// <remarks>
        /// Par délégation à <see cref="IR_Generic{T}.CountAsync"/> : l'opération est traduite en
        /// <c>SELECT COUNT(*)</c> SQL côté serveur de base de données, sans matérialisation
        /// d'enregistrement en mémoire. À utiliser conjointement avec
        /// <see cref="HandleGetPagedAsNoTrackingAsync"/> pour calculer le nombre total de pages.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage. Peut être <see langword="null"/> : le comptage porte alors sur la table entière.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns>Nombre d'enregistrements satisfaisant le prédicat (ou de la table entière si <paramref name="predicate"/> est <see langword="null"/>).</returns>
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

        /// <summary>
        /// Indique si au moins un enregistrement de la table satisfait le prédicat spécifié.
        /// </summary>
        /// <remarks>
        /// Par délégation à <see cref="IR_Generic{T}.AnyByPredicateAsync"/> : l'opération est traduite
        /// en <c>SELECT TOP 1 1 WHERE ...</c> SQL côté serveur de base de données, sans matérialisation
        /// d'enregistrement en mémoire. Complète <see cref="HandleGetAnyAsync"/> qui est limité à
        /// la vérification d'existence par clé primaire.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="predicate">Expression booléenne de filtrage. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <returns><see langword="true"/> si au moins un enregistrement satisfait le prédicat ; <see langword="false"/> sinon.</returns>
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