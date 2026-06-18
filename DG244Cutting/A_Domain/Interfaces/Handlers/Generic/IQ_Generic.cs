using System.Linq.Expressions;
using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Generic
{
    /// <summary>
    /// Contrat générique d'un Query Handler CQRS pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain afin de constituer le contrat
    /// contractuel entre la couche B_UseCases (Services métier appelants) et les Query Handlers
    /// génériques. Son implémentation concrète <c>QH_Generic&lt;T&gt;</c> réside dans
    /// B_UseCases/Handlers/Generic/.
    /// </para>
    /// <para>
    /// Positionnement CQRS : les méthodes exposées ici sont exclusivement des opérations
    /// de lecture (Queries). Elles ne modifient jamais l'état du système et ne déclenchent
    /// aucun enregistrement Event Store. Les opérations d'écriture relèvent des Command Handlers
    /// (<c>IC_*</c>) et sont intentionnellement absentes de ce contrat.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer les opérations CRUD de lecture disponibles pour toute entité <typeparamref name="T"/>.</description></item>
    ///   <item><description>Garantir la propagation de la CallChain depuis le Service appelant jusqu'au repository.</description></item>
    ///   <item><description>Supporter l'annulation coopérative via <see cref="CancellationToken"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne mute jamais l'état d'une entité ni ne déclenche d'enregistrement Event Store.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités appartiennent au UseCase via <c>IU_LogAndNotify</c>.</description></item>
    ///   <item><description>Ne gère pas les transactions : les lectures n'écrivent pas et n'ont pas de frontière transactionnelle propre.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité métier manipulée. Doit être une classe référence.
    /// </typeparam>
    public interface IQ_Generic<T> where T : class
    {
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
        Task<T?> HandleGetByIdAsync(string caller, int id, CancellationToken ct = default);

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
        Task<T?> HandleGetByIdAsNoTrackingAsync(string caller, int id, CancellationToken ct = default);

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
        Task<T?> HandleGetFirstOrDefaultAsync(string caller, CancellationToken ct = default);

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
        Task<T?> HandleGetFirstOrDefaultAsNoTrackingAsync(string caller, CancellationToken ct = default);

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
        Task<T?> HandleGetFirstOrDefaultAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

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
        Task<bool> HandleGetAnyAsync(string caller, int id, CancellationToken ct = default);

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
        Task<List<T>> HandleGetAllAsync(string caller, CancellationToken ct = default);

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
        Task<List<T>> HandleGetAllAsNoTrackingAsync(string caller, CancellationToken ct = default);

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
        Task<List<T>> HandleGetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

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
        Task<List<T>> HandleGetFilteredAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

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
        Task<List<T>> HandleGetPagedAsNoTrackingAsync(
            string caller,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            int skip,
            int take,
            CancellationToken ct = default);

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
        Task<int> HandleCountAsync(string caller, Expression<Func<T, bool>>? predicate, CancellationToken ct = default);

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
        Task<bool> HandleAnyByPredicateAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }
}