using DG244Cutting.A_Domain.Common.Exceptions;
using System.Linq.Expressions;

namespace DG244Cutting.A_Domain.Interfaces.Repositories.Generic
{
    /// <summary>
    /// Contrat générique d'un repository EF Core pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain afin de garantir que la couche Domain
    /// reste l'autorité contractuelle de l'accès aux données, sans dépendance vers EF Core ou
    /// vers C_Infrastructure. Son implémentation concrète générique <c>CR_Generic&lt;T&gt;</c>
    /// réside dans C_Infrastructure/Repositories/Generic/.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer le contrat CRUD applicable à toute entité persistée.</description></item>
    ///   <item><description>Permettre l'injection de dépendances vers un repository concret sans couplage à EF Core.</description></item>
    ///   <item><description>Propager la CallChain jusqu'à la couche de persistance pour garantir la traçabilité complète.</description></item>
    ///   <item><description>Supporter l'annulation coopérative des opérations asynchrones via <see cref="CancellationToken"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne gère pas les transactions : cette responsabilité appartient exclusivement au UseCase appelant.</description></item>
    ///   <item><description>Ne contient aucune logique de validation métier : ce rôle appartient au Command Handler appelant.</description></item>
    ///   <item><description>Ne persiste pas les changements (<c>SaveChangesAsync</c> est absent de ce contrat).</description></item>
    ///   <item><description>
    ///     Ne positionne pas les champs techniques d'audit (<c>CreatedAt</c>, <c>UpdatedAt</c>, <c>IsDeleted</c>) :
    ///     cette responsabilité appartient au Command Handler appelant (<c>CH_Generic&lt;T&gt;</c>).
    ///   </description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité manipulée par le repository. Doit être une classe référence.
    /// </typeparam>
    public interface IR_Generic<T> where T : class
    {
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
        /// Levée si une défaillance technique EF Core survient lors de l'inscription dans le change tracker.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task AddAsync(string caller, T entity, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<T?> GetByIdAsync(string caller, int id, CancellationToken ct = default);

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
        /// (type <see cref="int"/>) : <c>FindAsync</c> ne supportant pas <c>AsNoTracking</c>,
        /// la résolution est effectuée par <c>Where</c> + <c>FirstOrDefaultAsync</c>.
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
        /// Levée si une défaillance technique EF Core survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<T?> GetByIdAsNoTrackingAsync(string caller, int id, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<T?> GetFirstOrDefaultAsync(string caller, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<T?> GetFirstOrDefaultAsNoTrackingAsync(string caller, CancellationToken ct = default);

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
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<T?> GetFirstOrDefaultAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la vérification d'existence.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<bool> GetAnyAsync(string caller, int id, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la récupération de la liste.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<T>> GetAllAsync(string caller, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la récupération de la liste.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<T>> GetAllAsNoTrackingAsync(string caller, CancellationToken ct = default);

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
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture filtrée.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<T>> GetFilteredAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);

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
        /// Levée si le prédicat fourni est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la lecture filtrée.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<T>> GetFilteredAsNoTrackingAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);


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
        /// Levée si une défaillance technique EF Core survient lors de l'exécution de la requête paginée.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<List<T>> GetPagedAsNoTrackingAsync(
            string caller,
            Expression<Func<T, bool>>? predicate,
            Expression<Func<T, object>> orderBy,
            int skip,
            int take,
            CancellationToken ct = default);


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
        /// Levée si une défaillance technique EF Core survient lors du comptage.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<int> CountAsync(string caller, Expression<Func<T, bool>>? predicate, CancellationToken ct = default);

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
        /// Levée si une défaillance technique EF Core survient lors de la vérification d'existence.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task<bool> AnyByPredicateAsync(string caller, Expression<Func<T, bool>> predicate, CancellationToken ct = default);


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
        /// Levée si une défaillance technique EF Core survient lors du marquage en Modified.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task UpdateAsync(string caller, T entity, CancellationToken ct = default);

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
        /// Levée si la collection fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors du marquage en Modified de la collection.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task UpdateRangeAsync(string caller, IEnumerable<T> entities, CancellationToken ct = default);


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
        /// Levée si une défaillance technique EF Core survient lors de la suppression.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task DeleteAsync(string caller, int id, CancellationToken ct = default);

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
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique EF Core survient lors de la suppression logique.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task SoftDeleteAsync(string caller, int id, CancellationToken ct = default);
    }
}