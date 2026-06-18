using DG244Cutting.A_Domain.Common.Exceptions;

namespace DG244Cutting.A_Domain.Interfaces.Handlers.Generic
{
    /// <summary>
    /// Contrat générique d'un Command Handler CQRS pour tout type d'entité <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contexte : cette interface est définie dans A_Domain afin de constituer le contrat
    /// contractuel entre la couche B_UseCases (Services métier appelants) et les Command Handlers
    /// génériques. Son implémentation concrète <c>CH_Generic&lt;T&gt;</c> réside dans
    /// B_UseCases/Handlers/Generic/.
    /// </para>
    /// <para>
    /// Positionnement CQRS : les méthodes exposées ici sont exclusivement des opérations
    /// d'écriture (Commands). Elles modifient l'état du système et s'exécutent toujours dans
    /// le périmètre transactionnel ouvert par le UseCase orchestrateur. Les opérations de lecture
    /// relèvent des Query Handlers (<c>IQ_*</c>) et sont intentionnellement absentes de ce contrat.
    /// </para>
    /// <para>
    /// Gestion des champs d'audit : les méthodes de ce contrat garantissent le positionnement
    /// automatique des champs techniques communs à toutes les entités persistées :
    /// </para>
    /// <list type="bullet">
    ///   <item><description><c>CreatedAt</c> : positionné à la date UTC courante lors de <see cref="HandleAddAsync"/>.</description></item>
    ///   <item><description><c>UpdatedAt</c> : positionné à la date UTC courante lors de <see cref="HandleUpdateAsync"/>, <see cref="HandleUpdateRangeAsync"/> et <see cref="HandleSoftDeleteAsync"/>.</description></item>
    ///   <item><description><c>IsDeleted</c> : positionné à <see langword="true"/> exclusivement lors de <see cref="HandleSoftDeleteAsync"/>.</description></item>
    /// </list>
    /// <para>
    /// Ce positionnement repose sur une convention de nommage de propriétés : toute entité
    /// exposant une propriété publique accessible en écriture nommée <c>CreatedAt</c>, <c>UpdatedAt</c>
    /// ou <c>IsDeleted</c> verra ces propriétés automatiquement renseignées. Les entités ne
    /// les possédant pas ne sont pas impactées.
    /// </para>
    /// <para>Responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Déclarer les opérations CRUD d'écriture disponibles pour toute entité <typeparamref name="T"/>.</description></item>
    ///   <item><description>Garantir la propagation de la CallChain depuis le Service appelant jusqu'au repository.</description></item>
    ///   <item><description>Assurer la solidarité transactionnelle entre la mutation métier et son enregistrement Event Store.</description></item>
    ///   <item><description>Positionner automatiquement les champs techniques d'audit avant toute mutation.</description></item>
    ///   <item><description>Supporter l'annulation coopérative via <see cref="CancellationToken"/>.</description></item>
    /// </list>
    /// <para>Non-responsabilités :</para>
    /// <list type="bullet">
    ///   <item><description>Ne gère pas les transactions : cette responsabilité appartient exclusivement au UseCase.</description></item>
    ///   <item><description>Ne journalise pas, ne notifie pas : ces responsabilités appartiennent au UseCase via <c>IU_LogAndNotify</c>.</description></item>
    ///   <item><description>Ne persiste pas les changements (<c>SaveChangesAsync</c> absent de ce contrat).</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">
    /// Type de l'entité métier manipulée. Doit être une classe référence.
    /// </typeparam>
    public interface IC_Generic<T> where T : class
    {
        /// <summary>
        /// Inscrit une entité en création dans le contexte EF Core, positionne <c>CreatedAt</c>
        /// à la date UTC courante, et enregistre l'événement correspondant dans l'Event Store.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Le champ <c>CreatedAt</c> est positionné automatiquement si la propriété existe sur
        /// le type <typeparamref name="T"/>. Le champ <c>UpdatedAt</c> n'est intentionnellement
        /// pas renseigné à la création. Le champ <c>IsDeleted</c> conserve sa valeur par défaut
        /// (<see langword="false"/> / <c>0</c> en base de données).
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant, transmise pour enrichissement et traçabilité.</param>
        /// <param name="entity">Entité de type <typeparamref name="T"/> à créer. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleAddAsync(string caller, T entity, CancellationToken ct = default);

