using LeitTemporImport.A_Domain.Common.Exceptions;
using LeitTemporImport.A_Domain.Interfaces.Handlers.Commands;
using LeitTemporImport.A_Domain.Interfaces.Repositories.Generic;
using LeitTemporImport.C_Infrastructure.Persistence.DIGIT_TRY.Entities;

namespace LeitTemporImport.A_Domain.Interfaces.Handlers.Generic
{
    /// <summary>
    /// <para>Description</para>
    /// <para>
    /// CommandHandler générique fournissant les commandes CRUD standard pour toute entité <typeparamref name="T"/>,
    /// avec historisation automatique dans <see cref="UserAppEventStore"/> sous forme de snapshot JSON.
    /// </para>
    /// <para>Contexte</para>
    /// <para>
    /// Utilisé comme classe de base des CommandHandlers spécialisés afin d’éviter la duplication
    /// des opérations standard (add/update/update-range/delete) et de la logique de journalisation.
    /// </para>
    /// <para>Objectif</para>
    /// <para>
    /// Centraliser l’exécution des écritures via <see cref="IR_Generic{T}"/> et déclencher une journalisation
    /// systématique via <see cref="IC_UserAppEventStore"/>.
    /// </para>
    /// <para>Utilisateurs cibles</para>
    /// <para>CommandHandlers de la couche UseCases.</para>
    /// <para>Tâches / Actions</para>
    /// <list type="bullet">
    /// <item><description>Exécuter les commandes CRUD.</description></item>
    /// <item><description>Mettre à jour automatiquement <c>UpdatedAt</c> si la propriété existe.</description></item>
    /// <item><description>Proposer une commande de soft delete via <c>IsDeleted</c> si la propriété existe.</description></item>
    /// <item><description>Journaliser chaque écriture dans l’Event Store (snapshot JSON).</description></item>
    /// </list>
    /// </summary>
    public interface IC_Generic<T>
    {
        /// <summary>
        /// <para>Description</para>
        /// <para>Ajoute un enregistrement <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>Persister l’entité puis écrire un snapshot dans l’Event Store.</para>
        /// <para>Tâches / Actions</para>
        /// <list type="bullet">
        /// <item><description>Construire la callChain.</description></item>
        /// <item><description>Ajouter l’entité via repository.</description></item>
        /// <item><description>Journaliser le snapshot via Event Store.</description></item>
        /// </list>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity"> Entité <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        Task HandleAddAsync(string caller, T entity, bool eventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un enregistrement <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Mettre à jour l’entité, positionner <c>UpdatedAt</c> si disponible,
        /// puis écrire un snapshot dans l’Event Store.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entity"> Entité <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        Task HandleUpdateAsync(string caller, T entity, bool eventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Met à jour un ensemble d’enregistrements <typeparamref name="T"/> et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Commande générique utilisée par les handlers spécialisés.</para>
        /// <para>Objectif</para>
        /// <para>
        /// Positionner <c>UpdatedAt</c> si disponible, effectuer l’update range,
        /// puis écrire un snapshot pour chaque entité.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="entities"> Collection d’entités <typeparamref name="T"/> à persister en base de données.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entities"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        Task HandleUpdateRangeAsync(string caller, IEnumerable<T> entities, bool eventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Supprime physiquement un enregistrement (hard delete) et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>Utilisé uniquement si le modèle autorise la suppression physique.</para>
        /// <para>Objectif</para>
        /// <para>Récupérer l’entité, la supprimer, puis journaliser le snapshot supprimé.</para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique de l’entité à supprimer.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="ArgumentNullException">Si <paramref name="entity"/> est null.</exception>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de la suppression  en base ou dans l’Event Store.</exception>
        /// </summary>
        Task HandleHardDeleteAsync(string caller, int id, bool eventStore = true, CancellationToken ct = default);

        /// <summary>
        /// <para>Description</para>
        /// <para>Réalise un soft delete (si supporté) et journalise l’opération.</para>
        /// <para>Contexte</para>
        /// <para>
        /// Utilisé lorsque l’entité supporte la suppression logique via la propriété <c>IsDeleted</c>.
        /// </para>
        /// <para>Objectif</para>
        /// <para>
        /// Charger l’entité, positionner <c>IsDeleted = true</c> (et <c>UpdatedAt</c> si disponible),
        /// puis persister et journaliser un snapshot.
        /// </para>
        /// <param name="caller">Chaîne d’appel amont utilisée pour construire la <c>callChain</c> et
        /// assurer la traçabilité complète en cas d’erreur.</param>
        /// <param name="id">Identifiant technique de l’entité à supprimer.</param>
        /// <param name="eventStore">Indique si l’opération doit être journalisée dans <see cref="UserAppEventStore</param>
        /// <param name="ct">Jeton d’annulation permettant d’interrompre l’opération si demandé.</param>
        /// <exception cref="Ex_Infrastructure">Si une erreur survient lors de l’écriture en base ou dans l’Event Store.</exception>
        /// </summary>
        Task HandleSoftDeleteAsync(string caller, int id, bool eventStore = true, CancellationToken ct = default);

    }
}