        /// <summary>
        /// Inscrit une entité en mise à jour dans le contexte EF Core, positionne <c>UpdatedAt</c>
        /// à la date UTC courante, et enregistre l'événement correspondant dans l'Event Store.
        /// </summary>
        /// <remarks>
        /// Le champ <c>CreatedAt</c> n'est jamais altéré lors d'une mise à jour.
        /// Le champ <c>IsDeleted</c> n'est pas forcé : toute décision relative à ce champ
        /// appartient à la logique métier du UseCase appelant.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="entity">Entité de type <typeparamref name="T"/> à mettre à jour. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'entité fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleUpdateAsync(string caller, T entity, CancellationToken ct = default);

        /// <summary>
        /// Inscrit un ensemble d'entités en mise à jour dans le contexte EF Core, positionne
        /// <c>UpdatedAt</c> à la date UTC courante sur chacune, et enregistre un événement
        /// Event Store pour chaque entité de manière séquentielle.
        /// </summary>
        /// <remarks>
        /// Les enregistrements Event Store sont produits séquentiellement afin de garantir la
        /// cohérence des opérations sur le DbContext partagé, qui n'est pas thread-safe.
        /// Le champ <c>CreatedAt</c> n'est jamais altéré lors d'une mise à jour en masse.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="entities">Collection d'entités de type <typeparamref name="T"/> à mettre à jour. Ne doit pas être <see langword="null"/>.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si la collection fournie est <see langword="null"/> (code <c>BU_ER_01</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la persistance ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleUpdateRangeAsync(string caller, IEnumerable<T> entities, CancellationToken ct = default);

        /// <summary>
        /// Supprime physiquement l'entité correspondant à l'identifiant spécifié et enregistre
        /// l'événement correspondant dans l'Event Store, si l'entité existe.
        /// </summary>
        /// <remarks>
        /// Si aucune entité ne correspond à l'identifiant fourni, la méthode retourne sans erreur
        /// et sans enregistrement Event Store : il n'y a pas de mutation à tracer.
        /// Pour une suppression logique, utiliser <see cref="HandleSoftDeleteAsync"/> à la place.
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant de l'entité à supprimer physiquement. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture, de la suppression ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleDeleteAsync(string caller, int id, CancellationToken ct = default);

        /// <summary>
        /// Réalise la suppression logique (soft delete) de l'entité correspondant à l'identifiant
        /// spécifié, et enregistre l'événement correspondant dans l'Event Store si l'entité existe.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Cette méthode ne supprime jamais physiquement l'enregistrement en base de données.
        /// Elle positionne le champ <c>IsDeleted</c> à <see langword="true"/> et le champ
        /// <c>UpdatedAt</c> à la date UTC courante sur l'entité, puis délègue la persistance
        /// au repository via une mise à jour.
        /// </para>
        /// <para>
        /// Si aucune entité ne correspond à l'identifiant fourni, la méthode retourne sans erreur
        /// et sans enregistrement Event Store : il n'y a pas de mutation à tracer.
        /// </para>
        /// <para>
        /// Si le type <typeparamref name="T"/> ne possède pas de propriété <c>IsDeleted</c>
        /// accessible en écriture, une <see cref="Ex_Business"/> est levée (code <c>BU_ER_03</c>) :
        /// le soft delete n'est pas applicable à cette entité.
        /// </para>
        /// </remarks>
        /// <param name="caller">CallChain construite par le composant appelant.</param>
        /// <param name="id">Identifiant de l'entité à marquer comme supprimée logiquement. Doit être strictement positif.</param>
        /// <param name="ct">Jeton d'annulation permettant d'interrompre l'opération de manière coopérative.</param>
        /// <exception cref="Ex_Business">
        /// Levée si l'identifiant fourni est inférieur ou égal à zéro (code <c>BU_ER_02</c>),
        /// ou si le type <typeparamref name="T"/> ne supporte pas le soft delete (code <c>BU_ER_03</c>).
        /// </exception>
        /// <exception cref="Ex_Infrastructure">
        /// Levée si une défaillance technique survient lors de la lecture, de la mise à jour ou de l'enregistrement Event Store.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Levée si l'annulation est signalée via <paramref name="ct"/> avant ou pendant l'exécution.
        /// </exception>
        Task HandleSoftDeleteAsync(string caller, int id, CancellationToken ct = default);
    }
}